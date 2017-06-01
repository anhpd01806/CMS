using CMS.API.Data;
using CMS.API.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.API.Bussiness
{
    public class PaymentBussiness : InitDB
    {
        public int UpdatePaymentAccepted(long payment, int userId)
        {
            var paymentAccepted = Instance.PaymentAccepteds.FirstOrDefault(x => x.UserId == userId);
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
                    Instance.PaymentHistories.InsertOnSubmit(pay);

                    Instance.SubmitChanges();

                    return 0;// "Nạp tiền thành công";
                }
            }
        }

        public void Insert(PaymentHistory model)
        {
            Instance.PaymentHistories.InsertOnSubmit(model);
            Instance.SubmitChanges();
        }
    }
}