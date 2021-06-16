using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USER_SERVICE_NET.Utilities.Enums;

namespace USER_SERVICE_NET.ViewModels.Accounts
{
    public class AccountView
    {
        public int Id{ get; set; }
        public string UserName { get; set; }
        public string ImageUrl { get; set; }
        public string Password { get; set; }
        public byte IsActive { get; set; }
        public int StoreId { get; set; }
        public DateTime? Created_at { get; set; }
        public AccountTypes Type { get; set; }
    }
}
