using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Collections.Specialized;
using Elmah;
using CMS.API.Bussiness;
using CMS.API.Models;

namespace CMS.API.Controllers
{
    public class APIController : Controller
    {
        #region member
        private readonly AccountBussiness _accountbussiness = new AccountBussiness();
        private readonly NewsBussiness _newsbussiness = new NewsBussiness();
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
                    var gen_sign = Common.Common.GenSign(str.ToLower(), Common.APIConfig.PrivateKey);

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
        public JsonResult GetListDistric(string sign)
        {
            try
            {
                if (string.IsNullOrEmpty(sign))
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
                    var gen_sign = Common.Common.GenSign(string.Empty, Common.APIConfig.PrivateKey);

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
                        var data = _newsbussiness.GetListDistric();
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = data
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
        public JsonResult GetListCategory(string sign)
        {
            try
            {
                if (string.IsNullOrEmpty(sign))
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
                    var gen_sign = Common.Common.GenSign(string.Empty, Common.APIConfig.PrivateKey);

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
                        var data = _newsbussiness.GetListCategory();
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = data
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
        public JsonResult GetListSite(string sign)
        {
            try
            {
                if (string.IsNullOrEmpty(sign))
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
                    var gen_sign = Common.Common.GenSign(string.Empty, Common.APIConfig.PrivateKey);

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
                        var data = _newsbussiness.GetListSite();
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = data
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
        public JsonResult GetlistStatus(string sign)
        {
            try
            {
                if (string.IsNullOrEmpty(sign))
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
                    var gen_sign = Common.Common.GenSign(string.Empty, Common.APIConfig.PrivateKey);

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
                        var data = _newsbussiness.GetlistStatusModel();
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = data
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
        public JsonResult GetListNewsInHome(int UserId, int CateId, int DistricId, int StatusId, int SiteId,
            int BackDate, string From, string To, double MinPrice, double MaxPrice, int pageIndex, int pageSize, bool IsRepeat, string key, string NameOrder, bool descending, string sign)
        {
            try
            {
                if (string.IsNullOrEmpty(UserId.ToString()) || string.IsNullOrEmpty(CateId.ToString()) || string.IsNullOrEmpty(DistricId.ToString()) || string.IsNullOrEmpty(StatusId.ToString()) || string.IsNullOrEmpty(StatusId.ToString()) || string.IsNullOrEmpty(SiteId.ToString()) || string.IsNullOrEmpty(BackDate.ToString()) || string.IsNullOrEmpty(MinPrice.ToString()) || string.IsNullOrEmpty(MaxPrice.ToString()) || string.IsNullOrEmpty(pageIndex.ToString()) || string.IsNullOrEmpty(pageSize.ToString()) || string.IsNullOrEmpty(IsRepeat.ToString()) || string.IsNullOrEmpty(descending.ToString()) || string.IsNullOrEmpty(sign))
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
                    param.Add("UserId", UserId.ToString());
                    param.Add("CateId", CateId.ToString());
                    param.Add("DistricId", DistricId.ToString());
                    param.Add("StatusId", StatusId.ToString());
                    param.Add("SiteId", SiteId.ToString());
                    param.Add("BackDate", BackDate.ToString());
                    param.Add("From", From.ToString());
                    param.Add("To", To.ToString());
                    param.Add("MinPrice", MinPrice.ToString());
                    param.Add("MaxPrice", MaxPrice.ToString());
                    param.Add("pageIndex", pageIndex.ToString());
                    param.Add("pageSize", pageSize.ToString());
                    param.Add("IsRepeat", IsRepeat.ToString());
                    param.Add("key", key.ToString());
                    param.Add("NameOrder", NameOrder.ToString());
                    param.Add("descending", descending.ToString());
                    var str = Common.Common.Sort(param).ToLower();
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
                        int total = 0;
                        var data = _newsbussiness.GetListNewByFilter(UserId, CateId, DistricId, StatusId, SiteId, BackDate, From, To, MinPrice, MaxPrice, pageIndex, pageSize, Convert.ToBoolean(IsRepeat), key, NameOrder, descending, ref total);
                        var model = new HomeModel();
                        model.Total = total;
                        model.pageIndex = pageIndex;
                        model.pageSize = pageSize;
                        model.ListNew = data;
                        model.Totalpage = (int)Math.Ceiling((double)model.Total / (double)model.pageSize);
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = model
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
        public JsonResult GetNewsDetail(int Id, int UserId, string sign)
        {
            try
            {
                if (string.IsNullOrEmpty(sign) || string.IsNullOrEmpty(Id.ToString()) || string.IsNullOrEmpty(UserId.ToString()))
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
                    param.Add("Id", Id.ToString());
                    param.Add("UserId", UserId.ToString());
                    var str = Common.Common.Sort(param);
                    var gen_sign = Common.Common.GenSign(str.ToLower(), Common.APIConfig.PrivateKey);

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
                        var data = _newsbussiness.GetNewsDetail(Id, UserId);
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = data
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
            return Json(Common.Common.GenSign(str.ToLower(), Common.APIConfig.PrivateKey));
        }
    }
}