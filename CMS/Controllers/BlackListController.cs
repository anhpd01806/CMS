using CMS.Bussiness;
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
    public class BlackListController : BaseAuthedController
    {
        private int PageSize = 20;
        // GET: BlackList
        public ActionResult Index()
        {
            BlackListViewModel model = new BlackListViewModel();
            return View(model);
        }

        #region Json
        [HttpPost]
        public JsonResult InsertData(string Phone, string Description)
        {
            try
            {
                var blackList = new Blacklist
                {
                    Words = Phone,
                    Description = Description,
                    CreatedOn = DateTime.Now,
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
        public JsonResult DeleteData(int id)
        {
            try
            {
                new BlackListBussiness().Delete(id);
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
                          CreatedOn = a.CreatedOn,
                          Type = a.Type
                      }).ToList();
            totalPage = (int)Math.Ceiling((double)totalCount / (double)pageSize);
            return rs;
        }

        #endregion
    }
}