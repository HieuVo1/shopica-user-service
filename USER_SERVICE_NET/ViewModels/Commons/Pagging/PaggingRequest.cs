using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace USER_SERVICE_NET.ViewModels.Commons.Pagging
{
    public class PaggingRequest
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }
}
