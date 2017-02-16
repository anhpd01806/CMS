using CMS.Data;
using CMS.Models;
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
            return db.CategorySites.Where(x => x.Deleted == false).ToList();
        }

        public CategorySite GetCategorySiteById(int id)
        {
            return db.CategorySites.FirstOrDefault(x => x.Id == id);
        }

        public void Insert(CategorySite model)
        {
            db.CategorySites.InsertOnSubmit(model);
            db.SubmitChanges();
        }

        public void Update(CMS.ViewModel.CategorySiteModel model)
        {
            var categorySite = db.CategorySites.FirstOrDefault(x => x.Id == model.Id);
            if (categorySite != null)
            {
                categorySite.Name = model.Name;
                categorySite.ParentId = model.ParentId;
                categorySite.SiteId = model.SiteId;
                categorySite.CategoryId = model.CategoryId;
                categorySite.Description = model.Description;
                categorySite.Published = model.Active;
            }
            db.SubmitChanges();
        }

        public void Delete(int id)
        {
            var categorySite = db.CategorySites.FirstOrDefault(x => x.Id == id);
            categorySite.Deleted = true;
            db.SubmitChanges();
        }
    }
}