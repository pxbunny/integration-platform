using Refit;

namespace Integrations.Todoist.TodoistClient;

internal interface ITodoistApi
{
    [Get("/tasks?ids={ids}&cursor={cursor}")]
    Task<TodoistResponse> GetTasksAsync(
        string ids,
        string? cursor = null,
        CancellationToken cancellationToken = default);

    [Get("/tasks/filter?query={query}&cursor={cursor}")]
    Task<TodoistResponse> GetTasksByFilterAsync(
        string query,
        string? cursor = null,
        CancellationToken cancellationToken = default);

    [Post("/tasks/{taskId}")]
    Task UpdateTaskAsync(
        string taskId,
        [Body] object request,
        CancellationToken cancellationToken = default);
}
