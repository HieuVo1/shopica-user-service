using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using USER_SERVICE_NET.Utilities;

namespace USER_SERVICE_NET.Services.Commons
{
    public static class HttpClientServiceExtensions
    {
        public static async Task<HttpResponseMessage> PostAsJsonAsyncWithAuth<TRequest>(this HttpClient httpClient,string url, TRequest request, IHttpContextAccessor httpContextAccessor)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, Constant.HttpClientMediaType);
            var accessToken = httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await httpClient.PostAsync(url, httpContent);
        }

        public static async Task<HttpResponseMessage> PostAsJsonAsync<TRequest>(this HttpClient httpClient, string url, TRequest request)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, Constant.HttpClientMediaType);
            return await httpClient.PostAsync(url, httpContent);
        }


        public static async Task<HttpResponseMessage> GetAsyncWithAuth(this HttpClient httpClient, string url, IHttpContextAccessor httpContextAccessor)
        {
            var accessToken = httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await httpClient.GetAsync(url);
        }


    }
}
