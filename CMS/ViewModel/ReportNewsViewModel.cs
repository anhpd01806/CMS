using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.ViewModel
{
    public class ReportNewsViewModel
    {
        public List<NewsReportModel> NewsReportList { get; set; }
        public NewsReportModel FirstRandomNewsReport { get; set; }
    }

    public class NewsReportModel
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
        public int NewsId { get; set; }
        public int CustomerId { get; set; }
        public string CreateDate { get; set; }
        public string Notes { get; set; }
        public string Users { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
    }
}