using CMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.ViewModel
{
    public class CategoryViewModel
    {
        public List<CategoryModel> CategoryList { get; set; }
       
    }

    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ParentName { get; set; }
        public string Published { get; set; }
        public int CategoryId { get; set; }
        public List<SelectListItem> SelectListItem { get; set; }
        public Boolean Active { get; set; }
    }
}