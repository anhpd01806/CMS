using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Models
{
    public class DistrictModel
    {
        public int Id { get; set; }

        public int ProvinceId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public bool Published { get; set; }

        public int DisplayOrder { get; set; }

    }
}