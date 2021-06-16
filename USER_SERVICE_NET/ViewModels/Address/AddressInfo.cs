using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace USER_SERVICE_NET.ViewModels.Address
{
    public class AddressInfo
    {
        public string AddressName { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public string WardId { get; set; }
    }
}
