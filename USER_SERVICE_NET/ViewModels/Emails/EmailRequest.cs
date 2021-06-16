using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace USER_SERVICE_NET.ViewModels.Emails
{
    public class EmailRequest
    {
        public string To { get; set; }
        public List<string> Recipients { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string FullName { get; set; }
    }
}
