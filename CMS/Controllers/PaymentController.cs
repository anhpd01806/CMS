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

namespace CMS.Controllers
{
    public class PaymentController : BaseAuthedController
    {
        private static readonly Dictionary<String, String> errorMap = new Dictionary<String, String>()
        {
            { "-1985", "Invalid parameters request." },
            { "-1984", "Wrong username." },
            {"-1983", "Wrong password."},
            {"-1982", "Invalid IP request."},
            {"-1981", "System busy."},
            {"0", "Giao dich that bai."},
            {"1", "Giao dich thanh cong."},
            {"-10", "Mã thẻ sai định dạng."},
            {"4", "Thẻ không sử dụng được."},
            {"5", "Nhập sai mã thẻ quá 5 lần."},
            {"9", "Tạm thời khóa kênh nạp thẻ do hệ thống Mobifone quá tải."},
            {"10", "Hệ thống nhà cung cấp dịch vụ gập lỗi."},
            {"11", "Kết nối với nhà cung cấp bị gián đoạn."},
            {"13", "Hệ thống tạm thời bận."},
            {"-2", "Thẻ đã bị khóa."},
            {"-3", "Thẻ hết hạn sử dụng."},
            {"50", "Thẻ đã sử dụng hoặc không tồn tại."},
            {"51", "Serial thẻ không đúng."},
            {"52", "Mã thẻ và seerial không khớp."},
            {"53", "Serial hoặc mã thẻ không đúng."},
            {"55", "Card tạm thời bị khóa 24h."},
            {"59", "Mã thẻ chưa được kick hoạt." },
            {"99", "Giao dịch pending." },
            {"100", "Không tồn tại access key." },
            {"102", "Tài khoản tạm thời bị khóa 15 phút vì nạp sai mã thẻ quá 5 lần liên tiếp." },
        };

        [HttpPost]
        public JsonResult Recharge(RechargeModel form)
        {
            var resultObj = new RechargeModel();
            if (!isValidCode(form))
            {
                resultObj.isError = true;
                resultObj.message = errorMap["53"];
                return Json(resultObj, JsonRequestBehavior.AllowGet);
            }
            //get url
            string url = ConfigWeb.Api_Charging;
            //get accesskey
            string accesskey = ConfigWeb.Access_Key;
            string requestUrl = url + "?accesskey=" + accesskey + "&serial=" + form.SERIAL + "&pin=" + form.CODE + "&type=" + form.TELCO;
            // Create a request for the URL.   
            WebRequest rq = WebRequest.Create(requestUrl);
            // If required by the server, set the credentials.  
            rq.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.  
            WebResponse response = rq.GetResponse();
            // Display the status.  
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.  
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.  
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.  
            string code = reader.ReadToEnd();
            // Clean up the streams and the response.  
            reader.Close();
            response.Close();
            resultObj.isError = Int32.Parse(code) < 10000;
            if (!resultObj.isError)
            {
                try
                {
                    //insert card to db
                    int amount = Int32.Parse(code);
                    var paymentHistory = new PaymentHistory
                    {
                        UserId = Convert.ToInt32(Session["SS-USERID"]),
                        PaymentMethodId = 6,
                        CreatedDate = DateTime.Now,
                        Notes = "Nạp thẻ điện thoại",
                        Amount = amount
                    };

                    //insert payment history
                    new PaymentBussiness().Insert(paymentHistory);

                    //insert payment accepted
                    PaymentViewModel payment = new PaymentViewModel();
                    payment.Amount = amount.ToString();
                    new PaymentBussiness().PaymentAcceptedUpdate(payment, Convert.ToInt32(Session["SS-USERID"]));
                    resultObj.message = "Bạn đã nạp thành công " + string.Format("{0:n0}", amount) + "vnđ !Cảm ơn bạn đã sử dụng phần mềm.";
                }

                catch (Exception)
                {
                }
            }
            else
            {
                if (errorMap.ContainsKey(code))
                {
                    resultObj.message = errorMap[code];
                }
                else
                {
                    resultObj.message = "Nạp thẻ thất bại";
                }

            }
            return Json(resultObj, JsonRequestBehavior.AllowGet);
        }
        //check is valid card code
        private bool isValidCode(RechargeModel card)
        {
            bool result = true;
            try
            {
                switch (card.TELCO)
                {
                    case "VTT":
                        if (card.CODE.Length < 13 || card.SERIAL.Length < 11)
                            result = false;
                        break;
                    case "VNP":
                        if (card.CODE.Length < 12 || card.SERIAL.Length < 9)
                            result = false;
                        break;
                    case "VMS":
                        if (card.CODE.Length < 12 || card.SERIAL.Length < 9)
                            result = false;
                        break;
                    default:
                        result = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return result;
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
                    var userInfo = (User)Session["SS-USER"];
                    var userId = new UserBussiness().GetUserByName(model.UserName);
                    var paymentHistory = new PaymentHistory
                    {
                        UserId = userId,
                        PaymentMethodId = model.PaymentMethodId,
                        CreatedDate = DateTime.Now,
                        Notes = "Nhân viên nạp: " + userInfo.UserName + "Ghi chú: " + model.Note,
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