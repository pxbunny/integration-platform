using Newtonsoft.Json;

namespace Integrations.Todoist.TodoistClient;

internal sealed record TodoistResponse(
    IEnumerable<TodoistTask> Results,
    [JsonProperty(PropertyName = "next_cursor")] string NextCursor);

internal sealed record TodoistTask(
    string Id,
    [JsonProperty(PropertyName = "parent_id")] string ParentId,
    IEnumerable<string> Labels,
    object? Due, // TODO: specify type if needed
    string Content,
    string Description);
