using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FirstAzureApp.Services
{
    public class ImageStore : IImageStore
    {
        private readonly IConfiguration configuration;
        CloudBlobClient blobClient;
        public ImageStore(IConfiguration configuration)
        {
            this.configuration = configuration;
            var credentials = new StorageCredentials(this.configuration["StorageName"]??"", this.configuration["BlobKey"]??"");
            blobClient = new CloudBlobClient(new Uri(this.configuration["StorageBaseUri"] ?? ""), credentials);
           
        }
        public async Task<string> SaveImage(Stream imageStream)
        {
            var imageId = Guid.NewGuid().ToString();
            var container = blobClient.GetContainerReference("images");
            var blob = container.GetBlockBlobReference(imageId);
            await blob.UploadFromStreamAsync(imageStream);
            return imageId;
        }

        public string UriFor(string imageId)
        {
            var sasPolicy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(15),
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-15)

            };
            var container = blobClient.GetContainerReference("images");
            var blob = container.GetBlockBlobReference(imageId);
            var sas = blob.GetSharedAccessSignature(sasPolicy);
            return $"{this.configuration["StorageBaseUri"]}/images/{imageId}{sas}";
        }
    }
}
