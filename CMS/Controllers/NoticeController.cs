using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.Models;
using CMS.Bussiness;
using WebBackendPlus.Controllers;

namespace CMS.Controllers
{
    public class NoticeController : BaseAuthedController
    {
        // GET: Notice
        public ActionResult Index()
        {
            return View();
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
                //update notify status
                new NotifyBussiness().UpdateNotifyView(id);
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
                //update session
                List<NoticeModel> lstNotify = new NotifyBussiness().GetNotify(false, 1);
                Session["NotityUser"] = lstNotify;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

    }


}