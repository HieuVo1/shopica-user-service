using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USER_SERVICE_NET.ViewModels.Commons;
using USER_SERVICE_NET.ViewModels.Promotions;
using USER_SERVICE_NET.ViewModels.Stores;

namespace USER_SERVICE_NET.Services.Communicates
{
    public interface ICommunicateService
    {
        Task<APIResult<StoreCreateResponse>> CreateStoreForSeller(StoreRequest request);
        Task<APIResult<List<PromotionResponse>>> GetPromotionValid();
        Task<APIResult<List<StoreCreateResponse>>> GetListStore(List<int> storeIds);
    }
}
