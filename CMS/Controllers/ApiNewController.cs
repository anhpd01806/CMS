﻿using CMS.Data;
using Elmah;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.Controllers
{
    public class ApiNewController : Controller
    {
        // GET: ApiNew
        public ActionResult Index()
        {
            return View();
        }
        #region DucAnh
        [HttpPost]
        public JsonResult InsertNews(int categoryId, string title, string contents, string link, int siteId, int districtId, int provinceId,
                                    DateTime createdOn, string phone, string priceText, decimal area, decimal price)
        {
            try
            {
                var Query = "INSERT INTO News (CategoryId,Title,Contents,Summary,Link,SiteId,DistrictId,ProvinceId,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,StatusId,Phone,IsUpdated,DateOld,IsDeleted,IsPhone,PriceText,IsRepeat,Area,Price,IsSpam,TotalRepeat)" +
                              "VALUES(" + categoryId + ", N'" + title.Replace("'", "") + "', N'" + contents.Replace("'", "") + "', N'" + contents.Replace("'", "") + "', '" + link + "', " + siteId + ", " + districtId +
                              "," + provinceId + "," + 1 + ",'" + createdOn + "'," + 1 + ",'" + (DateTime.Now) + "' ," + 1 + ",'" + phone + "',0,'" + createdOn +
                              "',0," + 1 + ",N'" + priceText + "',0," + area + "," + price + ",0,1);SELECT Convert(Int, @@IDENTITY)";
                int newID = 0;
                using (var dbContext = new CmsDataDataContext())
                {
                    SqlConnection conn = new SqlConnection(dbContext.Connection.ConnectionString);
                    newID = dbContext.ExecuteQuery<int>(Query).First();
                    dbContext.Dispose();
                }
                return Json(new
                {
                    status = "0",
                    errorcode = "0",
                    message = "Success",
                    data = newID
                });
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return Json(new
                {
                    status = "200",
                    errorcode = "5000",
                    message = ex.ToString(),
                    data = ""
                });
            }
        }

        [HttpPost]
        public JsonResult InsertImageNews(int newsId, string imageUrl)
        {
            try
            {
                var queryImage = "INSERT INTO Image (NewsId,ImageUrl)" +
                                   "VALUES(" + newsId + ", '" + imageUrl + "');";
                int imageId;
                using (var dbContext = new CmsDataDataContext())
                {
                    SqlConnection conn = new SqlConnection(dbContext.Connection.ConnectionString);
                    imageId = dbContext.ExecuteQuery<int>(queryImage).First();
                    dbContext.Dispose();
                }
                return Json(new
                {
                    status = "0",
                    errorcode = "0",
                    message = "Success",
                    data = imageId
                });
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return Json(new
                {
                    status = "200",
                    errorcode = "5000",
                    message = ex.ToString(),
                    data = ""
                });
            }
        }
        #endregion
    }
}