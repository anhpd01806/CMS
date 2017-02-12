using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elmah;
using CMS.Bussiness;
using WebBackendPlus.Controllers;
using CMS.ViewModel;
using CMS.Helper;
using CMS.Data;

namespace CMS.Controllers
{
    public class HomeController : BaseAuthedController
    {
        #region member
        private readonly HomeBussiness _bussiness = new HomeBussiness();
        #endregion

        // GET: Home
        public ActionResult Index()
        {
            try
            {
                int total = 0;
                var model = new HomeViewModel();
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
                        var listchillcate = _bussiness.GetChilldrenlistCategory(item.ParentCategoryId);
                        foreach (var chill in listchillcate)
                        {
                            cateListItems.Add(new SelectListItem { Text = (item.Name + " >> " + chill.Name), Value = chill.Id.ToString() });
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

                model.ListNew = _bussiness.GetListNewByFilter(userId, 0, 0, 0, 0, -1, string.Empty, string.Empty, 0, -1, model.pageIndex, model.pageSize, ref total);
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

        [HttpPost]
        public JsonResult LoadData(int cateId, int districtId, int newTypeId, int siteId, int backdate, decimal
                minPrice, decimal maxPrice, string from, string to, int pageIndex, int pageSize)
        {
            try
            {
                int userId = Convert.ToInt32(Session["SS-USERID"]);
                int total = 0;
                var listNews = _bussiness.GetListNewByFilter(userId, cateId, districtId, newTypeId, siteId, backdate, string.Empty, string.Empty, 0, -1, pageIndex, pageSize, ref total);
                var content = RenderPartialViewToString("~/Views/Home/Paging.cshtml", listNews);
                return Json(new
                {
                    TotalPage = (int)Math.Ceiling((double)total / (double)pageSize),
                    Content = content
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return Json(new
                {
                    TotalPage = 0,
                    Content = "<tr><td colspan='8'>Hệ thống gặp sự cố trong quá trình load dữ liệu<td></tr>"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetNewsDetail()
        {
            try
            {
                var Id = Convert.ToInt32(Request["Id"]);
                var news = _bussiness.GetNewsDetail(Id);
                var content = RenderPartialViewToString("~/Views/Home/NewDetail.cshtml", news);
                return Json(new
                {
                    Content = content
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return Json(new
                {
                    Content = string.Empty
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UserSaveNews(int[] listNewsId)
        {
            try
            {
                int userId = Convert.ToInt32(Session["SS-USERID"]);
                var result = 1;
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
                    result = _bussiness.SaveNewByUserId(listItem, userId);
                }
                return Json(new { Status = result });
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return Json(new { Status = 0 });
            }
        }

        [HttpPost]
        public JsonResult UserHideNews(int[] listNewsId)
        {
            try
            {
                int userId = Convert.ToInt32(Session["SS-USERID"]);
                var result = 1;
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
                    result = _bussiness.HideNewByUserId(listItem, userId);
                }
                return Json(new { Status = result });
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return Json(new { Status = 0 });
            }
        }

        
    }
}