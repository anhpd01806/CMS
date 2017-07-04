using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elmah;
using CMS.Bussiness;
using CMS.Models;
using CMS.Data;
using CMS.Helper;
using System.Net;
using System.IO;
using CMS.ViewModel;

namespace CMS.Controllers
{
    public class ApiController : Controller
    {
        #region Member
        private readonly ApiBussiness _accountbussiness = new ApiBussiness();
        private readonly APINewsBussiness _newsbussiness = new APINewsBussiness();
        #endregion

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
                        var userId = _accountbussiness.GetIdByAccount(username);
                        if (!CheckUserLogin(userId))
                        {
                            return Json(new
                            {
                                status = "200",
                                errorcode = "2200",
                                message = "Tài khoản đang sử dụng phần mềm ở một nơi khác. vui lòng thoát tài khoản đang sử dụng hoặc thử lại sau 10 phút.",
                                data = 0
                            });
                        }

                        var userItem = _accountbussiness.Login(username, password);
                        if (userItem == null)
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
        public JsonResult GetAmountCustomer(int userid, string sign)
        {
            try
            {
                if (string.IsNullOrEmpty(userid.ToString()) || string.IsNullOrEmpty(sign))
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
                    param.Add("userid", userid.ToString());
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
                        string amount = new PaymentBussiness().GetCashPaymentByUserId(userid);
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = amount
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
        public JsonResult Logout(int userid, string sign)
        {
            try
            {
                if (string.IsNullOrEmpty(userid.ToString()) || string.IsNullOrEmpty(sign))
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
                    param.Add("userid", userid.ToString());
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
                        System.Web.HttpContext.Current.Application.Remove("usr_" + userid);
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = ""
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

        /// <summary>
        /// Lấy tin theo status id
        /// 1. Tin đã lưu,
        /// 3. Tin đã ẩn
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="CateId"></param>
        /// <param name="DistricId"></param>
        /// <param name="StatusId"></param>
        /// <param name="SiteId"></param>
        /// <param name="BackDate"></param>
        /// <param name="From"></param>
        /// <param name="To"></param>
        /// <param name="MinPrice"></param>
        /// <param name="MaxPrice"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="newsStatus"></param>
        /// <param name="IsRepeat"></param>
        /// <param name="key"></param>
        /// <param name="NameOrder"></param>
        /// <param name="descending"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetListNewStatus(int UserId, int CateId, int DistricId, int StatusId, int SiteId,
            int BackDate, string From, string To, double MinPrice, double MaxPrice, int pageIndex, int pageSize, int newsStatus, bool IsRepeat, string key, string NameOrder, bool descending, string sign)
        {
            try
            {
                if (string.IsNullOrEmpty(UserId.ToString()) || string.IsNullOrEmpty(CateId.ToString()) || string.IsNullOrEmpty(DistricId.ToString()) || string.IsNullOrEmpty(StatusId.ToString()) || string.IsNullOrEmpty(StatusId.ToString()) || string.IsNullOrEmpty(SiteId.ToString()) || string.IsNullOrEmpty(BackDate.ToString()) || string.IsNullOrEmpty(MinPrice.ToString()) || string.IsNullOrEmpty(MaxPrice.ToString()) || string.IsNullOrEmpty(pageIndex.ToString()) || string.IsNullOrEmpty(pageSize.ToString()) || string.IsNullOrEmpty(IsRepeat.ToString()) || string.IsNullOrEmpty(descending.ToString()) || string.IsNullOrEmpty(sign) || string.IsNullOrEmpty(newsStatus.ToString()))
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
                    param.Add("newsStatus", newsStatus.ToString());
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
                        var data = _newsbussiness.GetListNewStatusByFilter(UserId, CateId, DistricId, StatusId, SiteId, BackDate, From, To, MinPrice, MaxPrice, pageIndex, pageSize, newsStatus, Convert.ToBoolean(IsRepeat), key, NameOrder, descending, ref total);
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

        /// <summary>
        /// Lấy danh sách tin đã xóa
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="CateId"></param>
        /// <param name="DistricId"></param>
        /// <param name="StatusId"></param>
        /// <param name="SiteId"></param>
        /// <param name="BackDate"></param>
        /// <param name="From"></param>
        /// <param name="To"></param>
        /// <param name="MinPrice"></param>
        /// <param name="MaxPrice"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="IsRepeat"></param>
        /// <param name="key"></param>
        /// <param name="NameOrder"></param>
        /// <param name="descending"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetListNewsDelete(int UserId, int CateId, int DistricId, int StatusId, int SiteId,
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
                        var data = _newsbussiness.GetListNewDeleteByFilter(UserId, CateId, DistricId, StatusId, SiteId, BackDate, From, To, MinPrice, MaxPrice, pageIndex, pageSize, Convert.ToBoolean(IsRepeat), key, NameOrder, descending, ref total);
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

        /// <summary>
        /// Lưu bài viết
        /// </summary>
        /// <param name="listNewsId"></param>
        /// <param name="userId"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UserSaveNews(int[] listNewsId, int userId, string sign)
        {
            try
            {
                if (listNewsId == null || string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(sign.ToString()))
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
                    param.Add("listNewsId", "[" + String.Join(",", listNewsId) + "]");
                    param.Add("userId", userId.ToString());
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
                        if (listNewsId.Length > 0)
                        {
                            var listItem = new List<News_Customer_Mapping>();
                            for (int i = 0; i < listNewsId.Length; i++)
                            {
                                listItem.Add(new News_Customer_Mapping
                                {
                                    CustomerId = userId,
                                    NewsId = listNewsId[i],
                                    IsSaved = true,
                                    IsDeleted = false,
                                    IsReaded = false,
                                    IsAgency = false,
                                    IsSpam = false,
                                    CreateDate = DateTime.Now
                                });
                            }
                            var result = _newsbussiness.SaveNewByUserId(listItem, userId);
                            return Json(new
                            {
                                status = "200",
                                errorcode = "0",
                                message = "success",
                                data = result
                            });
                        }
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = "2"
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

        /// <summary>
        /// Bỏ tin đã lưu ra khỏi danh sách
        /// </summary>
        /// <param name="listNewsId"></param>
        /// <param name="userId"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UserRemoveNewsSave(int[] listNewsId, int userId, string sign)
        {
            try
            {
                if (listNewsId == null || string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(sign.ToString()))
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
                    param.Add("listNewsId", "[" + String.Join(",", listNewsId) + "]");
                    param.Add("userId", userId.ToString());
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
                        if (listNewsId.Length > 0)
                        {
                            var listItem = new List<News_Customer_Mapping>();
                            for (int i = 0; i < listNewsId.Length; i++)
                            {
                                listItem.Add(new News_Customer_Mapping
                                {
                                    NewsId = listNewsId[i]
                                });
                            }
                            var result = new NewsBussiness().RemoveSaveNewByUserId(listItem, userId);
                            return Json(new
                            {
                                status = "200",
                                errorcode = "0",
                                message = "success",
                                data = result
                            });
                        }
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = "2"
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

        /// <summary>
        /// Ẩn bài viết
        /// </summary>
        /// <param name="listNewsId"></param>
        /// <param name="userId"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UserHideNews(int[] listNewsId, int userId, string sign)
        {
            try
            {
                if (listNewsId == null || string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(sign.ToString()))
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
                    param.Add("listNewsId", "[" + String.Join(",", listNewsId) + "]");
                    param.Add("userId", userId.ToString());
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
                        if (listNewsId.Length > 0)
                        {
                            var listItem = new List<News_Customer_Mapping>();
                            for (int i = 0; i < listNewsId.Length; i++)
                            {
                                listItem.Add(new News_Customer_Mapping
                                {
                                    CustomerId = userId,
                                    NewsId = listNewsId[i],
                                    IsSaved = false,
                                    IsDeleted = true,
                                    IsReaded = false,
                                    IsAgency = false,
                                    IsSpam = false,
                                    CreateDate = DateTime.Now
                                });
                            }
                            var result = _newsbussiness.HideNewByUserId(listItem, userId);
                            return Json(new
                            {
                                status = "200",
                                errorcode = "0",
                                message = "success",
                                data = result
                            });
                        }
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = "2"
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

        /// <summary>
        /// Bỏ tin đã ẩn ra khỏi danh sách
        /// </summary>
        /// <param name="listNewsId"></param>
        /// <param name="userId"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UserRemoveNewsHide(int[] listNewsId, int userId, string sign)
        {
            try
            {
                if (listNewsId == null || string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(sign.ToString()))
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
                    param.Add("listNewsId", "[" + String.Join(",", listNewsId) + "]");
                    param.Add("userId", userId.ToString());
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
                        if (listNewsId.Length > 0)
                        {
                            var listItem = new List<News_Customer_Mapping>();
                            for (int i = 0; i < listNewsId.Length; i++)
                            {
                                listItem.Add(new News_Customer_Mapping
                                {
                                    NewsId = listNewsId[i]
                                });
                            }
                            var result = new NewsBussiness().RemoveHideNewByUserId(listItem, userId);
                            return Json(new
                            {
                                status = "200",
                                errorcode = "0",
                                message = "success",
                                data = result
                            });
                        }
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = "2"
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

        /// <summary>
        /// Xóa bài viết
        /// </summary>
        /// <param name="listNewsId"></param>
        /// <param name="userId"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public JsonResult DeleteNews(int[] listNewsId, int userId, string sign)
        {
            try
            {
                if (listNewsId == null || string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(sign.ToString()))
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
                    param.Add("listNewsId", "[" + String.Join(",", listNewsId) + "]");
                    param.Add("userId", userId.ToString());
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
                        if (listNewsId.Length > 0)
                        {
                            var result = _newsbussiness.Delete(listNewsId, userId);
                            return Json(new
                            {
                                status = "200",
                                errorcode = "0",
                                message = "success",
                                data = result
                            });
                        }
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = "2"
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

        /// <summary>
        /// Báo tin chính chủ
        /// </summary>
        /// <param name="listNewsId"></param>
        /// <param name="userId"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult NewsForUser(int[] listNewsId, int userId, string sign)
        {
            try
            {
                if (listNewsId == null || string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(sign.ToString()))
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
                    param.Add("listNewsId", "[" + String.Join(",", listNewsId) + "]");
                    param.Add("userId", userId.ToString());
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
                        if (listNewsId.Length > 0)
                        {
                            var result = _newsbussiness.NewsforUser(listNewsId, userId);
                            return Json(new
                            {
                                status = "200",
                                errorcode = "0",
                                message = "success",
                                data = result
                            });
                        }
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = "2"
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

        /// <summary>
        /// Bỏ tin chính chủ
        /// </summary>
        /// <param name="listNewsId"></param>
        /// <param name="userId"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RemoveNewsforUser(int[] listNewsId, int userId, string sign)
        {
            try
            {
                if (listNewsId == null || string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(sign.ToString()))
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
                    param.Add("listNewsId", "[" + String.Join(",", listNewsId) + "]");
                    param.Add("userId", userId.ToString());
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
                        if (listNewsId.Length > 0)
                        {
                            var result = _newsbussiness.RemoveNewsforUser(listNewsId, userId);
                            return Json(new
                            {
                                status = "200",
                                errorcode = "0",
                                message = "success",
                                data = result
                            });
                        }
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = "2"
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

        /// <summary>
        /// Chặn tin
        /// </summary>
        /// <param name="listNewsId"></param>
        /// <param name="userId"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult NewsSpam(int[] listNewsId, int userId, string sign)
        {
            try
            {
                if (listNewsId == null || string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(sign.ToString()))
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
                    param.Add("listNewsId", "[" + String.Join(",", listNewsId) + "]");
                    param.Add("userId", userId.ToString());
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
                        if (listNewsId.Length > 0)
                        {
                            var listItem = new List<New>();
                            for (int i = 0; i < listNewsId.Length; i++)
                            {
                                var news = new HomeBussiness().GetNewsDetail(listNewsId[i]);
                                listItem.Add(news);
                            }
                            var result = new HomeBussiness().Spam(listItem, userId);
                            return Json(new
                            {
                                status = "200",
                                errorcode = "0",
                                message = "success",
                                data = result
                            });
                        }
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = "2"
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

        /// <summary>
        /// Báo môi giới
        /// </summary>
        /// <param name="listNewsId"></param>
        /// <param name="userId"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ReportNews(int[] listNewsId, int userId, string sign)
        {
            try
            {
                if (listNewsId == null || string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(sign.ToString()))
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
                    param.Add("listNewsId", "[" + String.Join(",", listNewsId) + "]");
                    param.Add("userId", userId.ToString());
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
                        if (listNewsId.Length > 0)
                        {
                            var listItem = new List<New>();
                            for (int i = 0; i < listNewsId.Length; i++)
                            {
                                var news = new HomeBussiness().GetNewsDetail(listNewsId[i]);
                                listItem.Add(news);
                            }
                            var result = new HomeBussiness().ReportNews(listItem, userId);
                            return Json(new
                            {
                                status = "200",
                                errorcode = "0",
                                message = "success",
                                data = result
                            });
                        }
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = "2"
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

        /// <summary>
        /// Status : 1 - Thành công.
        ///          2 - Không đủ tiền
        ///          0 - Thất bại
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="CateId"></param>
        /// <param name="DistrictId"></param>
        /// <param name="Phone"></param>
        /// <param name="Price"></param>
        /// <param name="Content"></param>
        /// <param name="UserId"></param>
        /// <param name="IsUser"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult CreateNews(string Title, int CateId, int DistrictId, string Phone, double Price, string Content, int UserId, bool IsUser, string sign)
        {
            try
            {
                if (string.IsNullOrEmpty(Title.ToString()) || string.IsNullOrEmpty(CateId.ToString()) || string.IsNullOrEmpty(DistrictId.ToString()) || string.IsNullOrEmpty(Phone) || string.IsNullOrEmpty(Price.ToString()) || string.IsNullOrEmpty(Content) || string.IsNullOrEmpty(UserId.ToString()) || string.IsNullOrEmpty(IsUser.ToString()) || string.IsNullOrEmpty(sign))
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
                    param.Add("Title", Title.ToString());
                    param.Add("CateId", CateId.ToString());
                    param.Add("DistrictId", DistrictId.ToString());
                    param.Add("Phone", Phone.ToString());
                    param.Add("Price", Price.ToString());
                    param.Add("Content", Content.ToString());
                    param.Add("UserId", UserId.ToString());
                    param.Add("IsUser", IsUser.ToString());
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
                        var resp = 1;
                        var newsItem = new New();
                        var count = _newsbussiness.CheckRepeatNews(Phone, DistrictId, UserId);
                        if (IsUser) //nếu là user thì kiểm tra tiền trong tài khoản
                        {
                            resp = _newsbussiness.PaymentForCreateNews(Convert.ToInt32(ConfigWeb.MinPayment), UserId);
                        }
                        if (resp == 1 || !IsUser)
                        {
                            newsItem.CategoryId = CateId;
                            newsItem.Title = Title;
                            newsItem.Contents = Content;
                            newsItem.Link = "http://ozo.vn/";
                            newsItem.SiteId = ConfigWeb.OzoId;
                            newsItem.DistrictId = DistrictId;
                            newsItem.ProvinceId = 1;
                            newsItem.DateOld = DateTime.Now;
                            newsItem.IsSpam = false;
                            newsItem.IsUpdated = false;
                            newsItem.IsDeleted = false;
                            newsItem.IsPhone = false;
                            newsItem.IsRepeat = count > 0 ? true : false;
                            newsItem.Phone = Phone;
                            newsItem.Price = string.IsNullOrEmpty(Price.ToString()) ? 0 : Convert.ToDecimal(Price);
                            newsItem.PriceText = Utils.ConvertPrice(Price.ToString());
                            newsItem.IsOwner = false;
                            newsItem.PageView = 0;
                            newsItem.CreatedOn = DateTime.Now;
                            newsItem.CreatedBy = UserId;
                            newsItem.StatusId = 1;
                            newsItem.TotalRepeat = count + 1;
                            resp = _newsbussiness.Createnew(newsItem, UserId);
                            return Json(new
                            {
                                status = "200",
                                errorcode = "0",
                                message = "success",
                                data = resp
                            });
                        }
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = 2
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

        [HttpPost]
        public JsonResult CreateCustomer(string username, string password, string fullname, string sign)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(sign) || string.IsNullOrEmpty(fullname))
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
                    param.Add("fullname", fullname);
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
                        var u = new User
                        {
                            UserName = username,
                            FullName = fullname,
                            CreatedOn = DateTime.Now,
                            Password = Common.Common.md5(username.Trim() + "ozo" + password.Trim()),
                            Sex = true,
                            Phone = username,
                            Email = "",
                            IsDeleted = false,
                            IsMember = true,
                            IsFree = false
                        };

                        var rs = _accountbussiness.Insert(u);
                        if (rs == 0)
                        {
                            return Json(new
                            {
                                status = "200",
                                errorcode = "250",
                                message = "User is existed.",
                                data = ""
                            });
                        }

                        var userItem = _accountbussiness.GetUserDetail(u.Id);
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
        public JsonResult RegisterPackage(int userId, long payment, string sign)
        {
            try
            {
                if (string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(payment.ToString()) || string.IsNullOrEmpty(sign))
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
                    param.Add("userId", userId.ToString());
                    param.Add("payment", payment.ToString());
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
                        string message = "";
                        var rs = new PaymentBussiness().UpdatePaymentAccepted(payment, userId);
                        if (rs == 1) message = "Bạn chưa nạp tiền. Vui lòng liên hệ admin để nạp tiền.";
                        else if (rs == 2) message = "Tài khoản của quý khách không đủ tiền.";
                        else message = "Nạp tiền thành công.";
                        return Json(new
                        {
                            status = "200",
                            errorcode = rs,
                            message = message,
                            data = userId
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
        public JsonResult Recharge(string telco, string pin, string serial, string sign)
        {
            try
            {
                if (string.IsNullOrEmpty(telco) || string.IsNullOrEmpty(serial) || string.IsNullOrEmpty(pin) || string.IsNullOrEmpty(sign))
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
                    param.Add("telco", telco);
                    param.Add("pin", pin);
                    param.Add("serial", serial);
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

                        var resultObj = new RechargeModel();
                        if (!isValidCode(telco, pin))
                        {
                            return Json(new
                            {
                                status = "200",
                                errorcode = "3000",
                                message = "Mã thẻ không đúng. vui lòng thử lại",
                                data = ""
                            });
                        }
                        //get url
                        string url = ConfigWeb.Api_Charging;
                        //get accesskey
                        string accesskey = ConfigWeb.Access_Key;
                        string requestUrl = url + "?accesskey=" + accesskey + "&serial=" + serial + "&pin=" + pin + "&type=" + telco;
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
                                string message = "";
                                var rs = new PaymentBussiness().UpdatePaymentAccepted(amount, Convert.ToInt32(Session["SS-USERID"]));
                                if (rs == 1) message = "Bạn chưa nạp tiền. Vui lòng liên hệ admin để nạp tiền.";
                                else if (rs == 2) message = "Tài khoản của quý khách không đủ tiền.";
                                else message = "Bạn đã nạp thành công " + string.Format("{0:n0}", amount) + "vnđ !Cảm ơn bạn đã sử dụng phần mềm.";
                                return Json(new
                                {
                                    status = "200",
                                    errorcode = rs,
                                    message = message,
                                    data = ""
                                });
                            }

                            catch (Exception)
                            {
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                status = "200",
                                errorcode = "3100",
                                message = "Nạp thẻ thất bại",
                                data = ""
                            });

                        }
                        return Json(resultObj, JsonRequestBehavior.AllowGet);
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

        private bool isValidCode(string telco, string code)
        {
            bool result = true;
            try
            {
                switch (telco)
                {
                    case "VTT":
                        if (code.Length < 13 || code.Length < 11)
                            result = false;
                        break;
                    case "VNP":
                        if (code.Length < 12 || code.Length < 9)
                            result = false;
                        break;
                    case "VMS":
                        if (code.Length < 12 || code.Length < 9)
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

        [HttpPost]
        public JsonResult GetPayment()
        {
            Payment rs = new Payment();
            rs.DayPackage = ConfigWeb.DayPackage;
            rs.MonthPackage = ConfigWeb.MonthPackage;

            return Json(new
            {
                status = "200",
                errorcode = "0",
                message = "success",
                data = rs
            });
        }

        [HttpPost]
        public JsonResult GetListCustomer(string search, int managerId, int statusId, int pageIndex, int pageSize, string sign)
        {
            try
            {
                if (string.IsNullOrEmpty(managerId.ToString()) || string.IsNullOrEmpty(sign) || string.IsNullOrEmpty(statusId.ToString()))
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
                    param.Add("search", search);
                    param.Add("managerId", managerId.ToString());
                    param.Add("statusId", statusId.ToString());
                    param.Add("pageIndex", pageIndex.ToString());
                    param.Add("pageSize", pageSize.ToString());
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
                        int pageTotal = 0;
                        double total = 0;
                        var allAdmin = new UserBussiness().GetAdminUser();
                        var allUser = new UserBussiness().GetCustomerUser(ref total, ref pageTotal, managerId, statusId, pageIndex, pageSize, search);
                        var allRoles = new RoleBussiness().GetRoles();
                        var allRolesUser = new RoleBussiness().GetAllRoleUser();
                        CustomerModel model = new CustomerModel();

                        var rs = (from a in allUser
                                  select new UserModel
                                  {
                                      Id = a.Id,
                                      FullName = a.FullName,
                                      UserName = a.UserName,
                                      Phone = a.Phone,
                                      Email = a.Email,
                                      IsDelete = a.IsDelete,
                                      IsMember = a.IsMember,
                                      ManagerBy = a.ManagerId != 0 ? allAdmin.Where(x => x.Id == a.ManagerId).Select(x => x.FullName).FirstOrDefault() : "",
                                      RoleName = getNameRole(allRoles, allRolesUser, a.Id),
                                      EndTimePayment = getPaymentStatus(a.Id)
                                  }).OrderBy(x => x.IsOnline ? false : true).ThenBy(x => x.EndTimePayment).ToList();
                        model.Total = total;
                        model.pageIndex = pageIndex;
                        model.pageSize = pageSize;
                        model.ListCustomer = rs;
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
        public JsonResult CustomerDetail(int id, string sign)
        {
            try
            {
                if (string.IsNullOrEmpty(id.ToString()))
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
                    param.Add("id", id.ToString());
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
                        CustomerDetail cusDetail = new CustomerDetail();
                        cusDetail = getCustomerDetail(id);
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = cusDetail
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
        public JsonResult GetPaymentStatus()
        {
            List<SelectListItem> lstPayment = getPaymentStatus();
            return Json(new
            {
                status = "200",
                errorcode = "0",
                message = "success",
                data = lstPayment
            });
        }

        [HttpPost]
        public JsonResult GetManagerList()
        {
            List<SelectListItem> lstManager = new UserBussiness().GetManagerUser();
            return Json(new
            {
                status = "200",
                errorcode = "0",
                message = "success",
                data = lstManager
            });
        }

        [HttpPost]
        public JsonResult NoticeList(int userId, int page, Boolean isUser, string sign)
        {
            try
            {
                if (string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(page.ToString()))
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
                    param.Add("userId", userId.ToString());
                    param.Add("page", page.ToString());
                    param.Add("isUser", isUser.ToString());
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
                        List<NoticeDetailModel> notice = new List<NoticeDetailModel>();
                        notice = getAllNoticeById(page, userId, isUser);
                        return Json(new
                        {
                            status = "200",
                            errorcode = "0",
                            message = "success",
                            data = notice
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

        private List<NoticeDetailModel> getAllNoticeById(int page, int userId, Boolean isUser)
        {
            var listNotice = new NotifyBussiness().GetAllNotice(isUser, userId, page);
            var rs = (from a in listNotice
                      select new NoticeDetailModel
                      {
                          Id = a.Id,
                          UserId = a.Userid ?? 0,
                          DateSend = a.DateSend ?? DateTime.Now,
                          UserName = a.UserName,
                          Title = a.Title,
                          Type = a.Type ?? 0,
                          Description = a.Description
                      }).ToList();
            return rs;
        }

        private List<SelectListItem> getPaymentStatus()
        {
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem { Text = "Tất cả", Value = "0" });
            status.Add(new SelectListItem { Text = "Đang hoạt động", Value = "1" });
            status.Add(new SelectListItem { Text = "Sắp hết hạn", Value = "2" });
            status.Add(new SelectListItem { Text = "Đã hết hạn", Value = "3" });
            status.Add(new SelectListItem { Text = "Chưa có gói cước", Value = "4" });
            return status;
        }

        private string getNameRole(List<Role> role, List<Role_User> roleUser, int userId)
        {
            var rs = (from r in role
                      join ru in roleUser on r.Id equals ru.RoleId
                      where ru.UserId == userId
                      select r.Name).ToArray();
            return String.Join(", ", rs);
        }

        private DateTime getPaymentStatus(int userId)
        {
            return new PaymentBussiness().GetEndTimeByUserId(userId);
        }

        private CustomerDetail getCustomerDetail(int id)
        {
            // get all admin
            var allAdmin = new UserBussiness().GetAdminUser();

            var cusDetail = new UserBussiness().GetUserById(id);
            var rs = new CustomerDetail();
            rs.UserId = cusDetail.Id;
            rs.UserName = cusDetail.UserName;
            rs.FullName = cusDetail.FullName;
            rs.LastLogin = cusDetail.LastActivityDate ?? DateTime.Now;
            rs.CreateDate = cusDetail.CreatedOn ?? DateTime.Now;
            rs.ManagerBy = cusDetail.ManagerBy != null ? allAdmin.Where(x => x.Id == cusDetail.ManagerBy).Select(x => x.FullName).FirstOrDefault() : "";
            rs.Notes = cusDetail.Notes;
            //get paymen by Id
            rs.Amount = new PaymentBussiness().GetCashPaymentByUserId(cusDetail.Id);
            rs.TimeEnd = new PaymentBussiness().GetTimePaymentByUserId(cusDetail.Id);
            //get payment by Id 
            var payment = new PaymentBussiness().GetPaymentByUserId(cusDetail.Id);
            rs.CashPayment = payment.FirstOrDefault(x => x.PaymentMethodId == 1) != null ? payment.FirstOrDefault(x => x.PaymentMethodId == 1).AmoutPayment : "0";
            rs.CardPayment = payment.FirstOrDefault(x => x.PaymentMethodId == 2) != null ? payment.FirstOrDefault(x => x.PaymentMethodId == 2).AmoutPayment : "0";
            return rs;
        }

        private Boolean CheckUserLogin(int userId)
        {
            var currentApp = System.Web.HttpContext.Current.Application["usr_" + userId];

            if (currentApp != null)
            {
                if (System.Web.HttpContext.Current.Application["usr_" + userId].Equals("true"))
                {
                    return false;
                }
            }
            //storing session to login at sametime
            System.Web.HttpContext.Current.Application["usr_" + userId] = "true";

            return true;
        }

        private class Payment
        {
            public string DayPackage { get; set; }
            public string MonthPackage { get; set; }
        }
    }
}