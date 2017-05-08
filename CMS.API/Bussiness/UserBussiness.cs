using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.API.Bussiness
{
    public class UserBussiness : InitDB
    {
        public string GetNameById(int id)
        {
            return Instance.Users.FirstOrDefault(x => x.Id == id).UserName;
        }
    }
}