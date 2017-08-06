using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBackendPlus.Controllers;
using Elmah;
using OfficeOpenXml;
using CMS.Bussiness;
using CMS.Models;
using System.IO;

namespace CMS.Controllers
{
    public class ExportDataController : BaseAuthedController
    {
        // GET: ExportData
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExportExcel()
        {

            string fileName = string.Format("News_{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            string filePath = Path.Combine(Request.PhysicalApplicationPath, "File\\ExportImport", fileName);
            var folder = Request.PhysicalApplicationPath + "File\\ExportImport";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            int userId = Convert.ToInt32(Session["SS-USERID"]);
            var listNews = new NewsBussiness().ExportDataOzO();
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
                        "Tiêu đề",
                        "Nội dung",
                        "Giá",
                        "Diện tích"
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
                    worksheet.Cells[row, col].Style.WrapText = true;
                    col++;
                    
                    //worksheet.Cells[row, col].Value = CMS.Helper.Utils.RemoveHtml(item.Contents).Trim();
                    //col++;
                    worksheet.Cells[row, col].Value = item.Contents;
                    col++;
                    
                    worksheet.Cells[row, col].Value = item.Price;
                    col++;
                    
                    worksheet.Cells[row, col].Value = item.Area;
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
    }
}