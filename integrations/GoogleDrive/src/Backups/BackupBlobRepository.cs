using Azure.Storage.Blobs;

namespace Integrations.GoogleDrive.Backups;

internal sealed class BackupBlobRepository(BlobServiceClient blobServiceClient)
{
    public async Task UploadAsync(
        string containerName,
        string filename,
        byte[] content,
        CancellationToken cancellationToken)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var blobClient = containerClient.GetBlobClient(filename);
        var binaryData = BinaryData.FromBytes(content);
        await blobClient.UploadAsync(binaryData, cancellationToken);
    }
}
