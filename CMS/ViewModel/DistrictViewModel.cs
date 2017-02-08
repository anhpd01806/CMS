using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.ViewModel
{
    public class DistrictViewModel
    {
        public List<DistrictModel> DistrictList { get; set; }
    }

    public class DistrictModel
    {
        public int Id { get; set; }
        public string Province { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Publish { get; set; }
        public int DisplayOrder { get; set; }
        public List<SelectListItem> SelectListItem { get; set; }
        public Boolean Active { get; set; }
        public int ProvinceId { get; set; }
    }
}