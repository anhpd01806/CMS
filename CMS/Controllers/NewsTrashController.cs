using System;
using System.Collections.Generic;
using System.Linq;
using System;
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
    public class NewsTrashController : Controller
    {
        #region member
        private readonly NewsBussiness _newsbussiness = new NewsBussiness();
        private readonly HomeBussiness _homebussiness = new HomeBussiness();
        #endregion
        //
        // GET: /NewsTrash/
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
                var listCategory = _homebussiness.GetListCategory();
                var cateListItems = new List<SelectListItem>();
                cateListItems.Add(new SelectListItem { Text = "Chọn chuyên mục", Value = "0" });
                foreach (var item in listCategory)
                {
                    if (item.ParentCategoryId == 0)
                    {
                        cateListItems.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                        var listchillcate = _homebussiness.GetChilldrenlistCategory(item.Id);
                        foreach (var chill in listchillcate)
                        {
                            cateListItems.Add(new SelectListItem { Text = ("\xA0\xA0\xA0" + chill.Name), Value = chill.Id.ToString() });
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

                ViewBag.Accept = Convert.ToBoolean(Session["USER-ACCEPTED"]);
                ViewBag.User = Convert.ToBoolean(string.IsNullOrEmpty(Session["IS-USERS"].ToString()) ? "false" : Session["IS-USERS"]);
                model.ListNew = _newsbussiness.GetListNewDeleteByFilter(userId, 0, 0, 0, 0, -1, string.Empty, string.Empty, 0, -1, model.pageIndex, model.pageSize,false, string.Empty, ref total);
                model.Total = total;
                model.Totalpage = (int)Math.Ceiling((double)model.Total / (double)model.pageSize);
                model.RoleId = _homebussiness.GetRoleByUser(userId);
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
                return null;
            }
        }
	}
}