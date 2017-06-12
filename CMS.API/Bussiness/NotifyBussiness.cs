using CMS.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Bussiness
{
    public class NotifyBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();
        public List<Notify> GetAllNotice(bool IsUser, int UserId, int page)
        {
            if (IsUser)
            {
                return (from c in db.Notifies
                        where c.SendFlag == false && c.SendTo.Equals(UserId)
                        orderby c.ViewFlag ascending, c.DateSend descending
                        select c).Skip(page * 20).Take(20).ToList();
            }
            else
            {
                //if user is admin
                return (from c in db.Notifies
                        where c.SendFlag == true
                        orderby c.ViewFlag ascending, c.DateSend descending
                        select c).Skip(page * 20).Take(20).ToList();
            }

        }
    }
}