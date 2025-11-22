namespace Integrations.GoogleDrive;

internal sealed class AccountingDocumentationOptions
{
    public const string SectionName = "AccountingDocumentation";

    public string DriveFolderId { get; init; } = null!;

    public string BackupContainerName { get; init; } = null!;

    public string BackupFileNamePrefix { get; init; } = null!;
}
