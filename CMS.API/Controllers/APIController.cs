using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Collections.Specialized;
using Elmah;
using CMS.API.Bussiness;

namespace CMS.API.Controllers
{
    public class APIController : Controller
    {
        #region member
        public AccountBussiness _accountbussiness = new AccountBussiness();
        #endregion
        // GET: API
        public ActionResult TestApi()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(string username, string password, string sign)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(sign))
                {
                    return Json(new
                    {
                        status = "200",
                        errorcode = "1100",
                        message = "one or more parameter is empty",
                        data = ""
                    });
                }
                else
                {
                    var param = new NameValueCollection();
                    param.Add("username", username);
                    param.Add("password", password);
                    var str = Common.Common.Sort(param);
                    var gen_sign = Common.Common.GenSign(str, Common.APIConfig.PrivateKey);

                    if (!sign.Equals(gen_sign))
                    {
                        return Json(new
                        {
                            status = "200",
                            errorcode = "1200",
                            message = "invalid signature",
                            data = ""
                        });
                    }
                    else
                    {
                        var userItem = _accountbussiness.Login(username, password);
                        if(userItem == null)
                        {
                            return Json(new
                            {
                                status = "200",
                                errorcode = "2100",
                                message = "the username or password is incorrect",
                                data = userItem
                            });
                        }
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = userItem
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return Json(new
                {
                    status = "200",
                    errorcode = "5000",
                    message = "system error",
                    data = ""
                });
            }
        }

        [HttpPost]
        public JsonResult GenSign(string str)
        {
            return Json(Common.Common.GenSign(str, Common.APIConfig.PrivateKey));
        }
    }
}