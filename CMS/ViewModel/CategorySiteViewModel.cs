using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.ViewModel
{
    public class CategorySiteViewModel
    {
        public List<CategorySiteModel> CategorySiteList { get; set; }
    }

    public class CategorySiteModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string ParentName { get; set; }
        public string SiteName { get; set; }
        public string CategoryName { get; set; }
        public string Status { get; set; }

        public List<SelectListItem> CategorySiteParentSiteList { get; set; }
        public int ParentId { get; set; }

        public List<SelectListItem> SiteList { get; set; }
        public int SiteId { get; set; }

        public List<SelectListItem> CategoryList { get; set; }
        public int CategoryId { get; set; }

        public Boolean Active { get; set; }
    }
}