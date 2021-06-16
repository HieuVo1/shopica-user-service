using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using USER_SERVICE_NET.Models;

namespace USER_SERVICE_NET.Utilities
{
    public static class Helpers
    {
        public static string CreateToken(Account user,bool isSocial, IConfiguration _configuration)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new[]
           {
                new Claim(ClaimTypes.Role, user.Type.ToString()),
                new Claim("accountId", user.Id.ToString()),
                new Claim("image", user.ImageUrl),
                new Claim("sub", user.Username),
                new Claim("isSocial", isSocial.ToString()),
                new Claim("storeId",  user.Seller.Count != 0 ? user.Seller.FirstOrDefault().StoreId.ToString() : "-1"),
                new Claim("name",  user.Seller.Count != 0 ? user.Seller.FirstOrDefault().SellerName??"" : ""),
            };
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("SecretKey").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string GenerateRandomString()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public static bool VeritifyHashString(string text, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(text, hash);
        }

        public static string Base64Encode(string str)
        {
            var strBytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(strBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
         
        public static string GetCurrentTime()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
        }

        public static string GetStringFromHtml(string rootPath, string fileName)
        {
            var filePath = Path.Combine(rootPath, "EmailTemplate", fileName);
            return File.ReadAllText(filePath);
        }

        public static string ConverToSlug(string value)
        {
            value = value.ToLowerInvariant();

            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);
            value = Encoding.ASCII.GetString(bytes);

            value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);

            value = Regex.Replace(value, @"[^a-z0-9\s-_]", "", RegexOptions.Compiled);

            value = value.Trim('-', '_');

            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return value;
        }
    }
}
