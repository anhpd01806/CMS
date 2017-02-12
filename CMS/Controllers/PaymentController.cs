using CMS.Bussiness;
using CMS.Data;
using CMS.Helper;
using CMS.ViewModel;
using System;
using System.Web.Mvc;

namespace CMS.Controllers
{
    public class PaymentController : Controller
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
                }
                catch (Exception ex)
                {
                    TempData["Error"] = Messages_Contants.ERROR_COMMON;
                }
            }
            return RedirectToAction("Index","Payment");
        }
        
        public JsonResult GetHistory(string UserName)
        {
            try
            {
                int userID = new UserBussiness().GetUserByName(UserName);
                var listHistoryPayment = new PaymentBussiness().GetPaymentHistoryByUserId(userID);
                var abcObj = new PaymentHistory { Id = 1, Amount = 2 };
                return Json(abcObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

    }
}