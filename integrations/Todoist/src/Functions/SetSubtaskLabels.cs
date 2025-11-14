using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Integrations.Todoist.Functions;

internal sealed class SetSubtaskLabels(ITodoistApi todoist, ILogger<SetSubtaskLabels> logger)
{
    [Function(nameof(SetSubtaskLabels))]
    public async Task RunAsync(
        [TimerTrigger("0 0 2 * * *", UseMonitor = false, RunOnStartup = true)] TimerInfo timer)
    {
        logger.LogInformation("Timer trigger function executed at: {TriggerTime}", DateTime.Now);

        await HandleFunctionAsync();

        if (timer.ScheduleStatus is null) return;

        logger.LogInformation("Next timer schedule at: {NextTriggerAt}", timer.ScheduleStatus.Next);
    }

    private async Task HandleFunctionAsync()
    {
        const string subtaskLabel = "subtask";

        var tasks = (await GetTasksAsync())
            .Where(t => !string.IsNullOrWhiteSpace(t.ParentId)); // just to make sure

        var updateRequest = new TodoistUpdateTaskRequest
        {
            Labels = [subtaskLabel]
        };

        foreach (var task in tasks)
        {
            await todoist.UpdateTaskAsync(task.Id, updateRequest);
        }
    }

    private async Task<IEnumerable<TodoistTask>> GetTasksAsync()
    {
        const string filter = "subtask";

        TodoistResponse? response = null;
        List<TodoistTask> tasks = [];

        do
        {
            response = await todoist.GetTasksByFilterAsync(query: filter, cursor: response?.NextCursor);
            tasks.AddRange(response.Results);
        } while (!string.IsNullOrWhiteSpace(response.NextCursor));

        return tasks;
    }
}
