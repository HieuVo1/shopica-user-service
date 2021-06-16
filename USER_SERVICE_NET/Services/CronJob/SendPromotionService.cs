using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using USER_SERVICE_NET.Models;
using USER_SERVICE_NET.Services.Communicates;
using USER_SERVICE_NET.Services.Emails;
using USER_SERVICE_NET.Utilities;
using USER_SERVICE_NET.ViewModels.CronJob;
using USER_SERVICE_NET.ViewModels.Emails;
using USER_SERVICE_NET.ViewModels.Promotions;

namespace USER_SERVICE_NET.Services.CronJob
{

    public class SendPromotionService : CronJobService
    {
        private readonly IServiceProvider _services;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SendPromotionService(IScheduleConfig<SendPromotionService> config, IServiceProvider services, IWebHostEnvironment webHostEnvironment)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _services = services;
            _webHostEnvironment = webHostEnvironment;
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            using (var scope = _services.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<ShopicaContext>();
                var _emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var _communicateService = scope.ServiceProvider.GetRequiredService<ICommunicateService>();

                var listCutomerMail = await _context.Customer.Select(x => x.Email).ToListAsync();
                var listPromotionValid = await _communicateService.GetPromotionValid();

                if(listPromotionValid.Data.Count > 0)
                {
                    //var emailRequest = GetMailRequest(listCutomerMail, listPromotionValid.Data, _webHostEnvironment.WebRootPath);

                    //await _emailService.SendEmailAsync(emailRequest);
                }
            }
        }

        public static EmailRequest GetMailRequest(List<string> listEmailCustomer, List<PromotionResponse> listPromotion, string rootPath)
        {
            string tableBody = "";
            string template = Helpers.GetStringFromHtml(rootPath, "Promotion.html");
            string tableTh = Helpers.GetStringFromHtml(rootPath, "TableTh.html");

            foreach (var item in listPromotion)
            {
                tableBody += String.Format(tableTh, item.QrCode);
            }

            var emailRequest = new EmailRequest();
            emailRequest.Subject = "[SHOPICA] PROMOTION DAYLI";
            emailRequest.Recipients = listEmailCustomer;
            emailRequest.Content = String.Format(template, tableBody); ;

            return emailRequest;
        }
    }
}
