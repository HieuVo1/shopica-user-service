using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace USER_SERVICE_NET.ViewModels.Accounts
{
    public class SocialLoginRequest
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Provider { get; set; }
        public string ProviderKey { get; set; }
        public string  ImageUrl { get; set; }
    }
}
