namespace Integrations.Contracts.Todoist;

internal sealed record NumberOfTasksInProjects(
    int NextActions,
    int Someday,
    int Recurring);
