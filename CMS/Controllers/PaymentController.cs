using CMS.Bussiness;
using CMS.Data;
using CMS.Helper;
using CMS.ViewModel;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Web.Mvc;
using WebBackendPlus.Controllers;
using CMS.Models;
using System.Net;
using System.IO;
using System.Text;
using _1Pay;

namespace CMS.Controllers
{
    public class PaymentController : BaseAuthedController
    {
        private static readonly Dictionary<String, String> errorMap = new Dictionary<String, String>()
        {
            {"00", "Giao dịch thành công." },
            {"01", "Lỗi, địa chỉ IP truy cập API bị từ chối." },
            {"02", "Tham số gửi từ merchant tới chưa chính xác."},
            {"03", "Merchant không tồn tại hoặc merchant đang bị khóa kết nối."},
            {"04", "Mật khẩu hoặc chữ ký xác thực không chính xác."},
            {"05", "Trùng mã giao dịch (transRef)."},
            {"06", "Mã giao dịch không tồn tại hoặc sai định dạng."},
            {"07", "Thẻ đã được sử dụng, hoặc thẻ sai."},
            {"08", "Thẻ bị khóa."},
            {"09", "Thẻ hết hạn sử dụng."},
            {"10", "Thẻ chưa được kích hoạt hoặc không tồn tại."},
            {"11", "Mã thẻ sai định dạng."},
            {"12", "Sai số serial của thẻ."},
            {"13", "Mã thẻ và số serial không khớp."},
            {"14", "Thẻ không tồn tại."},
            {"15", "Thẻ không sử dụng được."},
            {"16", "Số lần thử (nhập sai liên tiếp) của thẻ vượt quá giới hạn cho phép."},
            {"17", "Hệ thống đơn vị phát hành (Telco) bị lỗi hoặc quá tải, thẻ chưa bị trừ."},
            {"18", "Hệ thống đơn vị phát hành (Telco) bị lỗi hoặc quá tải, thẻ có thể bị trừ, cần phối hợp với 1pay để tra soát."},
            {"19", "Đơn vị phát hành không tồn tại."},
            {"20", "Đơn vị phát hành không hỗ trợ nghiệp vụ này."},
            {"21", "Không hỗ trợ loại card này." },
            {"22", "Kết nối tới hệ thống đơn vị phát hành (Telco) bị lỗi." },
            {"23", "Kết nối 1Pay tới hệ thống đơn vị cung cấp bị lỗi." },
            {"99", "Nạp thẻ thất bại." },
        };

        [HttpPost]
        public JsonResult Recharge(RechargeModel form)
        {
            var json = new JsonPhoneCardResult();
            String result = "";
            int userId = Convert.ToInt32(Session["SS-USERID"]);
            var resultObj = new RechargeModel();
            if (!isValidCode(form))
            {
                resultObj.isError = true;
                resultObj.message = errorMap["13"];
                return Json(resultObj, JsonRequestBehavior.AllowGet);
            }
            My1Pay my1Pay = new My1Pay();
            //get url https://api.1pay.vn/card-charging/v5/topup
            string url = "https://api.1pay.vn/card-charging/v5/topup";//ConfigWeb.Api_Charging;
            //get accesskey
            string accesskey = "4c3ea60k63vy7v6mcec0";//ConfigWeb.Access_Key;

            string secretkey = "odt0ujuzr8xx9vvwvg7rak26pghxgnq3";

            string transRef = userId + DateTime.Now.ToString("yyyyMMddHHmmss");
            String signature = my1Pay.generateSignature_Card_V5_TopupApi(accesskey, form.CODE, form.SERIAL, transRef, form.TELCO, secretkey); //create signature

            String urlParameter = String.Format("access_key={0}&type={1}&pin={2}&serial={3}&signature={4}&transRef={5}", accesskey, form.TELCO, form.CODE, form.SERIAL, signature, transRef);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0";
                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "en-US,en;q=0.5");
                var data = Encoding.ASCII.GetBytes(urlParameter);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                json = JsonConvert.DeserializeObject<JsonPhoneCardResult>(result);
                response.Close();
            }
            catch (Exception e)
            {
                result = e.GetBaseException().ToString();
            }
            string code = json.status;
            resultObj.isError = !json.status.Equals("00");
            if (!resultObj.isError)
            {
                try
                {
                    //insert card to db
                    int amount = Int32.Parse(json.amount);
                    var paymentHistory = new PaymentHistory
                    {
                        UserId = userId,
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
                    case "viettel":
                        if (card.CODE.Length < 13 || card.SERIAL.Length < 11)
                            result = false;
                        break;
                    case "vinaphone":
                        if (card.CODE.Length < 12 || card.SERIAL.Length < 9)
                            result = false;
                        break;
                    case "mobifone":
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

        public ActionResult ChargeByPhoneCard()
        {
            int userId = Convert.ToInt32(Session["SS-USERID"]);
            Session["SS-USERNAME"] = new UserBussiness().GetUserById(userId).UserName;
            return View();
        }

        private class JsonPhoneCardResult
        {
            public string transId { get; set; }
            public string transRef { get; set; }
            public string serial { get; set; }
            public string status { get; set; }
            public string amount { get; set; }
            public string description { get; set; }
        }
    }
}