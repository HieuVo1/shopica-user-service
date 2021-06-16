using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USER_SERVICE_NET.Utilities.Enums;
using USER_SERVICE_NET.ViewModels.Address;

namespace USER_SERVICE_NET.ViewModels.Sellers
{
    public class SellerUpdateRequest
    {
        public int AccountId { get; set; }
        public string Sellername { get; set; }
        public AddressInfo Address { get; set; }
        public Genders Gender { get; set; }
        public string Image { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
