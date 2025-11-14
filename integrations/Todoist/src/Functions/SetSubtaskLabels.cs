using System.Threading;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Integrations.Todoist.Functions;

internal sealed class SetSubtaskLabels(ITodoistApi todoist, ILogger<SetSubtaskLabels> logger)
{
    [Function(nameof(SetSubtaskLabels))]
    public async Task RunAsync(
        [TimerTrigger("%SubtaskLabelsCheckSchedule%", UseMonitor = false, RunOnStartup = true)] TimerInfo _,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Timer trigger function executed at: {TriggerTime}", DateTime.Now);

        try
        {
            await HandleFunctionAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while setting subtask labels.");
            throw;
        }
    }

    private async Task HandleFunctionAsync(CancellationToken cancellationToken)
    {
        const string subtaskLabel = "subtask";

        var tasks = (await GetTasksAsync(cancellationToken))
            .Where(t => !string.IsNullOrWhiteSpace(t.ParentId)) // just to make sure
            .Where(t => !t.Labels.Contains(subtaskLabel) || t.Labels.Count() > 1)
            .ToList();

        if (tasks.Count == 0)
        {
            logger.LogInformation("No subtasks to update.");
            return;
        }

        var updatedCount = await UpdateSubtaskLabelsAsync(tasks, subtaskLabel, cancellationToken);

        logger.LogInformation("Updated labels for {UpdatedCount} subtasks.", updatedCount);
    }

    private async Task<IEnumerable<TodoistTask>> GetTasksAsync(CancellationToken cancellationToken)
    {
        const string requestQuery = "subtask";

        TodoistResponse? response = null;
        List<TodoistTask> tasks = [];

        do
        {
            response = await todoist.GetTasksByFilterAsync(
                query: requestQuery,
                cursor: response?.NextCursor,
                cancellationToken);
            tasks.AddRange(response.Results);
        } while (!string.IsNullOrWhiteSpace(response.NextCursor));

        return tasks;
    }

    private async Task<int> UpdateSubtaskLabelsAsync(
        IEnumerable<TodoistTask> tasks,
        string subtaskLabel,
        CancellationToken cancellationToken)
    {
        var updateRequest = new TodoistUpdateTaskRequest
        {
            Labels = [subtaskLabel]
        };

        using var semaphoreSlim = new SemaphoreSlim(5);
        var updateCounter = 0;

        IEnumerable<Task> apiCallTasks = tasks.Select(async task =>
        {
            await semaphoreSlim.WaitAsync(cancellationToken);

            try
            {
                await todoist.UpdateTaskAsync(task.Id, updateRequest, cancellationToken);
                Interlocked.Increment(ref updateCounter);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        });

        await Task.WhenAll(apiCallTasks);
        return updateCounter;
    }
}
