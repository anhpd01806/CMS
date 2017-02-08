using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMS.ViewModel
{
    public class ProvinceViewModel
    {
        public List<ProviceModel> ProvinceList { get; set; }
    }

    public class ProviceModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Published { get; set; }
        public int DisplayOrder { get; set; }
        public Boolean Active { get; set; } 
    }
}