using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace USER_SERVICE_NET.ViewModels.Accounts
{
    public class ChangePasswordRequest
    {
        public int AccountId { get; set; }

        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password and ConfirmPassword is not match")]
        public string ConfirmPassword { get; set; }

        public string LoginMethod { get; set; }
    }
}
