using CMS.Data;
using CMS.Helper;
using CMS.ViewModel;
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

        public void PaymentAcceptedUpdate(PaymentViewModel model, int userId)
        {
            var paymentAccepted = db.PaymentAccepteds.FirstOrDefault(x => x.UserId == userId);
            if (paymentAccepted == null)
            {
                var payment = new PaymentAccepted
                {
                    UserId = userId,
                    AmountTotal = long.Parse(string.Join("", model.Amount.Split(','))),
                    StartDate = DateTime.Now.AddDays(-1),
                    EndDate = DateTime.Now.AddDays(-1)
                };

                //insert to table payment accepted
                db.PaymentAccepteds.InsertOnSubmit(payment);
            }
            else
            {
                paymentAccepted.AmountTotal = paymentAccepted.AmountTotal + long.Parse(string.Join("", model.Amount.Split(',')));
            }
            db.SubmitChanges();
        }

        public List<PaymentHistory> GetPaymentHistoryByUserId(int userId, int page)
        {
            if (page == 0)
                return db.PaymentHistories.Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedDate).Take(20).ToList();
            else
                return db.PaymentHistories.Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedDate).Skip(page * 20).Take(20).ToList();
        }

        public string GetPaymentMethodById(int id)
        {
            return db.PaymentMethods.FirstOrDefault(x => x.Id == id).Name;
        }

        public string UpdatePaymentAccepted(PaymentViewModel model, int userId)
        {
            var paymentAccepted = db.PaymentAccepteds.FirstOrDefault(x => x.UserId == userId);
            if (paymentAccepted == null)
                return "Bạn chưa nạp tiền. Vui lòng liên hệ admin để nạp tiền";
            else
            {
                if (paymentAccepted.AmountTotal < model.Payment) return "Tài khoản của quý khách không đủ tiền";
                else
                {
                    paymentAccepted.AmountTotal = paymentAccepted.AmountTotal - model.Payment;
                    paymentAccepted.StartDate = DateTime.Now;
                    //th tk đã hết hạn sử dụng
                    if (DateTime.Now.Date > paymentAccepted.EndDate.Date)
                        paymentAccepted.EndDate = model.Payment.ToString() == ConfigWeb.DayPackage ? DateTime.Now.AddDays(1) : DateTime.Now.AddMonths(1);
                    else
                        paymentAccepted.EndDate = model.Payment.ToString() == ConfigWeb.DayPackage ? paymentAccepted.EndDate.AddDays(1) : paymentAccepted.EndDate.AddMonths(1);
                    //insert to payment history
                    PaymentHistory pay = new PaymentHistory()
                    {
                        PaymentMethodId = 5,  // nap tiền
                        CreatedDate = DateTime.Now,
                        Amount = model.Payment,
                        Notes = "Nạp tài khoản",
                        UserId = userId
                    };
                    db.PaymentHistories.InsertOnSubmit(pay);

                    db.SubmitChanges();

                    return "Nạp tiền thành công";
                }
            }
        }

        public string GetCashPaymentByUserId(int userId)
        {
            var cash = db.PaymentAccepteds.FirstOrDefault(x => x.UserId == userId);
            if (cash != null)
                return string.Format("{0:n0}", cash.AmountTotal);
            else return "0";
        }
    }
}