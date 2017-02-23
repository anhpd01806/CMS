﻿using CMS.Bussiness;
using CMS.Data;
using CMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBackendPlus.Controllers;

namespace CMS.Controllers
{
    public class ReportNewsController : BaseAuthedController
    {
        // GET: ReportNews
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadData()
        {
            try
            {
                ReportNewsViewModel model = new ReportNewsViewModel();
                model = new ReportNewsBussiness().GetRandomNewsReport(0);
                var content = RenderPartialViewToString("~/Views/ReportNews/ReportNewsDetail.cshtml", model.NewsReportList);
                var firstRandom = RenderPartialViewToString("~/Views/ReportNews/FirstRandomReports.cshtml", model.FirstRandomNewsReport);
                return Json(new
                {
                    Content = content,
                    FirstRandom = firstRandom
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteReportNews(int id)
        {
            try
            {
                var check = new ReportNewsBussiness().DeleteReportNews(id);
                if (check)
                {
                    //get news by id
                    var news = new NewsBussiness().GetNewsById(id);
                    //insert phone into blacklist

                    var model = new Blacklist
                    {
                        Words = news.Phone,
                        Description = news.Title,
                        CreatedOn = DateTime.Now,
                        Type = 1
                    };
                    new BlackListBussiness().Insert(model);

                    return Json(new
                    {
                        Result = true
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    Result = false
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Result = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CancelReportNews(int id)
        {
            try
            {
                var check = new ReportNewsBussiness().DeleteReportNews(id);
                if (check)
                    return Json(new
                    {
                        Result = true
                    }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new
                    {
                        Result = false
                    }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    Result = false
                }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ChangeFirstRandom(int id)
        {
            try
            {
                ReportNewsViewModel model = new ReportNewsViewModel();
                model = new ReportNewsBussiness().GetRandomNewsReport(id);
                var content = RenderPartialViewToString("~/Views/ReportNews/ReportNewsDetail.cshtml", model.NewsReportList);
                var firstRandom = RenderPartialViewToString("~/Views/ReportNews/FirstRandomReports.cshtml", model.FirstRandomNewsReport);
                return Json(new
                {
                    Content = content,
                    FirstRandom = firstRandom
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}