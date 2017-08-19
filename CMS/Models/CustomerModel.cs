using CMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Models
{
    public class CustomerModel
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public int Totalpage { get; set; }
        public double Total { get; set; }
        public List<UserModelApi> ListCustomer { get; set; }
    }
}