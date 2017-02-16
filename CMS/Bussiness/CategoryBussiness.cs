using CMS.Data;
using CMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.Bussiness
{
    public class CategoryBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();

        public List<Category> GetAllCateGory()
        {
            return db.Categories.Where(x => x.Deleted == false && x.Published == true).ToList();
        }

        public List<SelectListItem> GetCategoryDrop()
        {
            var rs = new List<SelectListItem>();
            var category = db.Categories.Where(x => x.Deleted == false && x.Published == true).ToList();
            if (category != null)
            {
                foreach(var item in category)
                {
                    if (item.ParentCategoryId == 0) rs.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                    else rs.Add(new SelectListItem {Text = category.FirstOrDefault(x=>x.Id == item.ParentCategoryId).Name + " >> " + item.Name, Value = item.Id.ToString() });
                }
            }
            return rs;
        }

        public List<Category> GetParentCateGory()
        {
            return db.Categories.Where(x => x.Deleted == false && x.Published == true && x.ParentCategoryId == 0).ToList();
        }

        public void Insert(Category category)
        {
            db.Categories.InsertOnSubmit(category);
            db.SubmitChanges();
        }

        public Category GetCateGoryById(int id)
        {
            return db.Categories.FirstOrDefault(x => x.Deleted == false && x.Published == true && x.Id == id);
        }

        public string GetNameCategoryById(int id)
        {
            var category = db.Categories.FirstOrDefault(x => x.Deleted == false && x.Published == true && x.Id == id);
            if (category != null)
            {
                if (category.ParentCategoryId == 0) return category.Name;
                string nameCategory = db.Categories.FirstOrDefault(x => x.Id == category.ParentCategoryId && x.Deleted == false).Name + " >> " + category.Name;
                return nameCategory;
            }
            return "";
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
            category.Deleted = true;
            db.SubmitChanges();
            return true;
        }

        public List<CategorySite> GetCategorySiteBySiteId(int siteId)
        {
            return db.CategorySites.Where(x => x.SiteId == siteId && x.Deleted == false && x.Published == true).ToList();
        }
    }
}