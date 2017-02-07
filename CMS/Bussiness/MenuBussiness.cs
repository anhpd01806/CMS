using CMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Bussiness
{
    public class MenuBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();
        public List<Menus> GetAllMenus()
        {
            return db.Menus.OrderBy(m => m.Orders).ToList();
        }

        public List<Menus> GetLstMenuParent()
        {
            return db.Menus.Where(x => !db.Operations.Select(op => op.MenuId).Contains(x.Id)).OrderBy(m => m.Orders).ToList();
        }

        public void Insert(Menus menu)
        {
            db.Menus.InsertOnSubmit(menu);
            db.SubmitChanges();
        }

        public List<Menus> GetSelectChildMenus()
        {
            return db.Menus.Where(x => !db.Menus.Select(m => m.ParentId).Contains(x.Id)).ToList();
        }

        public Menus FindById(int? id)
        {
            var u = (from b in db.Menus
                     where b.Id == id
                     select b).FirstOrDefault();

            return u;
        }

        public void Update(Menus menu)
        {
            var menuUpdate = db.Menus.FirstOrDefault(u => u.Id == menu.Id);
            if (menuUpdate != null)
            {
                menuUpdate.Name = menu.Name;
                menuUpdate.ParentId = menu.ParentId;
                menuUpdate.Orders = menu.Orders;
                menuUpdate.CssClass = menu.CssClass;
                menuUpdate.IsActive = menu.IsActive;
            }
            db.SubmitChanges();
        }

        public List<Menus> GetChildMenus()
        {
            return db.Menus.Where(m => db.Operations.Select(x => x.MenuId).Contains(m.Id)).OrderBy(m => m.Orders).ToList();
        }

        public Menus Delete(int? id)
        {
            var itemToRemove = db.Menus.SingleOrDefault(x => x.Id == id);

            if (itemToRemove != null)
            {
                //db.Menus.Attach(itemToRemove);
                db.Menus.DeleteOnSubmit(itemToRemove);
                db.SubmitChanges();
            }
            return itemToRemove;
        }
    }
}