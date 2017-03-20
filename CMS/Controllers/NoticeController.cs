using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.Models;
using CMS.Bussiness;
using WebBackendPlus.Controllers;
using CMS.ViewModel;

namespace CMS.Controllers
{
    public class NoticeController : BaseAuthedController
    {
        // GET: Notice
        public ActionResult Index()
        {
            return View();
        }
        // get all notice
        [HttpPost]
        public JsonResult LoadData(int page)
        {
            try
            {
                NoticeViewModel model = new NoticeViewModel();
                model.NoticeList = getAllNoticeById(page);
                var content = RenderPartialViewToString("~/Views/Notice/NoticeDetail.cshtml", model.NoticeList);
                return Json(new
                {
                    Content = content
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    TotalPage = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult LoadMore(int page)
        {
            try
            {
                NoticeViewModel model = new NoticeViewModel();
                model.NoticeList = getAllNoticeById(page);
                return Json(model.NoticeList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        // get nodify by id
        public JsonResult GetNotify(string Id)
        {
            try
            {
                //parse notify id
                int id = int.Parse(Id);
                NoticeModel notify = new NoticeModel();
                notify = new NotifyBussiness().GetNotifyById(id);
                return Json(notify, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        //accept account 
        public JsonResult AcceptAccount(string Id, string UserId)
        {
            try
            {
                //parse user  id
                int userId = int.Parse(UserId);
                //parse notify  id
                int id = int.Parse(Id);
                //update notify status
                new NotifyBussiness().UpdateNotifyStatus(id);
                bool status = (new NotifyBussiness().UpdateUser(userId) == 1);
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        //update notify is viewed
        public JsonResult UpdateNotify(string Id)
        {
            try
            {
                //parse notify  id
                int id = int.Parse(Id);
                if (id != 0)
                {
                    //update notify status
                    new NotifyBussiness().UpdateNotifyView(id);
                }
                //update session
                List<NoticeModel> lstNotify = (List<CMS.Models.NoticeModel>)Session["NotityUser"];
                if (lstNotify != null)
                {
                    Session["NotityUser"] = (from n in lstNotify where n.Id != id select n).ToList();
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateNotifySesion()
        {
            try
            {
                bool isUser = (bool)Session["IS-USERS"];
                int userId = (int)Session["SS-USERID"];
                //update session
                List<NoticeModel> lstNotify = new NotifyBussiness().GetNotify(isUser, userId);
                Session["NotityUser"] = lstNotify;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateStatusShowNotify(Boolean ShowNotify)
        {
            try
            {
                int userId = (int)Session["SS-USERID"];
                //update shownotify status
                new NotifyBussiness().UpdateUser(userId, ShowNotify);
                //update session 
                Session["IS-NOTIFY"] = ShowNotify;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult IsExpiredSession()
        {
            try
            {
                return Json(Session["SS-USERID"] == null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        #region Private
        private List<NoticeDetailModel> getAllNoticeById(int page)
        {
            Boolean isUser = (Boolean)Session["IS-USERS"];
            int userId = int.Parse(Session["SS-USERID"].ToString());
            var listNotice = new NotifyBussiness().GetAllNotice(isUser, userId, page);
            var rs = (from a in listNotice
                      select new NoticeDetailModel
                      {
                          Id = a.Id,
                          UserId = a.Userid ?? 0,
                          DateSend = a.DateSend ?? DateTime.Now,
                          UserName = a.UserName,
                          Title = a.Title,
                          Type = a.Type ?? 0,
                          Description = a.Description
                      }).ToList();
            return rs;
        }
        #endregion
    }


}