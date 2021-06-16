using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using USER_SERVICE_NET.Utilities.Enums;
using USER_SERVICE_NET.ViewModels.Address;

namespace USER_SERVICE_NET.ViewModels.Accounts
{
    public class UpdateInfoRequest
    {
        public int AccountId { get; set; }
        public string Fullname { get; set; }
        public AddressInfo Address { get; set; }
        public Genders Gender { get; set; }
        public string ImageUrl { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
