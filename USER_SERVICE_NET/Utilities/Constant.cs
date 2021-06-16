using Google.Apis.Drive.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace USER_SERVICE_NET.Utilities
{
    public class Constant
    {
        public const string ConnectionString = "ShopicaDatabase";

        public const string BaseAppUrl = "https://localhost:5001";

        public const string UserImageFolder = "UserImages";

        public const long TokenExpireTime = 1000 * 60 * 10; // 10 minutes

        public const string HttpClientMediaType = "application/json";

        public const string ShopicaUrl = "http://localhost:4200";

        // google-driver
        public static readonly string[] DriveServiceScopes = { DriveService.Scope.Drive };

        public static readonly string[] DriveServiceFolder ={ "1L8wce4Ow8409OWbzhlCy93C5fKrAOlXs" } ;

        public const string DriveServiceAppName = "GoogleDriveRestAPI-v3";

        public const string DriveServiceNotFoundImage = "1KXVcuCEi-aYgrJXkUwV_RODDh5cT5qHv";

        public const string DriveServiceBaseImageUrl = "https://drive.google.com/thumbnail?id={0}";


    }
}
