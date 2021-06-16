using System;
using System.Collections.Generic;
using USER_SERVICE_NET.Utilities.Enums;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace USER_SERVICE_NET.Models
{
    public partial class Customer
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public Genders Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int AccountId { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Updated_at { get; set; }

        public virtual Account Account { get; set; }
    }
}
