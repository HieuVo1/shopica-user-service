using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using USER_SERVICE_NET.Services.Commons;
using USER_SERVICE_NET.Utilities;
using USER_SERVICE_NET.ViewModels.Commons;
using USER_SERVICE_NET.ViewModels.Promotions;
using USER_SERVICE_NET.ViewModels.Stores;

namespace USER_SERVICE_NET.Services.Communicates
{
    public class CommunicateService : ICommunicateService
    {
        private readonly HttpClient _httpClient;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public CommunicateService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<APIResult<StoreCreateResponse>> CreateStoreForSeller(StoreRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("store/v2", request);
            if (response.IsSuccessStatusCode)
            {
                using (HttpContent content = response.Content)
                {
                    var data = await content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<APIResult<StoreCreateResponse>>(data);
                }
            }
            else {
                return JsonConvert.DeserializeObject<APIResult<StoreCreateResponse>>(null);
            }

        }

        public async Task<APIResult<List<PromotionResponse>>> GetPromotionValid()
        {
            var response = await _httpClient.GetAsync("promotion/valid-date");

            if (response.IsSuccessStatusCode)
            {
                using (HttpContent content = response.Content)
                {
                    var data = await content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<APIResult<List<PromotionResponse>>>(data);
                }
            }
            else
            {
                return JsonConvert.DeserializeObject<APIResult<List<PromotionResponse>>>(null);
            }
        }

        public async Task<APIResult<List<StoreCreateResponse>>> GetListStore(List<int> storeIds)
        {
            var response = await _httpClient.PostAsJsonAsyncWithAuth("store/list", storeIds, _httpContextAccessor);
            if (response.IsSuccessStatusCode)
            {
                using(HttpContent content = response.Content)
                {
                    string data = await content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<APIResult<List<StoreCreateResponse>>>(data);
                }
            }
            else
            {
                return JsonConvert.DeserializeObject<APIResult<List<StoreCreateResponse>>>(null);
            }
        }

    }
}
