﻿using System;
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
    public class NewsController : BaseAuthedController
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

                model.ListNew = _newsbussiness.GetListNewNotActiveByFilter(0,0,0,0,-1,string.Empty, string.Empty,0,-1,model.pageIndex, model.pageSize, false, string.Empty, ref total);
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

        public ActionResult Create()
        {
            try
            {
                var model = new HomeViewModel();

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

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return null;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult CreateNews()
        {
            try
            {
                int userId = Convert.ToInt32(Session["SS-USERID"]);
                var title = Request["title"];
                var cateId = Convert.ToInt32(Request["cateId"]);
                var districtId = Convert.ToInt32(Request["districtId"]);
                var phone = Request["phone"];
                var price = Request["price"].Replace(".", "");
                var pricetext = Request["pricetext"];
                var content = Request["content"];

                var newsItem = new New();
                var count = _newsbussiness.CheckRepeatNews(phone, districtId, userId);

                newsItem.CategoryId = cateId;
                newsItem.Title = title;
                newsItem.Contents = content;
                newsItem.Link = "http://ozo.vn/";
                newsItem.SiteId = 0;
                newsItem.DistrictId = districtId;
                newsItem.ProvinceId = 1;
                newsItem.DateOld = DateTime.Now;
                newsItem.IsSpam = false;
                newsItem.IsUpdated = false;
                newsItem.IsDeleted = false;
                newsItem.IsPhone = false;
                newsItem.IsRepeat = count > 0 ? true : false;
                newsItem.Phone = phone;
                newsItem.Price = string.IsNullOrEmpty(price) ? 0 : Convert.ToDecimal(price);
                newsItem.PriceText = pricetext;
                newsItem.IsOwner = false;
                newsItem.PageView = 0;
                newsItem.CreatedOn = DateTime.Now;
                newsItem.CreatedBy = userId;
                newsItem.StatusId = 1;

                var result = _newsbussiness.Createnew(newsItem, userId);
                return Json(new
                {
                    type = result
                });
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return Json(new
                {
                    type = 0
                });
            }
        }
    }
}