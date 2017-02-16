using CMS.Bussiness;
using CMS.Data;
using CMS.Helper;
using CMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBackendPlus.Controllers;

namespace CMS.Controllers
{
    public class CategorySiteController : BaseAuthedController
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
                                          CategoryName = new CategoryBussiness().GetNameCategoryById(a.CategoryId ?? 0),
                                          Status = a.Published == true ? "Hoạt động" : "Ngừng hoạt động"
                                      }).OrderByDescending(x => x.Id).OrderBy(x => x.Status).ToList();
            return View(model);
        }

        public ActionResult Create()
        {
            CategorySiteModel model = new CategorySiteModel();
            GetAllDropPage(ref model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(CategorySiteModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var categorySite = new CategorySite
                    {
                        Name = model.Name,
                        ParentId = model.ParentId,
                        SiteId = model.SiteId,
                        CategoryId = model.CategoryId,
                        Description = model.Description,
                        Published = model.Active,
                        Deleted = false,
                        DisplayOrder = 1
                    };

                    new CategorySiteBussiness().Insert(categorySite);
                    TempData["Success"] = Messages_Contants.SUCCESS_INSERT;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex;
            }
            ModelState.Clear();
            model = new CategorySiteModel();
            GetAllDropPage(ref model);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            CategorySiteModel model = new CategorySiteModel();
            GetAllDropPage(ref model);
            var categorySite = new CategorySiteBussiness().GetCategorySiteById(id);
            model.Id = categorySite.Id;
            model.Name = categorySite.Name;
            model.Description = categorySite.Description;
            model.ParentId = categorySite.ParentId ?? 0;
            model.SiteId = categorySite.SiteId;
            model.CategoryId = categorySite.CategoryId ?? 0;
            model.Active = categorySite.Published;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CategorySiteModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    new CategorySiteBussiness().Update(model);
                    TempData["Success"] = Messages_Contants.SUCCESS_UPDATE;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex;
            }
            return RedirectToAction("Index", "CategorySite");
        }

        public ActionResult Delete(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    new CategorySiteBussiness().Delete(id);
                    TempData["Success"] = Messages_Contants.SUCCESS_UPDATE;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex;
            }
            return RedirectToAction("Index", "CategorySite");
        }
        private void GetAllDropPage(ref CategorySiteModel model)
        {
            model.CategorySiteParentSiteList = new CategorySiteBussiness().GetAllCategorySite()
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            model.SiteList = new LinkSiteBussiness().GetAllSite()
                .Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Name }).ToList();
            model.CategoryList = new CategoryBussiness().GetCategoryDrop();
            model.Active = true;
        }
    }
}