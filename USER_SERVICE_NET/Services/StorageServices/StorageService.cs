using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using USER_SERVICE_NET.Utilities;

namespace USER_SERVICE_NET.Services.StorageServices
{
    public class StorageService : IStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        public StorageService(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        public async Task SaveFileAsync(Stream mediaBinaryStream, string folder, string fileName)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folder, fileName);
            using FileStream output = new FileStream(filePath, FileMode.Create);
            {
                await mediaBinaryStream.CopyToAsync(output);
            }
            
        }

        public async Task DeleteFileAsync(string folder, string fileName)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folder, fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }

        public DriveService GetService()
        {
            ClientSecrets clientSecrets = new ClientSecrets();
            clientSecrets.ClientId = _configuration.GetSection("Authentication:Google:ClientId").Value;
            clientSecrets.ClientSecret = _configuration.GetSection("Authentication:Google:ClientSecret").Value;

            string FilePath = Path.Combine(_webHostEnvironment.WebRootPath, "DriveServiceCredentials.json");

            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                clientSecrets,
                Constant.DriveServiceScopes,
                "user",
                CancellationToken.None,
                new FileDataStore(FilePath, true)).Result;

            //create Drive API service.
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Constant.DriveServiceAppName,
            });
            return service;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                DriveService service = GetService();
                FilesResource filesResource = new FilesResource(service);
                string fileId = filesResource.GenerateIds().Execute().Ids[0];

                var FileMetaData = new Google.Apis.Drive.v3.Data.File();
                FileMetaData.Name = Path.GetFileName(file.FileName);
                FileMetaData.MimeType = file.ContentType;
                FileMetaData.Id = fileId;
                FileMetaData.Parents = Constant.DriveServiceFolder;

                FilesResource.CreateMediaUpload request;

                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    request = service.Files.Create(FileMetaData, ms, file.ContentType);
                    var result  = await request.UploadAsync();
                    if( result.Status == UploadStatus.Completed)
                    {
                        return String.Format(Constant.DriveServiceBaseImageUrl, fileId);
                    }
                    else
                    {
                        return String.Format(Constant.DriveServiceBaseImageUrl, Constant.DriveServiceNotFoundImage);
                    }
                } 
            }
            return String.Format(Constant.DriveServiceBaseImageUrl, Constant.DriveServiceNotFoundImage);
        }
    }
}
