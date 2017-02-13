using CMS.Bussiness;
using CMS.Data;
using CMS.Helper;
using CMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebBackendPlus.Controllers;

namespace CMS.Controllers
{
    public class PaymentController : BaseAuthedController
    {
        // GET: Payment
        public ActionResult Index()
        {
            PaymentViewModel model = new PaymentViewModel();
            model.PayMethodList = new PaymentBussiness().GetPaymentMethod();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(PaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var paymentHistory = new PaymentHistory
                    {
                        UserId = new UserBussiness().GetUserByName(model.UserName),
                        PaymentMethodId = model.PaymentMethodId,
                        CreatedDate = DateTime.Now,
                        Notes = model.Note,
                        Amount = long.Parse(string.Join("", model.Amount.Split(',')))
                    };

                    //insert payment history
                    new PaymentBussiness().Insert(paymentHistory);
                    TempData["Success"] = "Nạp tiền thành công";
                    ModelState.Clear();
                }
                catch (Exception ex)
                {
                    TempData["Error"] = Messages_Contants.ERROR_COMMON;
                }
            }
            model = new PaymentViewModel();
            model.PayMethodList = new PaymentBussiness().GetPaymentMethod();
            return View(model);
        }
        
        [AllowAnonymous]
        public JsonResult GetHistory(string UserName)
        {
            try
            {
                int userID = new UserBussiness().GetUserByName(UserName);
                var itemList = (from a in new PaymentBussiness().GetPaymentHistoryByUserId(userID)
                                select new PaymentHistoryModel
                                {
                                    Id = a.Id,
                                    PaymentMethod = new PaymentBussiness().GetPaymentMethodById(a.PaymentMethodId),
                                    DateString = a.CreatedDate.ToString("dd/MM/yyyy"),
                                    Amount = string.Format("{0:n0}", a.Amount),
                                    Notes = a.Notes
                                }).ToList();
                return Json(itemList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

    }
}