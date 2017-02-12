using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Models
{
    public class SiteModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string XpathList { get; set; }

        public string XpathDetails { get; set; }

        public string Description { get; set; }

        public bool Published { get; set; }

        public bool Deleted { get; set; }

        public int DisplayOrder { get; set; }

    }

}