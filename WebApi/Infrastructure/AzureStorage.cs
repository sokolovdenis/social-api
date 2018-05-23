using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WebApi.Infrastructure
{
	public class AzureStorage
	{
		private readonly CloudStorageAccount _storageAccount;
		private const string _usersContainerName = "users";
		private const string _postsContainerName = "posts";

		public AzureStorage(string connectionString)
		{
			if (!CloudStorageAccount.TryParse(connectionString, out _storageAccount))
			{
				throw new ArgumentException(nameof(connectionString));
			}
		}

		public async Task<string> UploadUserImage(Stream fileStream, string fileName)
		{
			return await UploadFile(fileStream, fileName, "image/jpg", _usersContainerName);
		}

		public async Task<string> UploadPostImageAsync(Stream fileStream, string fileName)
		{
			return await UploadFile(fileStream, fileName, "image/jpg", _postsContainerName);
		}

		private async Task<string> UploadFile(Stream fileStream, string fileName, string contentType, string containerName)
		{
			CloudBlobClient client = _storageAccount.CreateCloudBlobClient();
			CloudBlobContainer container = client.GetContainerReference(containerName);

			if (await container.CreateIfNotExistsAsync())
			{
				await container.SetPermissionsAsync(new BlobContainerPermissions
				{
					PublicAccess = BlobContainerPublicAccessType.Blob
				});
			}

			CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

			fileStream.Position = 0;
			await blockBlob.UploadFromStreamAsync(fileStream);

			blockBlob.Properties.ContentType = contentType;
			await blockBlob.SetPropertiesAsync();

			return await Task.FromResult(blockBlob.StorageUri.PrimaryUri.ToString());
		}
	}
}
