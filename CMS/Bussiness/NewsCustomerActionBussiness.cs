using CMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Bussiness
{
    public class NewsCustomerActionBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();

        public void InsertActionCustomer(News_customer_action model)
        {
            db.News_customer_actions.InsertOnSubmit(model);
            db.SubmitChanges();
        }
    }
}