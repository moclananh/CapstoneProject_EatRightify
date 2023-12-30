using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.Common
{
    public class FileStorageService : IStorageService
    {
        private readonly string _userContentFolder;
        private const string USER_CONTENT_FOLDER_NAME = "Images";
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public FileStorageService(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _userContentFolder = Path.Combine(webHostEnvironment.ContentRootPath, USER_CONTENT_FOLDER_NAME);
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }
        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _userContentFolder = Path.Combine(webHostEnvironment.ContentRootPath, USER_CONTENT_FOLDER_NAME);
            _webHostEnvironment = webHostEnvironment;
        }

        public string GetFileUrl(string fileName)
        {
            return $"/{USER_CONTENT_FOLDER_NAME}/{fileName}";
        }

        /*      public async Task SaveFileAsync(Stream mediaBinaryStream, string fileName)
              {
                  var filePath = Path.Combine(_userContentFolder, fileName);
                  using var output = new FileStream(filePath, FileMode.Create);
                  await mediaBinaryStream.CopyToAsync(output);
              }*/

        public async Task DeleteFileAsync(string fileName)
        {
            var filePath = Path.Combine(_userContentFolder, fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }

        public async Task<string> SaveImageAsync(IFormFile image)
        {
            // Retrieve Azure Storage connection string from App Settings
            var connectionString = _configuration.GetConnectionString("AzureBlobStorageConnection");

            // Retrieve the container name from App Settings
            var containerName = _configuration["ersimages"];

            // Create a unique filename for the blob
            string fileName = Guid.NewGuid().ToString() + image.FileName;

            // Create a blob client using the connection string
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Create a container client using the container name
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Create a blob client for the new blob
            var blobClient = containerClient.GetBlobClient(fileName);

            // Set the content type based on the file extension (example: JPEG)
            var contentType = GetContentType(image.FileName);

            // Open a stream to the blob and upload the file with content type
            using (var stream = image.OpenReadStream())
            {
                var options = new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = contentType },
                };

                await blobClient.UploadAsync(stream, options);
            }

            // Return the URL of the uploaded blob
            return blobClient.Uri.ToString();
        }

        private string GetContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
