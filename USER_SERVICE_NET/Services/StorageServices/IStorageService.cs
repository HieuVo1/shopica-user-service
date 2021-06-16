using Google.Apis.Drive.v3;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using USER_SERVICE_NET.ViewModels.Commons;

namespace USER_SERVICE_NET.Services.StorageServices
{
    public interface IStorageService
    {
        DriveService GetService();
        Task<string> UploadFileAsync(IFormFile file);
        Task SaveFileAsync(Stream mediaBinaryStream, string folder, string fileName);
        Task DeleteFileAsync(string folder, string fileName);
    }
}
