using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.ViewModel
{
    public class NoticeViewModel
    {
        public List<NoticeDetailModel> NoticeList { get; set; }
    }

    public class NoticeDetailModel
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public DateTime DateSend { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
    }
}