using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace CMS.API.Controllers
{
    public class APIController : Controller
    {
        // GET: API
        public ActionResult TestApi()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(string username, string password, string sign)
        {

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(sign))
            {
                return Json(new
                {
                    status = "200",
                    errorcode = "1100",
                    message = "one or more parameter is empty"
                });
            }
            else
            {
                var param = new NameValueCollection();
                param.Add("username", username);
                param.Add("password", password);
                var str = Common.Common.Sort(param);
                var gen_sign = Common.Common.GenSign(str, Common.APIConfig.PrivateKey);

                if(!sign.Equals(gen_sign))
                {
                    return Json(new
                    {
                        status = "200",
                        errorcode = "1200",
                        message = "invalid signature"
                    });
                }
                else
                {

                }
            }
            return Json("");
        }

        [HttpPost]
        public JsonResult GenSign(string str)
        {
            return Json(Common.Common.GenSign(str, Common.APIConfig.PrivateKey));
        }
    }
}