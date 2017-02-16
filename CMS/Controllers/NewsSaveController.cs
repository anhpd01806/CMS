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
    public class NewsSaveController : BaseAuthedController
    {
        #region member
        private readonly NewsBussiness _newsbussiness = new NewsBussiness();
        private readonly HomeBussiness _homebussiness = new HomeBussiness();
        #endregion
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
                var listCategory = _homebussiness.GetListCategory();
                var cateListItems = new List<SelectListItem>();
                cateListItems.Add(new SelectListItem { Text = "Chọn chuyên mục", Value = "0" });
                foreach (var item in listCategory)
                {
                    if (item.ParentCategoryId == 0)
                    {
                        cateListItems.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                        var listchillcate = _homebussiness.GetChilldrenlistCategory(item.ParentCategoryId);
                        foreach (var chill in listchillcate)
                        {
                            cateListItems.Add(new SelectListItem { Text = (item.Name + " >> " + chill.Name), Value = chill.Id.ToString() });
                        }
                    }
                }
                model.ListCategory = new SelectList(cateListItems, "Value", "Text");
                #endregion
                #region Get select list district

                var listDistrict = _homebussiness.GetListDistric();
                var listdictrictItem = new List<SelectListItem>();
                listdictrictItem.Add(new SelectListItem { Text = "Chọn quận huyện", Value = "0" });
                foreach (var item in listDistrict)
                {
                    listdictrictItem.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }
                model.ListDistric = new SelectList(listdictrictItem, "Value", "Text");
                #endregion
                #region Get select list site
                var listSite = _homebussiness.GetListSite();
                var listsiteItem = new List<SelectListItem>();
                listsiteItem.Add(new SelectListItem { Text = "Tất cả", Value = "0" });
                foreach (var item in listSite)
                {
                    listsiteItem.Add(new SelectListItem { Text = item.Name, Value = item.ID.ToString() });
                }
                model.ListSite = new SelectList(listsiteItem, "Value", "Text");
                #endregion
                #region Get select list status
                var listStatus = _homebussiness.GetlistStatusModel();
                var listStatusItem = new List<SelectListItem>();
                listStatusItem.Add(new SelectListItem { Text = "Tất cả", Value = "0" });
                foreach (var item in listStatus)
                {
                    listStatusItem.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }
                model.ListStatus = new SelectList(listStatusItem, "Value", "Text");
                #endregion

                model.ListNew = _newsbussiness.GetListNewStatusByFilter(userId, 0, 0, 0, 0, -1, string.Empty, string.Empty, 0, -1, model.pageIndex, model.pageSize,1, ref total);
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
                var listNews = _newsbussiness.GetListNewStatusByFilter(userId, cateId, districtId, newTypeId, siteId, backdate, string.Empty, string.Empty, 0, -1, pageIndex, pageSize,1, ref total);
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

        public ActionResult ExportExcel(int cateId, int districtId, int newTypeId, int siteId, int backdate, decimal
                minPrice, decimal maxPrice, string from, string to, int pageIndex, int pageSize)
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
            var listNews = _newsbussiness.GetListNewStatusByFilter(userId, cateId, districtId, newTypeId, siteId, backdate, string.Empty, string.Empty, 0, -1, pageIndex, pageSize,1, ref total);
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
                var worksheet = xlPackage.Workbook.Worksheets.Add("Danh sách tin tức được lưu");
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                //Create Headers and format them
                var properties = new string[]
                    {
                        "STT",
                        "Tiêu đề",
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

                    worksheet.Cells[row, col].Value = item.Title;
                    col++;

                    worksheet.Cells[row, col].Value = item.DistictName;
                    col++;

                    worksheet.Cells[row, col].Value = Convert.ToDateTime(item.CreatedOn).ToString("dd-MM-yyy");
                    col++;

                    worksheet.Cells[row, col].Value = item.PriceText;
                    col++;

                    worksheet.Cells[row, col].Value = item.Phone;
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

        [HttpPost]
        public JsonResult UserRemoveNewsSave(int[] listNewsId)
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
                    result = _newsbussiness.RemoveSaveNewByUserId(listItem, userId);
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