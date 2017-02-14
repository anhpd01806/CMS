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
                    var userId = new UserBussiness().GetUserByName(model.UserName);
                    var paymentHistory = new PaymentHistory
                    {
                        UserId = userId,
                        PaymentMethodId = model.PaymentMethodId,
                        CreatedDate = DateTime.Now,
                        Notes = model.Note,
                        Amount = long.Parse(string.Join("", model.Amount.Split(',')))
                    };

                    //insert payment history
                    new PaymentBussiness().Insert(paymentHistory);

                    //insert payment accepted
                    new PaymentBussiness().PaymentAcceptedUpdate(model,userId);

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
     
        public ActionResult RegisterPackage()
        {
            int userId = Convert.ToInt32(Session["SS-USERID"]);
            Session["SS-USERNAME"] = new UserBussiness().GetUserById(userId).UserName;
            PaymentViewModel model = new PaymentViewModel();
            model.PackageList = new List<SelectListItem>();
            model.PackageList.Add(new SelectListItem { Text = "Gói tháng", Value = ConfigWeb.MonthPackage });
            model.PackageList.Add(new SelectListItem { Text = "Gói ngày", Value = ConfigWeb.DayPackage });
            return View(model);
        }

        [HttpPost]
        public ActionResult RegisterPackage(PaymentViewModel model)
        {
            int userId = Convert.ToInt32(Session["SS-USERID"]);
            var rs = new PaymentBussiness().UpdatePaymentAccepted(model, userId);
            TempData["Success"] = rs;
            ModelState.Clear();
            model = new PaymentViewModel();
            model.PackageList = new List<SelectListItem>();
            model.PackageList.Add(new SelectListItem { Text = "Gói tháng", Value = ConfigWeb.MonthPackage });
            model.PackageList.Add(new SelectListItem { Text = "Gói ngày", Value = ConfigWeb.DayPackage });
            return View(model);
        }

        public JsonResult GetHistory(string UserName, string Page)
        {
            try
            {
                int page = int.Parse(Page);
                int userID = new UserBussiness().GetUserByName(UserName);
                var itemList = (from a in new PaymentBussiness().GetPaymentHistoryByUserId(userID,page)
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