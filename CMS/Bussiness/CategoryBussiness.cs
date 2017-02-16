using CMS.Data;
using CMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Bussiness
{
    public class CategoryBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();

        public List<Category> GetAllCateGory()
        {
            return db.Categories.Where(x => x.Deleted == false).ToList();
        }

        public List<Category> GetParentCateGory()
        {
            return db.Categories.Where(x => x.Deleted == false && x.ParentCategoryId == 0).ToList();
        }

        public void Insert(Category category)
        {
            db.Categories.InsertOnSubmit(category);
            db.SubmitChanges();
        }

        public Category GetCateGoryById(int id)
        {
            return db.Categories.FirstOrDefault(x => x.Deleted == false && x.Id == id);
        }

        public string GetNameCategorySiteById(int id)
        {
            var categorySite = db.CategorySites.FirstOrDefault(x => x.Deleted == false && x.Id == id);
            if (categorySite.ParentId == 0) return categorySite.Name;
            string nameCategory = db.CategorySites.FirstOrDefault(x => x.Id == categorySite.ParentId).Name + " >> " + categorySite.Name;
            return nameCategory;
        }

        public void Update(CategoryModel model)
        {
            var category = db.Categories.FirstOrDefault(x => x.Id == model.Id);
            category.Name = model.Name;
            category.Description = model.Description;
            category.ParentCategoryId = model.CategoryId;
            category.Published = model.Active;
            category.UpdatedOn = DateTime.Now;
            db.SubmitChanges();
        }

        public Boolean Delete(int id)
        {
            var category = db.Categories.FirstOrDefault(x => x.Id == id);
            if (db.Categories.Any(x => x.ParentCategoryId == id)) return false;
            db.Categories.DeleteOnSubmit(category);
            db.SubmitChanges();
            return true;
        }

        public List<CategorySite> GetCategorySiteBySiteId(int siteId)
        {
            return db.CategorySites.Where(x => x.SiteId == siteId).ToList();
        }
    }
}