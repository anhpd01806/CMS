using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public int ParentCategoryId { get; set; }        
    }
}