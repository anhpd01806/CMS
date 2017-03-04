using CMS.Data;
using CMS.ViewModel;
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

        public NewsCustomerActionModel GetCustomerDetail(int userId, DateTime startDate, DateTime endDate)
        {
            //get informtaion of user
            var user = db.Users.FirstOrDefault(x => x.Id == userId);

            var userAction = (from a in db.News_customer_actions
                              where a.CustomerId == userId && startDate.Date <= a.DateCreate.Date
                              && a.DateCreate < endDate.Date.AddDays(1)
                              select a).ToList();
            int sumIscc = (from a in userAction
                           where a.Iscc == true
                           select a).Count();
            int sumIsReport = (from a in userAction
                               where a.IsReport == true
                               select a).Count();
            return new NewsCustomerActionModel
            {
                Id = user.Id,
                UserName = user.UserName,
                SumIscc = sumIscc,
                SumIsReport = sumIsReport,
                StartDate = startDate.ToString("dd/MM/yyyy"),
                EndDate = endDate.ToString("dd/MM/yyyy"),
            };
        }
    }
}