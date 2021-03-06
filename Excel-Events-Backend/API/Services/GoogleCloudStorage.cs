using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using API.Services.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace API.Services
{
    public class GoogleCloudStorage : ICloudStorage
    {
        private readonly GoogleCredential googleCredential;
        private readonly StorageClient storageClient;
        private readonly string bucketName;

        public GoogleCloudStorage()
        {
            googleCredential = GoogleCredential.FromFile(Environment.GetEnvironmentVariable("GOOGLE_CREDENTIAL_FILE"));
            storageClient = StorageClient.Create(googleCredential);
            bucketName = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_STORAGE_BUCKET");
        }

        public async Task<string> UploadFileAsync(IFormFile imageFile, string fileNameForStorage)
        {
            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                var dataObject = await storageClient.UploadObjectAsync(bucketName, fileNameForStorage, null, memoryStream);
                dataObject.Acl = dataObject.Acl ?? new List<ObjectAccessControl>();
                storageClient.UpdateObject(dataObject, new UpdateObjectOptions
                {
                    PredefinedAcl = PredefinedObjectAcl.PublicRead
                });
                return dataObject.MediaLink;
            }
        }

        public async Task DeleteFileAsync(string fileNameForStorage)
        {
            await storageClient.DeleteObjectAsync(bucketName, fileNameForStorage);
        }
    }
}