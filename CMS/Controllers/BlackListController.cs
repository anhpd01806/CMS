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
        private int PageSize = 100;
        // GET: BlackList
        public ActionResult Index()
        {
            BlackListViewModel model = new BlackListViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;

                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            var black = new Blacklist();
                            try
                            {
                                try
                                {
                                    black.Words = workSheet.Cells[rowIterator, 2].Value.ToString();
                                }
                                catch (Exception)
                                {
                                    string test = rowIterator.ToString();
                                }

                                try
                                {
                                    black.Description = workSheet.Cells[rowIterator, 3].Value.ToString();
                                }
                                catch (Exception)
                                {

                                    black.Description = "";
                                }
                                black.CreatedOn = DateTime.Now;
                                try
                                {
                                    black.LinkUrl = workSheet.Cells[rowIterator, 4].Value.ToString();
                                }
                                catch (Exception)
                                {

                                    black.LinkUrl = "";
                                }

                                black.Type = 1;
                                new BlackListBussiness().Insert(black);
                            }
                            catch (Exception ex)
                            {
                                string test = black.Words;
                                TempData["Error"] = "Có một hoặc nhiều bản ghi đang sai định dạng.Lưu ý: Số điện thoại không được để trống";
                            }
                        }
                    }
                    TempData["Success"] = "Import dữ liệu thành công";
                }
                else
                {
                    TempData["Error"] = "Đường dẫn không chính xác hoặc để trống. vui lòng thử lại";
                }
            }
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
        public JsonResult InsertData(string Phone, string Description, string LinkUrl)
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
                int totalCount = 0;
                BlackListViewModel model = new BlackListViewModel();
                model.BlackList = GetBlackListLink(ref totalCount, ref totalpage, search, PageSize, pageIndex);
                var content = RenderPartialViewToString("~/Views/BlackList/BlackListDetail.cshtml", model.BlackList);
                model.Totalpage = totalpage;
                return Json(new
                {
                    TotalPage = model.Totalpage,
                    totalCount = totalCount,
                    Content = content
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    TotalPage = 0,
                    totalCount = 0
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
                        "Mô tả",
                        "Link Url"
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

                    worksheet.Cells[row, col].Value = item.LinkUrl;
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

        private List<BlacklistModel> GetBlackListLink(ref int totalCount, ref int totalPage, string search, int pageSize, int pageIndex)
        {
            var blackList = new BlackListBussiness().GetBlackListByParam(ref totalCount, search, pageSize, (pageIndex - 1));
            var rs = (from a in blackList
                      select new BlacklistModel
                      {
                          Id = a.Id,
                          Words = a.Words,
                          Description = a.Description,
                          LinkUrl = String.IsNullOrEmpty(a.LinkUrl) ? "" : a.LinkUrl.Replace("//", "/").Split('/')[1].ToString(),
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
                          LinkUrl = String.IsNullOrEmpty(a.LinkUrl) ? "" : a.LinkUrl.Replace("//", "/").Split('/')[1].ToString(),
                          CreatedOn = a.CreatedOn,
                          Type = a.Type
                      }).ToList();
            return rs;
        }
        #endregion
    }
}