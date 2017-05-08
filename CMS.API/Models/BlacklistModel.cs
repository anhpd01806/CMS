using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Models
{
    public class BlacklistModel
    {
        public int Id { get; set; }

        public string Words { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public int Type { get; set; }
    }

}