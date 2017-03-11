﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.Models;
using Elmah;
using OfficeOpenXml;
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
        private readonly CacheNewsBussiness _cacheNewsBussiness = new CacheNewsBussiness();
        #endregion

        // GET: Home 
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
                var listCategory = _cacheNewsBussiness.GetListCategory();
                var cateListItems = new List<SelectListItem>();
                cateListItems.Add(new SelectListItem { Text = "Chọn chuyên mục", Value = "0" });
                foreach (var item in listCategory)
                {
                    if (item.ParentCategoryId == 0)
                    {
                        cateListItems.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                        var listchillcate = _cacheNewsBussiness.GetChilldrenlistCategory(item.Id);
                        foreach (var chill in listchillcate)
                        {
                            cateListItems.Add(new SelectListItem { Text = ("\xA0\xA0\xA0" + chill.Name), Value = chill.Id.ToString() });
                        }
                    }
                }
                model.ListCategory = new SelectList(cateListItems, "Value", "Text");
                #endregion
                #region Get select list district

                var listDistrict = _cacheNewsBussiness.GetListDistric();
                var listdictrictItem = new List<SelectListItem>();
                listdictrictItem.Add(new SelectListItem { Text = "Chọn quận huyện", Value = "0" });
                foreach (var item in listDistrict)
                {
                    listdictrictItem.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }
                model.ListDistric = new SelectList(listdictrictItem, "Value", "Text");
                #endregion
                #region Get select list site
                var listSite = _cacheNewsBussiness.GetListSite();
                var listsiteItem = new List<SelectListItem>();
                listsiteItem.Add(new SelectListItem { Text = "Tất cả", Value = "0" });
                foreach (var item in listSite)
                {
                    listsiteItem.Add(new SelectListItem { Text = item.Name, Value = item.ID.ToString() });
                }
                model.ListSite = new SelectList(listsiteItem, "Value", "Text");
                #endregion
                #region Get select list status
                var listStatus = _cacheNewsBussiness.GetlistStatusModel();
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
                model.ListNew = _bussiness.GetListNewByFilter(userId, 0, 0, 0, 0, -1, string.Empty, string.Empty, 0, -1, model.pageIndex, model.pageSize, false, string.Empty, string.Empty, false, ref total);
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
        public JsonResult LoadData(int cateId, int districtId, int newTypeId, int siteId, int backdate, double
                minPrice, double maxPrice, string from, string to, int pageIndex, int pageSize, int IsRepeat, string NameOrder, bool descending, string key)
        {
            try
            {
                int userId = Convert.ToInt32(Session["SS-USERID"]);
                int total = 0;
                var listNews = _bussiness.GetListNewByFilter(userId, cateId, districtId, newTypeId, siteId, backdate, from, to, minPrice, maxPrice, pageIndex, pageSize, Convert.ToBoolean(IsRepeat), key, NameOrder, descending, ref total);
                ViewBag.Accept = Convert.ToBoolean(Session["USER-ACCEPTED"]);
                var content = RenderPartialViewToString("~/Views/Home/Paging.cshtml", listNews);
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
                    var news = _cacheNewsBussiness.GetNewsDetail(Id, userId);
                    ViewBag.RoleId = _cacheNewsBussiness.GetRoleByUser(userId);
                    var content = RenderPartialViewToString("~/Views/Home/NewDetail.cshtml", news);
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
        public JsonResult NewsForUser(int[] listNewsId)
        {
            try
            {
                int userId = Convert.ToInt32(Session["SS-USERID"]);
                var result = 1;
                if (listNewsId.Length > 0)
                {
                    result = _bussiness.NewsforUser(listNewsId, userId);
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
        public JsonResult RemoveNewsForUser(int[] listNewsId)
        {
            try
            {
                int userId = Convert.ToInt32(Session["SS-USERID"]);
                var result = 1;
                if (listNewsId.Length > 0)
                {
                    result = _bussiness.RemoveNewsforUser(listNewsId, userId);
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

        public ActionResult ExportExcel(int cateId, int districtId, int newTypeId, int siteId, int backdate, decimal
                minPrice, decimal maxPrice, string from, string to, int pageIndex, int pageSize, int IsRepeat, string NameOrder, bool descending, string key)
        {

            string fileName = string.Format("News_{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            string filePath = Path.Combine(Request.PhysicalApplicationPath, "File\\ExportImport", fileName);
            var folder = Request.PhysicalApplicationPath + "File\\ExportImport";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            int total = 0;
            int userId = Convert.ToInt32(Session["SS-USERID"]);
            var listNews = _bussiness.GetListNewByFilter(userId, cateId, districtId, newTypeId, siteId, backdate, string.Empty, string.Empty, 0, -1, pageIndex, pageSize, Convert.ToBoolean(IsRepeat), key, NameOrder, descending, ref total);
            ExportToExcel(filePath, listNews);

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "text/xls", fileName);
        }

        public ActionResult ExportExcelV2(string listNewsId)
        {
            var newsId = new List<int>();
            for (int i = 0; i < listNewsId.Split(',').Length; i++)
            {
                newsId.Add(Convert.ToInt32(listNewsId.Split(',')[i]));
            }

            string fileName = string.Format("News_{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            string filePath = Path.Combine(Request.PhysicalApplicationPath, "File\\ExportImport", fileName);
            var folder = Request.PhysicalApplicationPath + "File\\ExportImport";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var listNews = _bussiness.ExportExcel(newsId);
            ExportToExcel(filePath, listNews);

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "text/xls", fileName);
        }

        public virtual void ExportToExcel(string filePath, IList<NewsModel> listnews)
        {
            var newFile = new FileInfo(filePath);

            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(newFile))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Danh sách tin tức");
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                //Create Headers and format them
                var properties = new string[]
                    {
                        "STT",
                        "Nội dung",
                        "Quận huyện",
                        "Ngày đăng",
                        "Giá",
                        "Điện thoại",
                        "Loại tin"
                    };
                for (var i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                var row = 2;
                var dem = 0;
                foreach (var item in listnews)
                {
                    dem++;
                    int col = 1;

                    worksheet.Cells[row, col].Value = dem;
                    col++;

                    worksheet.Cells[row, col].Value = Convert.ToBoolean(Session["USER-ACCEPTED"]) ? (item.Title + "\r\n" + CMS.Helper.Utils.RemoveHtml(item.Contents).Trim()) : "Vui lòng nạp tiền";
                     worksheet.Cells[row, col].Style.WrapText = true;
                    col++;


                    worksheet.Cells[row, col].Value = Convert.ToBoolean(Session["USER-ACCEPTED"]) ? item.DistictName : "Vui lòng nạp tiền";
                    col++;

                    worksheet.Cells[row, col].Value = Convert.ToDateTime(item.CreatedOn).ToString("dd-MM-yyy");
                    col++;

                    worksheet.Cells[row, col].Value = item.PriceText;
                    col++;

                    worksheet.Cells[row, col].Value = Convert.ToBoolean(Session["USER-ACCEPTED"]) ? item.Phone : "Vui lòng nạp tiền";
                    col++;

                    worksheet.Cells[row, col].Value = item.StatusName;
                    col++;
                    //next row
                    row++;
                }


                var nameexcel = "Danh sách tin tức" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                xlPackage.Workbook.Properties.Title = string.Format("{0}", nameexcel);
                xlPackage.Workbook.Properties.Author = "Admin-IT";
                xlPackage.Workbook.Properties.Subject = string.Format("{0} TINTUC", "");
                xlPackage.Workbook.Properties.Category = "TINTUC";

                xlPackage.Workbook.Properties.Company = "OZO";
                xlPackage.Save();
            }
        }

        public JsonResult ReportNews(int[] listNewsId)
        {
            try
            {
                int userId = Convert.ToInt32(Session["SS-USERID"]);
                var result = 1;
                List<Notify> lstNotify = null;
                if (listNewsId.Length > 0)
                {
                    var listItem = new List<New>();
                    for (int i = 0; i < listNewsId.Length; i++)
                    {
                        var news = _bussiness.GetNewsDetail(listNewsId[i]);
                        listItem.Add(news);
                    }

                    result = _bussiness.ReportNews(listItem, userId);
                    //save notify
                    var userInfo = (User)Session["SS-USER"];
                    string userName = Session["SS-FULLNAME"] != null ? Session["SS-FULLNAME"].ToString() : userInfo.FullName != null ? userInfo.FullName : userInfo.UserName;
                    lstNotify = new NotifyBussiness().ReportNews(listItem, userId, userName);
                }
                return Json(lstNotify);
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return Json(new { Status = 0 });
            }
        }

        public JsonResult NewsSpam(int[] listNewsId)
        {
            try
            {
                int userId = Convert.ToInt32(Session["SS-USERID"]);
                var result = 1;
                if (listNewsId.Length > 0)
                {
                    var listItem = new List<New>();
                    for (int i = 0; i < listNewsId.Length; i++)
                    {
                        var news = _bussiness.GetNewsDetail(listNewsId[i]);
                        listItem.Add(news);
                    }
                    result = _bussiness.Spam(listItem, userId);
                }
                return Json(new { Status = result });
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return Json(new { Status = 0 });
            }
        }

        public JsonResult Delete(int[] listNewsId)
        {
            try
            {
                int userId = Convert.ToInt32(Session["SS-USERID"]);
                var result = 1;
                if (listNewsId.Length > 0)
                {
                    result = _bussiness.Delete(listNewsId, userId);
                }
                else
                {
                    result = 2;
                }
                return Json(new { Status = result });
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return Json(new { Status = 0 });
            }
        }

        /// <summary>
        /// Author: Phạm Đức Anh
        /// Báo cáo tin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DangerNewsReport(int id, string reason)
        {
            try
            {
                ReasonReportNew model = new ReasonReportNew();
                model.NewsId = id;
                model.Note = reason;
                model.UserId = Convert.ToInt32(Session["SS-USERID"]);
                model.DateCreate = DateTime.Now;
                new HomeBussiness().InsertReasonReportNews(model);
                return Json(new { Messages = reason });
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return Json(new { Status = 0 });
            }
        }


        /// <summary>
        /// Author: Phạm Đức Anh
        /// Hủy báo tin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CancerDangerNewsReport(int id)
        {
            try
            {
                int userId = Convert.ToInt32(Session["SS-USERID"]);
                new HomeBussiness().DeleteReasonReportNews(id, userId);
                return Json(new { Messages = "Xóa thành công" });
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return Json(new { Status = 0 });
            }
        }
    }
}