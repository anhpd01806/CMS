using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Elmah;
using CMS.Bussiness;
using WebBackendPlus.Controllers;
using CMS.ViewModel;
using CMS.Data;
using CMS.Helper;
namespace CMS.Controllers
{
    public class BrokersInformationController : BaseAuthedController
    {
        #region member
        private readonly HomeBussiness _bussiness = new HomeBussiness();
        private readonly NewsBussiness _newsbussiness = new NewsBussiness();
        #endregion
        public ActionResult Index()
        {
            try
            {
                var model = new HomeViewModel();
                int total = 0;
                model.pageIndex = 1;
                model.pageSize = ConfigWeb.PageSize;
                int userId = Convert.ToInt32(Session["SS-USERID"]);

                #region Get select list category
                var listCategory = _bussiness.GetListCategory();
                var cateListItems = new List<SelectListItem>();
                cateListItems.Add(new SelectListItem { Text = "Chọn chuyên mục", Value = "0" });
                foreach (var item in listCategory)
                {
                    if (item.ParentCategoryId == 0)
                    {
                        cateListItems.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                        var listchillcate = _bussiness.GetChilldrenlistCategory(item.Id);
                        foreach (var chill in listchillcate)
                        {
                            cateListItems.Add(new SelectListItem { Text = ("\xA0\xA0\xA0" + chill.Name), Value = chill.Id.ToString() });
                        }
                    }
                }
                model.ListCategory = new SelectList(cateListItems, "Value", "Text");
                #endregion
                #region Get select list district

                var listDistrict = _bussiness.GetListDistric();
                var listdictrictItem = new List<SelectListItem>();
                listdictrictItem.Add(new SelectListItem { Text = "Chọn quận huyện", Value = "0" });
                foreach (var item in listDistrict)
                {
                    listdictrictItem.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }
                model.ListDistric = new SelectList(listdictrictItem, "Value", "Text");
                #endregion
                #region Get select list site
                var listSite = _bussiness.GetListSite();
                var listsiteItem = new List<SelectListItem>();
                listsiteItem.Add(new SelectListItem { Text = "Tất cả", Value = "0" });
                foreach (var item in listSite)
                {
                    listsiteItem.Add(new SelectListItem { Text = item.Name, Value = item.ID.ToString() });
                }
                model.ListSite = new SelectList(listsiteItem, "Value", "Text");
                #endregion
                #region Get select list status
                var listStatus = _bussiness.GetlistStatusModel();
                var listStatusItem = new List<SelectListItem>();
                listStatusItem.Add(new SelectListItem { Text = "Tất cả", Value = "0" });
                foreach (var item in listStatus)
                {
                    listStatusItem.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }
                model.ListStatus = new SelectList(listStatusItem, "Value", "Text");
                #endregion

                ViewBag.Accept = Convert.ToBoolean(Session["USER-ACCEPTED"]);
                var checkuser = Convert.ToBoolean(string.IsNullOrEmpty(Session["IS-USERS"].ToString()) ? "false" : Session["IS-USERS"]);
                ViewBag.User = checkuser;
                model.ListNew = _newsbussiness.GetListBrokersInformationByFilter(userId, 0, 0, 0, 0, -1, string.Empty, string.Empty, 0, -1, model.pageIndex, model.pageSize, false, string.Empty, ref total);
                model.Total = total; 
                model.Totalpage = (int)Math.Ceiling((double)model.Total / (double)model.pageSize);
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return null;
            }
        }

        public JsonResult LoadData(int cateId, int districtId, int newTypeId, int siteId, int backdate, double
                minPrice, double maxPrice, string from, string to, int pageIndex, int pageSize, int IsRepeat, string key)
        {
            try
            {
                int userId = Convert.ToInt32(Session["SS-USERID"]);
                int total = 0;
                var listNews = _newsbussiness.GetListBrokersInformationByFilter(userId, cateId, districtId, newTypeId, siteId, backdate, from, to, minPrice, maxPrice, pageIndex, pageSize, Convert.ToBoolean(IsRepeat), key, ref total);
                ViewBag.Accept = Convert.ToBoolean(Session["USER-ACCEPTED"]);
                var content = RenderPartialViewToString("~/Views/BrokersInformation/Paging.cshtml", listNews);
                return Json(new
                {
                    TotalPage = (int)Math.Ceiling((double)total / (double)pageSize),
                    Content = content,
                    TotalRecord = total
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return Json(new
                {
                    TotalPage = 0,
                    Content = "<tr><td colspan='9'>Hệ thống gặp sự cố trong quá trình load dữ liệu<td></tr>"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteBlacklist(int[] listNewsId)
        {
            try
            {
                var result = 1;
                if (listNewsId.Length > 0)
                {
                    result = _newsbussiness.DeleteBlacklist(listNewsId);
                }
                return Json(new { Status = result });
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return Json(new { Status = 0 });
            }
        }

        public JsonResult GetNewsDetail()
        {
            try
            {
                if (Convert.ToBoolean(Session["USER-ACCEPTED"]))
                {
                    var Id = Convert.ToInt32(Request["Id"]);
                    int userId = Convert.ToInt32(Session["SS-USERID"]);
                    ViewBag.Accept = Convert.ToBoolean(Session["USER-ACCEPTED"]);
                    ViewBag.User = Convert.ToBoolean(string.IsNullOrEmpty(Session["IS-USERS"].ToString()) ? "false" : Session["IS-USERS"]);
                    var news = _bussiness.GetNewsDetail(Id, userId);
                    ViewBag.RoleId = _bussiness.GetRoleByUser(userId);
                    var content = RenderPartialViewToString("~/Views/BrokersInformation/NewsDetail.cshtml", news);
                    return Json(new
                    {
                        Pay = 1,
                        Content = content
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new
                {
                    Pay = 0,
                    Content = String.Empty
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return Json(new
                {
                    Pay = 1,
                    Content = string.Empty
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}