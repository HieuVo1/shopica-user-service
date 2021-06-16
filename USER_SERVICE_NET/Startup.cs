using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USER_SERVICE_NET.Models;
using USER_SERVICE_NET.Services.Commons;
using USER_SERVICE_NET.Services.Communicates;
using USER_SERVICE_NET.Services.CronJob;
using USER_SERVICE_NET.Services.Emails;
using USER_SERVICE_NET.Services.StorageServices;
using USER_SERVICE_NET.Services.Users;
using USER_SERVICE_NET.Utilities;
using USER_SERVICE_NET.ViewModels.Emails;

namespace USER_SERVICE_NET
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                );
            });

            services.AddControllers();
            services.AddDbContext<ShopicaContext>(options =>
                    options.UseMySQL(Configuration.GetConnectionString(Constant.ConnectionString)));
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IStorageService, StorageService>();
            services.AddHttpContextAccessor();

            var emailConfig = Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();

            services.AddSingleton(emailConfig);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options =>
           {
               options.RequireHttpsMetadata = false;
               options.SaveToken = true;
               options.TokenValidationParameters = new TokenValidationParameters()
               {
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ClockSkew = TimeSpan.Zero,
                   IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Configuration.GetSection("SecretKey").Value))
               };
           });

            services.AddCronJob<SendPromotionService>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"0 6 * * *"; // 6am every day
            });

            services.AddHttpClient<ICommunicateService, CommunicateService>(client =>
            {
                client.BaseAddress = new Uri(Configuration.GetSection("ProductServiceUrl").Value);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
