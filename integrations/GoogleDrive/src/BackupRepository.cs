using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

namespace Integrations.GoogleDrive;

internal sealed class BackupRepository(
    BlobServiceClient blobServiceClient,
    IOptions<AccountingDocumentationOptions> options)
{
    private readonly BlobContainerClient _blobContainerClient =
        blobServiceClient.GetBlobContainerClient(options.Value.BackupContainerName);

    public async Task UploadAsync(
        string filename,
        byte[] content,
        CancellationToken cancellationToken)
    {
        await _blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var blobClient = _blobContainerClient.GetBlobClient(filename);
        var binaryData = BinaryData.FromBytes(content);
        await blobClient.UploadAsync(binaryData, cancellationToken);
    }
}
