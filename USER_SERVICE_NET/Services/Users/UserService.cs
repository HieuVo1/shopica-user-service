using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USER_SERVICE_NET.Models;
using USER_SERVICE_NET.Services.Communicates;
using USER_SERVICE_NET.Services.Emails;
using USER_SERVICE_NET.Services.StorageServices;
using USER_SERVICE_NET.Utilities;
using USER_SERVICE_NET.Utilities.Enums;
using USER_SERVICE_NET.ViewModels.Accounts;
using USER_SERVICE_NET.ViewModels.Address;
using USER_SERVICE_NET.ViewModels.Commons;
using USER_SERVICE_NET.ViewModels.Commons.Pagging;
using USER_SERVICE_NET.ViewModels.Customers;
using USER_SERVICE_NET.ViewModels.Emails;
using USER_SERVICE_NET.ViewModels.Sellers;
using USER_SERVICE_NET.ViewModels.Stores;

namespace USER_SERVICE_NET.Services.Users
{
    public class UserService : IUserService
    {
        private readonly ShopicaContext _context;
        private readonly IConfiguration _configuration;
        private readonly ICommunicateService _communicateService;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserService(
            ShopicaContext context,
            IConfiguration configuration,
            ICommunicateService communicateService,
            IEmailService emailService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _communicateService = communicateService;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<APIResult<string>> Authencate(LoginRequest request)
        {
            var user = await _context.Account.Include(x => x.Seller).FirstOrDefaultAsync(u => u.Username == request.Email);

            if (user == null) return new APIResultErrors<string>("Email not found");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return new APIResultErrors<string>("UserName or Password is incorrect");
            }

            var token = Helpers.CreateToken(user, false, _configuration);
            return new APIResultSuccess<string>(token);
        }

        public async Task<APIResult<string>> RegisterForCustomer(CustomerRegisterRequest request)
        {
            var account = new Account()
            {
                Username = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsActive = 1,
                Type = AccountTypes.Customer,
                ImageUrl = request.ImageUrl,
                Created_at = DateTime.Now,
                Customer = new List<Customer>()
                {
                    new Customer()
                    {
                        CustomerName = request.Fullname,
                        Address = JsonConvert.SerializeObject(request.Address),
                        Phone = request.Phone,
                        Email = request.Email,
                        Gender = request.Gender,
                        Created_at = DateTime.Now,
                    }
                }
            };

            _context.Account.Add(account);

            await _context.SaveChangesAsync();

            return new APIResultSuccess<string>("register successfully");

        }

        public async Task<APIResult<string>> RegisterForSeller(SellerRegisterRequest request)
        {
            var storeRequest = new StoreRequest();
            storeRequest.Owner = request.Fullname;
            storeRequest.StoreName = request.StoreName;
            storeRequest.OpenTime = request.OpenTime;
            storeRequest.CloseTime = request.CloseTime;
            storeRequest.Website = request.Website;

            var store = await _communicateService.CreateStoreForSeller(storeRequest);

            var account = new Account()
            {
                Username = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsActive = 1,
                Type = AccountTypes.Seller,
                Created_at = DateTime.Now,
                Seller = new List<Seller>()
                {
                    new Seller()
                    {
                        SellerName = request.Fullname,
                        Phone = request.Phone,
                        Email = request.Email,
                        Gender = request.Gender,
                        StoreId = store.Data.Id,
                        Created_at = DateTime.Now,
                    }
                }
            };

            _context.Account.Add(account);
            await _context.SaveChangesAsync();

            return new APIResultSuccess<string>("register successfully");
        }

        public async Task<APIResult<string>> ChangePassword(ChangePasswordRequest request)
        {
            var user = await _context.Account.FindAsync(request.AccountId);
            if (user == null)
            {
                return new APIResultErrors<string>("Can not found user");
            }

            if(request.LoginMethod == "NORMAL" && !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password))
            {
                return new APIResultErrors<string>("Current password is incorrect");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.Updated_at = DateTime.Now;

            await _context.SaveChangesAsync();

            return new APIResultSuccess<string>("Change password successfully");

        }

        public async Task<APIResult<string>> GenerateTokenResetPassword(string email)
        {
            var user = await _context.Account.FirstOrDefaultAsync(ac => ac.Username == email);
            if (user == null)
            {
                return new APIResultErrors<string>("Can not found user with this email!");
            }

            var token = Helpers.Base64Encode(Helpers.GetCurrentTime());

            user.TokenResetPassword = token;
            user.Updated_at = DateTime.Now;

            await _context.SaveChangesAsync();


            var redirectUrl = String.Format("{0}/account/reset?token={1}", Constant.ShopicaUrl,token);

            string contentTemplate = Helpers.GetStringFromHtml(_webHostEnvironment.WebRootPath, "ResetPassword.html");

            var emailRequest = new EmailRequest()
            {
                To = email,
                Subject = "Reset Password",
                Content = String.Format(contentTemplate, redirectUrl)
            };

            _emailService.SendEmailAsync(emailRequest);

            return new APIResultSuccess<string>("Send verify code successfully!");
        }

        public async Task<APIResult<bool>> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _context.Account.FirstOrDefaultAsync(ac => ac.Username == request.Email);
            if (user == null)
            {
                return new APIResultErrors<bool>("Can not found user with this email!");
            }
            var tokenGenerateTime = Convert.ToInt64(Helpers.Base64Decode(request.DatabaseToken));
            var currentTime = Convert.ToInt64(Helpers.GetCurrentTime());
            if (currentTime - tokenGenerateTime > Constant.TokenExpireTime) // 10 minutes
            {
                return new APIResultErrors<bool>("Token is expired!");
            }

            if (String.Compare(user.TokenResetPassword, request.DatabaseToken) == 0)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.TokenResetPassword = String.Empty;
                user.Updated_at = DateTime.Now;

                await _context.SaveChangesAsync();
                return new APIResultSuccess<bool>();
            }

            return new APIResultErrors<bool>("Token is invalid!");

        }

        public async Task<APIResult<string>> SocialLogin(SocialLoginRequest request)
        {
            string result;
            var user = await _context.Account.FirstOrDefaultAsync(
                    ac => ac.Username == request.Email ||
                    (ac.ProviderKey == request.ProviderKey && ac.Provider == request.Provider)
                );

            if (user == null)
            {
                var account = new Account()
                {
                    Username = request.Email,
                    Provider = request.Provider,
                    ProviderKey = request.ProviderKey,
                    ImageUrl = request.ImageUrl,
                    IsActive = 1,
                    Type = AccountTypes.Customer,
                    Created_at = DateTime.Now,
                    Customer = new List<Customer>()
                {
                    new Customer()
                    {
                        CustomerName = request.FullName,
                        Email = request.Email,
                        Created_at = DateTime.Now,
                    }
                }
                };

                _context.Account.Add(account);
                await _context.SaveChangesAsync();

                result = Helpers.CreateToken(account, true, _configuration);
                return new APIResultSuccess<string>(result);
            }

            else if (user.ProviderKey == null)
            {
                return new APIResultErrors<string>("Email has already been taken. Please reset your password with this email!");
            }

            result = Helpers.CreateToken(user, true, _configuration);
            return new APIResultSuccess<string>(result);

        }

        public async Task<APIResult<bool>> UpdateInfoForSeller(SellerUpdateRequest request)
        {
            var seller = await _context.Seller.Include(x => x.Account).FirstOrDefaultAsync(c => c.AccountId == request.AccountId);

            if (seller == null)
            {
                return new APIResultErrors<bool>("Can not found seller");
            }

            var account = seller.Account;
            account.Username = (request.Email != null && account.Username != request.Email) ? request.Email : account.Username;
            account.ImageUrl = (request.Image != null && account.ImageUrl != request.Image) ? request.Image : account.ImageUrl;
            seller.SellerName = (!String.IsNullOrEmpty(request.Sellername) && seller.SellerName != request.Sellername) ? request.Sellername : seller.SellerName;
            seller.Gender = (!String.IsNullOrEmpty(request.Gender.ToString()) && seller.Gender != request.Gender) ? request.Gender : seller.Gender;
            seller.Phone = (!String.IsNullOrEmpty(request.Phone) && seller.Phone != request.Phone) ? request.Phone : seller.Phone;
            seller.Address = (request.Address != null) ? JsonConvert.SerializeObject(request.Address) : seller.Address;
            seller.Updated_at = DateTime.Now;
            await _context.SaveChangesAsync();

            return new APIResultSuccess<bool>();

        }

        public async Task<APIResult<bool>> UpdateInfoForCustomer(CustomerUpdateRequest request)
        {
            var customer = await _context.Customer.Include(x => x.Account).FirstOrDefaultAsync(c => c.AccountId == request.AccountId);

            if (customer == null)
            {
                return new APIResultErrors<bool>("Can not found customer");
            }

            var account = customer.Account;
            account.Username = (request.Email != null && account.Username != request.Email) ? request.Email : account.Username;
            account.ImageUrl = (request.Image != null && account.ImageUrl != request.Image) ? request.Image : account.ImageUrl;
            customer.CustomerName = (!String.IsNullOrEmpty(request.CustomerName) && customer.CustomerName != request.CustomerName) ? request.CustomerName : customer.CustomerName;
            customer.Gender = (!String.IsNullOrEmpty(request.Gender.ToString()) && customer.Gender != request.Gender) ? request.Gender : customer.Gender;
            customer.Phone = (!String.IsNullOrEmpty(request.Phone) && customer.Phone != request.Phone) ? request.Phone : customer.Phone;
            customer.Address = (request.Address != null) ? JsonConvert.SerializeObject(request.Address) : customer.Address;
            customer.Updated_at = DateTime.Now;
            await _context.SaveChangesAsync();

            return new APIResultSuccess<bool>();
        }


        public async Task<APIResult<PaggingView<CustomerView>>> GetAllCustomer(PaggingRequest request)
        {
            var listCustomer = await _context.Customer
                .Include(c => c.Account)
                .ToListAsync();

            var totalRow = listCustomer.Count();

            var data = listCustomer
                .Skip(request.pageSize * (request.pageIndex - 1))
                .Take(request.pageSize)
                .Select(x => new CustomerView()
                {
                    CustomerName = x.CustomerName,
                    Address = x.Address != null ? JsonConvert.DeserializeObject<AddressInfo>(x.Address) : null,
                    Phone = x.Phone,
                    Email = x.Email,
                    Gender = x.Gender,
                    Image = x.Account.ImageUrl,
                    Created_at = x.Created_at
                }).ToList();

            var customerView = new PaggingView<CustomerView>()
            {
                TotalRecord = totalRow,
                Datas = data,
                Pageindex = request.pageIndex,
                PageSize = request.pageSize
            };

            return new APIResultSuccess<PaggingView<CustomerView>>(customerView);
        }

        public async Task<APIResult<PaggingView<SellerView>>> GetAllSeller(PaggingRequest request)
        {

            var listSeller = await _context.Seller
                .Include(c => c.Account)
                .ToListAsync();

            var totalRow = listSeller.Count();

            var listStoreId = listSeller.Select(x => x.StoreId).ToList();

            var listStore = await _communicateService.GetListStore(listStoreId);

            var fullList = from s in listSeller
                           join st in listStore.Data on s.StoreId equals st.Id
                           select new { s, st };

            var data = fullList
                .Skip(request.pageSize * (request.pageIndex - 1))
                .Take(request.pageSize)
                .Select(x => new SellerView()
            {
                SellerName = x.s.SellerName,
                Address = x.s.Address!=null? JsonConvert.DeserializeObject<AddressInfo>(x.s.Address):null,
                Phone = x.s.Phone,
                Email = x.s.Email,
                Gender = x.s.Gender,
                Image = x.s.Account.ImageUrl,
                StoreName = x.st.StoreName,
                Website = x.st.Website,
                Created_at = x.s.Created_at
            }).ToList();

            var sellerView = new PaggingView<SellerView>()
            {
                TotalRecord = totalRow,
                Datas = data,
                Pageindex = request.pageIndex,
                PageSize = request.pageSize
            };

            return new APIResultSuccess<PaggingView<SellerView>>(sellerView);
        }

        public async Task<APIResult<AccountView>> GetAccountInfoByUserName(string userName)
        {
            var account = await _context.Account.Include(x => x.Seller).FirstOrDefaultAsync(a => a.Username == userName);
            if(account == null)
            {
                return new APIResultErrors<AccountView>("Not found");
            }
            var accountView = new AccountView()
            {
                Id = account.Id,
                UserName = account.Username,
                Password = account.Password,
                Type = account.Type,
                ImageUrl = account.ImageUrl,
                IsActive = account.IsActive,
                StoreId = account.Seller.Count > 0 ? account.Seller.FirstOrDefault().StoreId : -1,
            };

            return new APIResultSuccess<AccountView>(accountView);
        }

        public async Task<APIResult<string>> CheckEmailExist(string email)
        {
            var account = await _context.Account.FirstOrDefaultAsync(x => x.Username == email);

            if(account != null)
            {
                return new APIResultErrors<string>("Email is already in use");
            }

            return new APIResultSuccess<string>();
        }

        public async Task<APIResult<SellerView>> GetSellerById(int accountId)
        {
            var seller = await _context.Seller.Include(x => x.Account).FirstOrDefaultAsync(a => a.AccountId == accountId);
            if (seller == null)
            {
                return new APIResultErrors<SellerView>("Not found");
            }
            var data = new SellerView()
            {
                SellerName = seller.SellerName,
                Address = seller.Address != null ? JsonConvert.DeserializeObject<AddressInfo>(seller.Address) : null,
                Phone = seller.Phone,
                Email = seller.Email,
                Gender = seller.Gender,
                Image = seller.Account.ImageUrl,
            };

            return new APIResultSuccess<SellerView>(data);
        }

        public async Task<APIResult<CustomerView>> GetCustomerById(int accountId)
        {
            var customer = await _context.Customer.Include(x => x.Account).FirstOrDefaultAsync(a => a.AccountId == accountId);
            if (customer == null)
            {
                return new APIResultErrors<CustomerView>("Not found");
            }
            var data = new CustomerView()
            {
                CustomerName = customer.CustomerName,
                Address = customer.Address != null ? JsonConvert.DeserializeObject<AddressInfo>(customer.Address) : null,
                Phone = customer.Phone,
                Email = customer.Email,
                Gender = customer.Gender,
                Image = customer.Account.ImageUrl,
            };

            return new APIResultSuccess<CustomerView>(data);
        }
    }
}
