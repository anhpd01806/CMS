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
    public class CategoryController : BaseAuthedController
    {
        // GET: Category
        public ActionResult Index()
        {
            CategoryViewModel model = new CategoryViewModel();
            var allCategory = new CategoryBussiness().GetAllCateGory();
            model.CategoryList = (from a in allCategory
                                  select new CategoryModel
                                  {
                                      Id = a.Id,
                                      Name = a.Name,
                                      Description = a.Description,
                                      ParentName = a.ParentCategoryId != 0 ? allCategory.Where(x => x.Id == a.ParentCategoryId).Select(x => x.Name).FirstOrDefault() : "",
                                      Published = a.Published == true ? "Hoạt động" : "Chưa kích hoạt"
                                  }).ToList();
            return View(model);
        }

        public ActionResult Create()
        {
            CategoryModel model = new CategoryModel();
            var listParent = new CategoryBussiness().GetParentCateGory().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            listParent.Insert(0, new SelectListItem { Value = "0", Text = "Chọn danh mục cha" });
            model.SelectListItem = listParent;
            model.Active = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(CategoryModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var ca = new Category
                    {
                        Name = model.Name,
                        Description = model.Description,
                        ParentCategoryId = model.CategoryId,
                        Published = model.Active,
                        CreatedOn = DateTime.Now,
                        PageSize = 0,
                        AllowCustomersToSelectPageSize = true,
                        ShowOnHomePage = true,
                        IncludeInTopMenu = true,
                        SubjectToAcl = true,
                        Deleted = false,
                        DisplayOrder = 0,
                        UpdatedOn = DateTime.Now,

                    };
                    new CategoryBussiness().Insert(ca);
                    TempData["Success"] = Messages_Contants.SUCCESS_INSERT;
                    ModelState.Clear();
                    model = new CategoryModel();
                    var listParent = new CategoryBussiness().GetParentCateGory().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                    listParent.Insert(0, new SelectListItem { Value = "0", Text = "Chọn danh mục cha" });
                    model.SelectListItem = listParent;
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
            CategoryModel model = new CategoryModel();
            var category = new CategoryBussiness().GetCateGoryById(id);
            var listParent = new CategoryBussiness().GetParentCateGory().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            listParent.Insert(0, new SelectListItem { Value = "0", Text = "Chọn danh mục cha" });
            model.SelectListItem = listParent;
            model.Id = category.Id;
            model.Name = category.Name;
            model.Description = category.Description;
            model.CategoryId = category.ParentCategoryId;
            model.Active = category.Published;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CategoryModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    new CategoryBussiness().Update(model);
                    TempData["Success"] = Messages_Contants.SUCCESS_INSERT;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex;
            }
            return RedirectToAction("Index", "Category");
        }

        public ActionResult Delete(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var rs = new CategoryBussiness().Delete(id);
                    if (rs == false)
                    {
                        TempData["Error"] = "Vui lòng xóa danh mục con trước khi xóa danh mục cha";
                    }
                    else
                    {
                        TempData["Success"] = Messages_Contants.SUCCESS_DELETE;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex;
            }
            return RedirectToAction("Index", "Category");
        }
    }
}