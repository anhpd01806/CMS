using CMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.Bussiness
{
    public class PaymentBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();

        public List<SelectListItem> GetPaymentMethod()
        {
            var paymentMethod = (from u in db.PaymentMethods
                                 where u.Active == true
                                 select new SelectListItem
                                 {
                                     Text = u.Name,
                                     Value = u.Id.ToString()
                                 }).ToList();
            return paymentMethod;
        }

        public void Insert(PaymentHistory model)
        {
            db.PaymentHistories.InsertOnSubmit(model);
            db.SubmitChanges();
        }

        public List<PaymentHistory> GetPaymentHistoryByUserId(int userId)
        {
            return db.PaymentHistories.Where(x => x.UserId == userId && x.CreatedDate <= DateTime.Now && x.CreatedDate >= DateTime.Now.AddDays(-30)).OrderByDescending(x=>x.CreatedDate).ToList();
        }

        public string GetPaymentMethodById(int id)
        {
            return db.PaymentMethods.FirstOrDefault(x => x.Id == id).Name;
        }
    }
}