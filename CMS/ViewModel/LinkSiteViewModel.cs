using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.ViewModel
{
    public class LinkSiteViewModel
    {
        public List<SelectListItem> SiteList { get; set; }
        public int SiteId { get; set; }

        public List<SelectListItem> Category { get; set; }
        public int CategoryId { get; set; }

        public List<SelectListItem> ProvinceList { get; set; }
        public int ProvinceId { get; set; }

        public List<SelectListItem> DistrictList { get; set; }
        public int districtId { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string LinkUrl { get; set; }
        public int Totalpage { get; set; }
        public List<LinkSiteModel> LinkSiteList { get; set; }
    }

    public class LinkSiteModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Site { get; set; }
        public string Category { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string Status { get; set; }
    }
}