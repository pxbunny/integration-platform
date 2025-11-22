namespace Integrations.Todoist.TodoistClient;

internal static class Extensions
{
    public static async Task<IEnumerable<TodoistTask>> GetAllTasksAsync(this ITodoistApi api,
        IList<string> ids,
        CancellationToken cancellationToken = default)
    {
        TodoistResponse? response = null;
        List<TodoistTask> tasks = [];

        do
        {
            response = await api.GetTasksAsync(string.Join(",", ids), response?.NextCursor, cancellationToken);
            tasks.AddRange(response.Results);
        }
        while (!string.IsNullOrWhiteSpace(response.NextCursor));

        return tasks;
    }

    public static async Task<IEnumerable<TodoistTask>> GetAllTasksByFilterAsync(this ITodoistApi api,
        string query,
        CancellationToken cancellationToken = default)
    {
        TodoistResponse? response = null;
        List<TodoistTask> tasks = [];

        do
        {
            response = await api.GetTasksByFilterAsync(query, response?.NextCursor, cancellationToken);
            tasks.AddRange(response.Results);
        }
        while (!string.IsNullOrWhiteSpace(response.NextCursor));

        return tasks;
    }

    public static async Task<int> UpdateTasksAsync(this ITodoistApi api,
        IEnumerable<TodoistTask> tasks,
        Func<TodoistTask, TodoistUpdateTaskRequest> createRequestBody,
        int concurrentRequests = 5,
        CancellationToken cancellationToken = default)
    {
        using var semaphore = new SemaphoreSlim(concurrentRequests);
        var updateCounter = 0;

        var apiCallTasks = tasks.Select(async task =>
        {
            await semaphore.WaitAsync(cancellationToken);

            try
            {
                var updateRequest = createRequestBody(task);
                await api.UpdateTaskAsync(task.Id, updateRequest, cancellationToken);
                Interlocked.Increment(ref updateCounter);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(apiCallTasks);
        return updateCounter;
    }
}
