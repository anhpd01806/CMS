using CMS.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Bussiness
{
    public class RoleBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();
        public List<Role> GetRoles()
        {
            return db.Roles.ToList();
        }

        public List<Role_User> GetAllRoleUser()
        {
            return db.Role_Users.ToList();
        }
    }
}