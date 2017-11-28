using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.Models;
using CMS.Data;

namespace CMS.ViewModel
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            ListNew = new List<NewsModel>();
            NewsItem = new New();
        }
        public int CategoryId { get; set; }
        public SelectList ListCategory { get; set; }
        public int ProvinId { get; set; }
        public SelectList LstProvince { get; set; }
        public int DistricId { get; set; }
        public SelectList ListDistric { get; set; }
        public int NewsType { get; set; }
        public int SiteId { get; set; }
        public SelectList ListSite { get; set; }
        public int BackDate { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public int StatusId { get; set; }
        public SelectList ListStatus { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public int Totalpage { get; set; }
        public int Total { get; set; }
        public int RoleId { get; set; }
        public List<NewsModel> ListNew { get; set; }
        public New NewsItem { get; set; }
        public int GovermentID { get; set; }
        public SelectList Goverment { get; set; }
    }
}