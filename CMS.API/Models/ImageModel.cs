using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Models
{
    public class ImageModel
    {
        public int Id { get; set; }

        public int? NewsId { get; set; }

        public string ImageUrl { get; set; }

    }

}