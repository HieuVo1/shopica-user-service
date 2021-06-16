using System;
using System.Collections.Generic;
using USER_SERVICE_NET.Utilities.Enums;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace USER_SERVICE_NET.Models
{
    public partial class Account
    {
        public Account()
        {
            Customer = new HashSet<Customer>();
            Seller = new HashSet<Seller>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public AccountTypes Type { get; set; }
        public string ImageUrl { get; set; }
        public byte IsActive { get; set; }
        public string Provider { get; set; }
        public string ProviderKey { get; set; }
        public string TokenResetPassword { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Updated_at { get; set; }

        public virtual ICollection<Customer> Customer { get; set; }
        public virtual ICollection<Seller> Seller { get; set; }
    }
}
