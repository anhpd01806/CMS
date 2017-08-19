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

        public List<PaymentMethod> GetPaymentList()
        {
            return db.PaymentMethods.OrderBy(x => x.DisplayOrder).ToList();
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

        public void PaymentAcceptedApi(int userid, long amount)
        {
            var paymentAccepted = db.PaymentAccepteds.FirstOrDefault(x => x.UserId == userid);
            if (paymentAccepted == null)
            {
                var payment = new PaymentAccepted
                {
                    UserId = userid,
                    AmountTotal = amount,
                    StartDate = DateTime.Now.AddDays(-1),
                    EndDate = DateTime.Now.AddDays(-1)
                };

                //insert to table payment accepted
                db.PaymentAccepteds.InsertOnSubmit(payment);
            }
            else
            {
                paymentAccepted.AmountTotal = paymentAccepted.AmountTotal + amount;
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

        public List<PaymentHistory> GetPaymentHistoryApi(int userId, int page, ref int totalPage)
        {
            totalPage = db.PaymentHistories.Where(x => x.UserId == userId).Count() / 20;
            if (page == 0)
                return db.PaymentHistories.Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedDate).Take(20).ToList();
            else
                return db.PaymentHistories.Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedDate).Skip(page * 20).Take(20).ToList();
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
                if (paymentAccepted.AmountTotal < model.Payment
                    || (ConfigWeb.MonthPackage != model.Payment.ToString() && ConfigWeb.DayPackage != model.Payment.ToString()))
                    return "Tài khoản của quý khách không đủ tiền";
                else
                {
                    paymentAccepted.AmountTotal = paymentAccepted.AmountTotal - model.Payment;
                    paymentAccepted.StartDate = DateTime.Now;
                    //th tk đã hết hạn sử dụng
                    if (DateTime.Now > paymentAccepted.EndDate)
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

        public string GetAllCashPaymentByAdmin(int adminId)
        {
            var cash = (from u in db.Users
                        join p in db.PaymentHistories on u.Id equals p.UserId
                        where u.ManagerBy == adminId
                        && p.PaymentMethodId != 5 // tai khoan thanh toan
                        && p.CreatedDate.Month < DateTime.Now.Month && p.CreatedDate.Month >= DateTime.Now.AddMonths(-1).Month
                        select p.Amount).Sum();
            if (cash != null)
                return string.Format("{0:n0}", cash);
            else return "0";
        }

        public string GetTimePaymentByUserId(int userId)
        {
            var cash = db.PaymentAccepteds.FirstOrDefault(x => x.UserId == userId);
            if (cash != null)
                return string.Format(cash.EndDate.ToString("dd/MM/yyyy"));
            else return "Chưa đăng ký gói cước";
        }

        public DateTime? GetTimePaymentApi(int userId, ref string timeEndStr)
        {
            var cash = db.PaymentAccepteds.FirstOrDefault(x => x.UserId == userId);
            if (cash != null)
            {
                timeEndStr = string.Format(cash.EndDate.ToString("dd/MM/yyyy"));
                return cash.EndDate;
            }
            else
            {
                timeEndStr = "Chưa đăng ký gói cước";
                return DateTime.MinValue;
            }
        }
        public DateTime GetEndTimeByUserId(int userId)
        {
            var cash = db.PaymentAccepteds.FirstOrDefault(x => x.UserId == userId);
            if (cash != null)
                return cash.EndDate;
            else return DateTime.MinValue;
        }

        public DateTime? GetEndTimeApiByUserId(int userId)
        {
            var cash = db.PaymentAccepteds.FirstOrDefault(x => x.UserId == userId);
            if (cash != null)
                return cash.EndDate;
            else return null;
        }
        /// <summary>
        /// Status:  0: "Bạn không có tiền trong tài khoản.";
        ///          1: "Thành công";
        ///          2: "Tài khoản của bạn không đủ tiền.";
        ///          3: "Hệ thống đang gặp sự cố. vui lòng thử lại!";
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int PaymentForCreateNews(long amount, int userId)
        {
            try
            {
                var paymentAccepted = db.PaymentAccepteds.FirstOrDefault(x => x.UserId == userId);
                if (paymentAccepted == null) return 0;
                if (amount > paymentAccepted.AmountTotal) return 2;

                // insert historypayment
                var paymentHis = new PaymentHistory
                {
                    PaymentMethodId = 5,
                    CreatedDate = DateTime.Now,
                    Amount = -amount,
                    Notes = "Trừ tiền đăng bài",
                    UserId = userId
                };

                db.PaymentHistories.InsertOnSubmit(paymentHis);
                // minus cash on amount
                paymentAccepted.AmountTotal = paymentAccepted.AmountTotal - amount;
                db.SubmitChanges();

                return 1;
            }
            catch (Exception)
            {
                return 3;
            }
        }

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
    }
}