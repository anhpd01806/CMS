using CMS.Bussiness;
using CMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBackendPlus.Controllers;

namespace CMS.Controllers
{
    public class CategorySiteController : Controller
    {

        // GET: CategorySite
        public ActionResult Index()
        {
            CategorySiteViewModel model = new CategorySiteViewModel();

            //get all category site
            var getall = new CategorySiteBussiness().GetAllCategorySite();
            model.CategorySiteList = (from a in getall
                                      select new CategorySiteModel
                                      {
                                          Id = a.Id,
                                          Name = a.Name,
                                          Description = a.Description,
                                          ParentName = getall.FirstOrDefault(x => x.Id == a.ParentId) != null ? getall.FirstOrDefault(x => x.Id == a.ParentId).Name : "",
                                          SiteName = new LinkSiteBussiness().GetNameSiteById(a.SiteId),
                                          CategoryName = new CategoryBussiness().GetNameCategorySiteById(a.CategoryId ?? 0),
                                          Status = a.Published == true ? "Hoạt động" : "Ngừng hoạt động"
                                      }).OrderBy(x=>x.Status).ToList();
            return View(model);
        }

        public ActionResult Create()
        {
            CategorySiteModel model = new CategorySiteModel();

            model.CategorySiteParentSiteList = new CategorySiteBussiness().GetAllCategorySite().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            return View();
        }
    }
}