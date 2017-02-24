using CMS.Bussiness;
using CMS.Data;
using CMS.ViewModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBackendPlus.Controllers;

namespace CMS.Controllers
{
    public class BlackListController : BaseAuthedController
    {
        private int PageSize = 20;
        // GET: BlackList
        public ActionResult Index()
        {
            BlackListViewModel model = new BlackListViewModel();
            return View(model);
        }

        public ActionResult ExportExcel(string listBlackListId)
        {
            string fileName = string.Format("BlackList_{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            string filePath = Path.Combine(Request.PhysicalApplicationPath, "File\\ExportImport", fileName);
            var folder = Request.PhysicalApplicationPath + "File\\ExportImport";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            int userId = Convert.ToInt32(Session["SS-USERID"]);
            var listBlackList = GetBlackListForExcel(listBlackListId);
            ExportToExcel(filePath, listBlackList);

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "text/xls", fileName);
        }

        #region Json
        [HttpPost]
        public JsonResult InsertData(string Phone, string Description,string LinkUrl)
        {
            try
            {
                var blackList = new Blacklist
                {
                    Words = Phone,
                    Description = Description,
                    CreatedOn = DateTime.Now,
                    LinkUrl = LinkUrl,
                    Type = 1
                };

                new BlackListBussiness().Insert(blackList);

                return Json(new
                {
                    Result = true,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    Result = false,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult LoadData(string search, int pageIndex)
        {
            try
            {
                int totalpage = 0;
                BlackListViewModel model = new BlackListViewModel();
                model.BlackList = GetBlackListLink(ref totalpage, search, PageSize, pageIndex);
                var content = RenderPartialViewToString("~/Views/BlackList/BlackListDetail.cshtml", model.BlackList);
                model.Totalpage = totalpage;
                return Json(new
                {
                    TotalPage = model.Totalpage,
                    Content = content
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    TotalPage = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        public JsonResult DeleteData(string id)
        {
            try
            {
                new BlackListBussiness().Delete(id.ToString());
                return Json(new
                {
                    Result = true,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    Result = false,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region PrivateFuntion

        public virtual void ExportToExcel(string filePath, IList<BlacklistModel> Blacklist)
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
                        "Số điện thoại",
                        "Mô tả"
                    };
                for (var i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                var row = 2;
                var dem = 0;
                foreach (var item in Blacklist)
                {
                    dem++;
                    int col = 1;

                    worksheet.Cells[row, col].Value = dem;
                    col++;

                    worksheet.Cells[row, col].Value = item.Words;
                    col++;

                    worksheet.Cells[row, col].Value = item.Description;
                    col++;
                    //next row
                    row++;
                }


                var nameexcel = "Danh sách blacklist" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                xlPackage.Workbook.Properties.Title = string.Format("{0}", nameexcel);
                xlPackage.Workbook.Properties.Author = "Admin-IT";
                xlPackage.Workbook.Properties.Subject = string.Format("{0} BLACKLIST", "");
                xlPackage.Workbook.Properties.Category = "BLACKLIST";

                xlPackage.Workbook.Properties.Company = "OZO";
                xlPackage.Save();
            }
        }

        private List<BlacklistModel> GetBlackListLink(ref int totalPage, string search, int pageSize, int pageIndex)
        {
            int totalCount = 0;
            var blackList = new BlackListBussiness().GetBlackListByParam(ref totalCount, search, pageSize, (pageIndex - 1));
            var rs = (from a in blackList
                      select new BlacklistModel
                      {
                          Id = a.Id,
                          Words = a.Words,
                          Description = a.Description,
                          LinkUrl = a.LinkUrl,
                          CreatedOn = a.CreatedOn,
                          Type = a.Type
                      }).ToList();
            totalPage = (int)Math.Ceiling((double)totalCount / (double)pageSize);
            return rs;
        }

        private List<BlacklistModel> GetBlackListForExcel(string listBlackListId)
        {
            var blackList = new BlackListBussiness().GetBlackListForExcel(listBlackListId);
            var rs = (from a in blackList
                      select new BlacklistModel
                      {
                          Id = a.Id,
                          Words = a.Words,
                          Description = a.Description,
                          LinkUrl = a.LinkUrl,
                          CreatedOn = a.CreatedOn,
                          Type = a.Type
                      }).ToList();
            return rs;
        }
        #endregion
    }
}