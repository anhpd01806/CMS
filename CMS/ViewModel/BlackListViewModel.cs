using CMS.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMS.ViewModel
{
    public class BlackListViewModel
    {
        public List<BlacklistModel> BlackList { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public string LinkUrl { get; set; }
        public int Totalpage { get; set; }
    }

    public class BlacklistModel
    {
        public long Id { get; set; }
        public string Words { get; set; }
        public string Description { get; set; }
        public string LinkUrl { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Type { get; set; }
    }
}