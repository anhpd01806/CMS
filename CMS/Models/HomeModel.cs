using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.Models;
using CMS.Data;

namespace CMS.Models
{
    public class HomeModel
    {
        public HomeModel()
        {
            ListNew = new List<NewsModel>();
        }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public int Totalpage { get; set; }
        public int Total { get; set; }
        public List<NewsModel> ListNew { get; set; }
    }
}