using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USER_SERVICE_NET.Utilities.Enums;
using USER_SERVICE_NET.ViewModels.Address;

namespace USER_SERVICE_NET.ViewModels.Sellers
{
    public class SellerView
    {
        public string SellerName { get; set; }
        public AddressInfo Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public Genders Gender { get; set; }
        public string StoreName { get; set; }
        public string Website { get; set; }
        public DateTime? Created_at { get; set; }
    }
}
