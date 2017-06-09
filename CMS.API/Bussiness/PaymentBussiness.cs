using CMS.API.Data;
using CMS.API.Helper;
using CMS.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.API.Bussiness
{
    public class PaymentBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();
        public int UpdatePaymentAccepted(long payment, int userId)
        {
            var paymentAccepted = db.PaymentAccepteds.FirstOrDefault(x => x.UserId == userId);
            if (paymentAccepted == null)
                return 1;// "Bạn chưa nạp tiền. Vui lòng liên hệ admin để nạp tiền"
            else
            {
                if (paymentAccepted.AmountTotal < payment) return 2; // "Tài khoản của quý khách không đủ tiền"
                else
                {
                    paymentAccepted.AmountTotal = paymentAccepted.AmountTotal - payment;
                    paymentAccepted.StartDate = DateTime.Now;
                    //th tk đã hết hạn sử dụng
                    if (DateTime.Now > paymentAccepted.EndDate)
                        paymentAccepted.EndDate = payment.ToString() == ConfigWeb.DayPackage ? DateTime.Now.AddDays(1) : DateTime.Now.AddMonths(1);
                    else
                        paymentAccepted.EndDate = payment.ToString() == ConfigWeb.DayPackage ? paymentAccepted.EndDate.AddDays(1) : paymentAccepted.EndDate.AddMonths(1);
                    //insert to payment history
                    var pay = new PaymentHistory
                    {
                        PaymentMethodId = 5,  // nap tiền
                        CreatedDate = DateTime.Now,
                        Amount = payment,
                        Notes = "Nạp tài khoản",
                        UserId = userId
                    };
                    db.PaymentHistories.InsertOnSubmit(pay);

                    db.SubmitChanges();

                    return 0;// "Nạp tiền thành công";
                }
            }
        }

        public void Insert(PaymentHistory model)
        {
            db.PaymentHistories.InsertOnSubmit(model);
            db.SubmitChanges();
        }

        public DateTime GetEndTimeByUserId(int userId)
        {
            var cash = db.PaymentAccepteds.FirstOrDefault(x => x.UserId == userId);
            if (cash != null)
                return cash.EndDate;
            else return DateTime.MinValue;
        }

        public string GetCashPaymentByUserId(int userId)
        {
            var cash = db.PaymentAccepteds.FirstOrDefault(x => x.UserId == userId);
            if (cash != null)
                return string.Format("{0:n0}", cash.AmountTotal);
            else return "0";
        }

        public string GetTimePaymentByUserId(int userId)
        {
            var cash = db.PaymentAccepteds.FirstOrDefault(x => x.UserId == userId);
            if (cash != null)
                return string.Format(cash.EndDate.ToString("dd/MM/yyyy"));
            else return "Chưa đăng ký gói cước";
        }

        public List<PaymentTotal> GetPaymentByUserId(int id)
        {
            var rs = (from a in db.PaymentHistories
                      join b in db.PaymentMethods
                      on a.PaymentMethodId equals b.Id
                      where (a.PaymentMethodId == 1 || a.PaymentMethodId == 2)
                      && a.CreatedDate.Month == DateTime.Now.AddMonths(-1).Month
                      && a.UserId == id
                      group a by new
                      {
                          a.PaymentMethodId,
                          a.PaymentMethod
                      } into gr
                      select new PaymentTotal
                      {
                          PaymentMethodId = gr.Key.PaymentMethodId,
                          TypePayment = gr.Key.PaymentMethod.ToString(),
                          AmoutPayment = string.Format("{0:n0}", gr.Sum(x => x.Amount))
                      }).ToList();
            return rs;
        }
    }
}