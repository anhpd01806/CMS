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
    public class DistrictController : BaseAuthedController
    {
        // GET: District
        public ActionResult Index()
        {
            DistrictViewModel model = new DistrictViewModel();
            var province = new DistrictBussiness().GetAllProvince();
            model.DistrictList = (from a in new DistrictBussiness().GetAllDistrict()
                                  select new DistrictModel
                                  {
                                      Id = a.Id,
                                      Name = a.Name,
                                      Province = province.Where(x => x.Id == a.ProvinceId).Select(x => x.Name).FirstOrDefault(),
                                      Publish = a.Published == true ? "Hoạt động" : "Chưa kích hoạt"
                                  }).ToList();

            return View(model);
        }

        public ActionResult Create()
        {
            DistrictModel model = new DistrictModel();
            var listProvince = new DistrictBussiness().GetAllProvince().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            model.SelectListItem = listProvince;
            model.Active = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(DistrictModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var ca = new District
                    {
                        Name = model.Name,
                        Description = model.Description,
                        ProvinceId = model.ProvinceId,
                        Published = model.Active,
                        DisplayOrder = 0
                    };
                    new DistrictBussiness().Insert(ca);
                    TempData["Success"] = Messages_Contants.SUCCESS_INSERT;
                    ModelState.Clear();
                    model = new DistrictModel();
                    var listProvince = new DistrictBussiness().GetAllProvince().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                    model.SelectListItem = listProvince;
                    model.Active = true;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex;
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            DistrictModel model = new DistrictModel();
            var district = new DistrictBussiness().GetDistrictById(id);
            var listProvince = new DistrictBussiness().GetAllProvince().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            model.SelectListItem = listProvince;
            model.Id = district.Id;
            model.Name = district.Name;
            model.Description = district.Description;
            model.ProvinceId = district.ProvinceId;
            model.Active = district.Published;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(DistrictModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    new DistrictBussiness().Ụpdate(model);
                    TempData["Success"] = Messages_Contants.SUCCESS_UPDATE;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex;
            }
            return RedirectToAction("Index", "District");
        }

        public ActionResult Delete(int id)
        {
            try
            {

                new DistrictBussiness().Delete(id);
                TempData["Success"] = Messages_Contants.SUCCESS_DELETE;
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex;
            }
            return RedirectToAction("Index", "District");
        }
    }
}