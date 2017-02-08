using CMS.Bussiness;
using CMS.Data;
using CMS.Helper;
using CMS.ViewModel;
using System;
using System.Linq;
using System.Web.Mvc;
using WebBackendPlus.Controllers;

namespace CMS.Controllers
{
    public class ProviceController : BaseAuthedController
    {
        // GET: Provice
        public ActionResult Index()
        {
            ProvinceViewModel model = new ProvinceViewModel();
            model.ProvinceList = (from a in new ProviceBussiness().GetAllProvice()
                                  select new ProviceModel {
                                      Id= a.Id,
                                      Name = a.Name,
                                      Description = a.Description,
                                      Published = a.Published == true ? "Hoạt động": "Chưa kích hoạt"
                                  }).ToList();
            ;
            return View(model);
        }

        public ActionResult Create()
        {
            ProviceModel model = new ProviceModel();
            model.Active = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ProviceModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var ca = new Province
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Published = model.Active,
                        DisplayOrder = 1
                    };
                    new ProviceBussiness().Insert(ca);
                    TempData["Success"] = Messages_Contants.SUCCESS_INSERT;
                    ModelState.Clear();
                    model = new ProviceModel();
                    model.Active = true;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            ProviceModel model = new ProviceModel();
            var district = new ProviceBussiness().GetProviceById(id);
            model.Id = district.Id;
            model.Name = district.Name;
            model.Description = district.Description;
            model.Active = district.Published;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ProviceModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    new ProviceBussiness().Update(model);
                    TempData["Success"] = Messages_Contants.SUCCESS_UPDATE;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
            }
            return RedirectToAction("Index", "Provice");
        }

        public ActionResult Delete(int id)
        {
            try
            {

                new ProviceBussiness().Delete(id);
                TempData["Success"] = Messages_Contants.SUCCESS_DELETE;
            }
            catch (Exception ex)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
            }
            return RedirectToAction("Index", "Provice");
        }
    }
}