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
using CMS.API.Data;
using CMS.API.Helper;

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
                if (string.IsNullOrEmpty(listNewsId.ToString()) || string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(sign.ToString()))
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
                    param.Add("listNewsId", listNewsId.ToString());
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
                if (string.IsNullOrEmpty(listNewsId.ToString()) || string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(sign.ToString()))
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
                    param.Add("listNewsId", listNewsId.ToString());
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
                if (string.IsNullOrEmpty(listNewsId.ToString()) || string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(sign.ToString()))
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
                    param.Add("listNewsId", listNewsId.ToString());
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
                if (string.IsNullOrEmpty(listNewsId.ToString()) || string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(sign.ToString()))
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
                    param.Add("listNewsId", listNewsId.ToString());
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
                if (string.IsNullOrEmpty(listNewsId.ToString()) || string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(sign.ToString()))
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
                    param.Add("listNewsId", listNewsId.ToString());
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
    }
}