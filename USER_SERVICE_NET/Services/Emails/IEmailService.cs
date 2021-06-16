using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USER_SERVICE_NET.ViewModels.Commons;
using USER_SERVICE_NET.ViewModels.Emails;

namespace USER_SERVICE_NET.Services.Emails
{
    public interface IEmailService
    {
        Task<APIResult<string>> SendEmailAsync(EmailRequest message);
    }
}
