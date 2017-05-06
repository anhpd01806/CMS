using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.API.Data;
using Elmah;

namespace CMS.API.Bussiness
{
    public class AccountBussiness
    {
        public void UpdateLastLogin(int id)
        {
            using (var db = new CmsDataDataContext())
            {
                try
                {
                    var user = db.Users.FirstOrDefault(x => x.Id == id);
                    user.LastLoginDate = DateTime.Now;
                    user.LastActivityDate = DateTime.Now;
                    db.SubmitChanges();
                }
                catch(Exception ex)
                {
                    ErrorLog.GetDefault(HttpContext.Current).Log(new Error(ex));
                    db.Dispose();
                }
            }
        }
    }
}