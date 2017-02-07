using CMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Bussiness
{
    public class UserBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();

        public List<User> GetAllUser()
        {
            return db.Users.OrderBy(m => m.Id).ToList();
        }

    }
}