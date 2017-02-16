using CMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Bussiness
{
    public class CategorySiteBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();

        public List<CategorySite> GetAllCategorySite()
        {
            return db.CategorySites.Where(x=>x.Deleted == false && x.Published == true).ToList();
        }
    }
}