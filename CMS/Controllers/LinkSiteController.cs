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
    public class LinkSiteController : BaseAuthedController
    {
        private int PageSize = 20;
        // GET: Site
        public ActionResult Index()
        {
            LinkSiteViewModel model = new LinkSiteViewModel();
            try
            {
                var allSite = new LinkSiteBussiness().GetAllSite();
                if (allSite != null)
                {
                    // get link site
                    model.SiteList = allSite.Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.Name }).ToList();

                    //get category site by link site
                    var categorySite = new CategoryBussiness().GetCategorySiteBySiteId(allSite.First().ID);

                    //get parent category site
                    model.CategorySite = new List<SelectListItem>();
                    foreach (var item in categorySite.Where(x => x.ParentId == 0))
                    {
                        model.CategorySite.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                        // get child category with parent
                        foreach (var itemChild in categorySite.Where(x => x.ParentId == item.Id))
                        {
                            model.CategorySite.Add(new SelectListItem {Text = "   " + itemChild.Name, Value = itemChild.Id.ToString() });
                        }
                    }
                }
                // get all province
                var allProvince = new ProviceBussiness().GetAllProvice();
                if (allProvince != null)
                {
                    model.ProvinceList = allProvince.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

                    //get district by province
                    model.DistrictList = new DistrictBussiness().GetDistrictByProvinceId(allProvince.First().Id).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                }
                //model.Totalpage = (int)Math.Ceiling((double)new LinkSiteBussiness().GetLinkSiteByParam("", allSite.First().ID
                //    , int.Parse(model.CategorySite.First().Value), int.Parse(model.CategorySite.First().Value), allProvince.First().Id, PageSize, 0).Count() / (double)PageSize);
            }
            catch (Exception)
            {

                throw;
            }
            return View(model);
        }

        #region Json
        [HttpPost]
        public JsonResult DeleteData(int id)
        {
            try
            {
                new LinkSiteBussiness().Delete(id);

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
        public JsonResult InsertData(string LinkUrl, int SiteId, int CategoryId, int ProvinceId, int districtId)
        {
            try
            {
                var linkSite = new LinkSite
                {
                    Url = LinkUrl,
                    SiteId = SiteId,
                    CategorySiteId = CategoryId,
                    DistrictId = districtId,
                    ProvinceId = ProvinceId,
                    Published = true,
                    Deleted = false,
                    DisplayOrder = 0
                };

                new LinkSiteBussiness().Insert(linkSite);

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
        public JsonResult LoadData(string search, int SiteId, int CategoryId, int ProvinceId, int districtId, int pageIndex)
        {
            try
            {
                int totalpage = 0;
                LinkSiteViewModel model = new LinkSiteViewModel();
                model.LinkSiteList = GetLinkSite(ref totalpage, search, SiteId, CategoryId, districtId, ProvinceId, PageSize, pageIndex);
                var content = RenderPartialViewToString("~/Views/LinkSite/LinkSiteDetail.cshtml", model.LinkSiteList);
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

        public ActionResult FillCategorySite(int id)
        {
            LinkSiteViewModel model = new LinkSiteViewModel();

            //get category site by link site
            var categorySite = new CategoryBussiness().GetCategorySiteBySiteId(id);

            //get parent category site
            model.CategorySite = new List<SelectListItem>();
            foreach (var item in categorySite.Where(x => x.ParentId == 0))
            {
                model.CategorySite.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                // get child category with parent
                foreach (var itemChild in categorySite.Where(x => x.ParentId == item.Id))
                {
                    model.CategorySite.Add(new SelectListItem { Text = "    >> " + itemChild.Name, Value = itemChild.Id.ToString() });
                }
            }
            return Json(model.CategorySite, JsonRequestBehavior.AllowGet);
        }


        public ActionResult FillDistrict(int id)
        {
            LinkSiteViewModel model = new LinkSiteViewModel();
            //get district by province
            model.DistrictList = new DistrictBussiness().GetDistrictByProvinceId(id).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            return Json(model.DistrictList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PrivateFuntion

        private List<LinkSiteModel> GetLinkSite(ref int totalPage, string search, int siteId, int categorySiteId, int districtId, int provinceId, int pageSize, int pageIndex)
        {
            int totalCount = 0;
            var linkSite = new LinkSiteBussiness().GetLinkSiteByParam(ref totalCount, search, siteId, categorySiteId, districtId, provinceId, pageSize, (pageIndex - 1));
            //var categoryName = new CategoryBussiness().GetNameCategorySiteById(categorySiteId);
            var district = new DistrictBussiness().GetDistrictById(districtId);
            var rs = (from a in linkSite
                      select new LinkSiteModel
                      {
                          Id = a.Id,
                          Url = a.Url,
                          Site = new LinkSiteBussiness().GetNameSiteById(siteId),
                          //Category = categoryName,
                          District = district == null ? "" : district.Name,
                          Province = new DistrictBussiness().GetNameProvinceById(provinceId),
                          Status = a.Published == true ? "Hoạt động" : "Tạm ngừng"
                      }).ToList();
            totalPage = (int)Math.Ceiling((double)totalCount / (double)pageSize);
            return rs;
        }

        #endregion
    }
}