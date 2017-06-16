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

        public List<Role> GetRolesByAdmin(int userId)
        {
            if (userId == 1)
                return db.Roles.ToList();
            else return db.Roles.Where(x => x.Id != 1).ToList();
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
        public List<Role_User> GetListByUserId(int userId)
        {
            return db.Role_Users.Where(r => r.UserId == userId).ToList();
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
            catch (Exception)
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
            catch (Exception)
            {

            }

        }

        public void DeleteRoleUserByUserId(int UserId)
        {
            var allrole = db.Role_Users.Where(x => x.UserId == UserId).ToList();
            if (allrole != null)
            {
                foreach (var item in allrole)
                {
                    var rs = db.ExecuteCommand(@"DELETE FROM Role_User WHERE RoleId =" + item.RoleId + " and UserId=" + item.UserId + ";");
                }
            }
        }
        
        public List<Role_User> GetAllRoleUser()
        {
            return db.Role_Users.ToList();
        }
    }
}