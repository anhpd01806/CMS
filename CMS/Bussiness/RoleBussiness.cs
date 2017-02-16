using CMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Bussiness
{
    public class RoleBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();
        public List<Role> GetRoles()
        {
            return db.Roles.ToList();
        }

        public void Insert(Role role)
        {
            try
            {
                db.Roles.InsertOnSubmit(role);
                db.SubmitChanges();
            }
            catch (Exception)
            {
            }

        }
        
        public Role GetById(int? id)
        {
            return db.Roles.FirstOrDefault(r => r.Id == id);
        }

        public void Update(Role role)
        {
            try
            {
                var roleUpdate = db.Roles.FirstOrDefault(u => u.Id == role.Id);
                if (roleUpdate != null)
                {
                    roleUpdate.Id = role.Id;
                    roleUpdate.Name = role.Name;
                }
                db.SubmitChanges();
            }
            catch (Exception )
            {

            }

        }

        public void Delete(Role role)
        {
            try
            {
                if (role != null)
                {
                    db.Roles.Attach(role);
                    db.Roles.DeleteOnSubmit(role);
                    db.SubmitChanges();
                }
            }
            catch (Exception )
            {

            }

        }
    }
}