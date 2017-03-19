using CMS.Bussiness;
using CMS.Data;
using CMS.Helper;
using CMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebBackendPlus.Controllers;
using CMS.Models;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Security.Cryptography;

namespace CMS.Controllers
{
    public class PaymentController : BaseAuthedController
    {
        private static readonly Dictionary<String, String> errorMap = new Dictionary<String, String>()
        {
            { "-1985", "Invalid parameters request" },
            { "-1984", "Wrong username" },
            {"-1983", "Wrong password"},
            {"-1982", "Invalid IP request"},
            {"-1981", "System busy"},
            {"0", "Giao dich that bai"},
            {"1", "Giao dich thanh cong"},
            {"-10", "Ma the sai dinh dang"},
            {"4", "The khong su dung duoc"},
            {"5", "Nhap sai ma the qua 5 lan"},
            {"9", "Tam thoi khoa kenh nap the do he thong Mobifone qua tai"},
            {"10", "He thong nha cung cap gap loi"},
            {"11", "Ket noi voi nha cung cap gian doan"},
            {"13", "He thong tam thoi ban"},
            {"-2", "The da bi khoa"},
            {"-3", "The het han su dung"},
            {"50", "The da su dung hoac khong ton tai"},
            {"51", "Serial the khong dung"},
            {"52", "Ma the va serial khong khop"},
            {"53", "Serial hoac ma the khong dung"},
            {"55", "Card tam thoi bi khoa 24h"},
            {"59", "Ma the chua duoc kich hoat" },
        };

        [HttpPost]
        public JsonResult Recharge(RechargeModel form)
        {
            //get from file config
            String url = "http://cardgw.inet.vn/services/cardcharging.inet";
            var result = "";

            //get from file config 16 length
            String keyEncrypt = "y1Mg91Qzp04tm4Aw";
            //get from config
            String request = "CHARGE";
            String userName = "user";
            String password = "pass";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                byte[] passwordEncript = System.Text.Encoding.UTF8.GetBytes(password);
                String PWD = libAES.Instance.Encrypt(form.SERIAL, keyEncrypt);
                var data = new RechargeModel
                {
                    RQST = request,
                    USR = userName,
                    PWD = System.Convert.ToBase64String(passwordEncript),
                    TELCO = form.TELCO,
                    SERIAL = libAES.Instance.Encrypt(form.SERIAL, keyEncrypt),
                    CODE = libAES.Instance.Encrypt(form.CODE, keyEncrypt)
                };
                var json = new JavaScriptSerializer().Serialize(data);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Recharge()
        {
            return View();
        }

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
                    new PaymentBussiness().PaymentAcceptedUpdate(model, userId);

                    TempData["Success"] = "Nạp tiền thành công";
                    //insert notifycation
                    var notify = new Notify
                    {
                        UserName = "Admin",
                        Userid = userId,
                        SendFlag = false,
                        DateSend = DateTime.Now,
                        Title = "Bạn đã nạp thành công " + model.Amount + "đ",
                        Accepted = false,
                        ViewFlag = false,
                        SendTo = userId,
                        Type = 3
                    };
                    var notifyId = new NotifyBussiness().Insert(notify);
                    notify.Id = notifyId;
                    TempData["Notify"] = notify;
                    ModelState.Clear();
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex;
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
            model.PackageList.Add(new SelectListItem { Text = "Gói tháng(" + string.Format("{0:n0}", int.Parse(ConfigWeb.MonthPackage)) + "vnđ)", Value = ConfigWeb.MonthPackage });
            model.PackageList.Add(new SelectListItem { Text = "Gói ngày(" + string.Format("{0:n0}", int.Parse(ConfigWeb.DayPackage)) + "vnđ)", Value = ConfigWeb.DayPackage });
            return View(model);
        }

        [HttpPost]
        public ActionResult RegisterPackage(PaymentViewModel model)
        {
            int userId = Convert.ToInt32(Session["SS-USERID"]);
            var rs = new PaymentBussiness().UpdatePaymentAccepted(model, userId);
            if (rs == "Nạp tiền thành công")
            {
                TempData["Success"] = rs;
                Session["USER-ACCEPTED"] = true;
            }
            else
            {
                TempData["Error"] = rs;
            }
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
                var itemList = (from a in new PaymentBussiness().GetPaymentHistoryByUserId(userID, page)
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
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

    }
}