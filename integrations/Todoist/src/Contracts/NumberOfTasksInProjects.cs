namespace Integrations.Todoist.Contracts;

internal sealed record NumberOfTasksInProjects(
    int NextActions,
    int Someday,
    int Recurring);
