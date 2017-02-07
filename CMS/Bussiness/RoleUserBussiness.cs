using CMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Bussiness
{
    public class RoleUserBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();
        public List<Role_User> GetAllRoleUser()
        {
            return db.Role_Users.ToList();
        }

        public void Insert(Role_User roleUser)
        {
            if (roleUser != null)
            {
                var rs = db.ExecuteCommand(@"INSERT INTO Role_User VALUES (" + roleUser.RoleId + "," + roleUser.UserId + ")");
            }
        }
    }
}