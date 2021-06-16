using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using USER_SERVICE_NET.Utilities.Enums;

namespace USER_SERVICE_NET.ViewModels.Stores
{
    public class StoreRequest
    {
        [Required]
        public string StoreName { get; set; }
        [Required]
        public string Owner { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
    }
}
