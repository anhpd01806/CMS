using CMS.Bussiness;
using CMS.ViewModel;
using System.Web.Mvc;
using WebBackendPlus.Controllers;

namespace CMS.Controllers
{
    public class CustomerController : BaseAuthedController
    {
        // GET: Customer
        public ActionResult Index(int id)
        {
            UserModel model = new UserModel();
            model.ManagerList = new UserBussiness().GetManagerUser();
            var user = new UserBussiness().GetUserById(id);
            model.Id = user.Id;
            model.FullName = user.FullName;
            model.UserName = user.UserName;
            model.Sex = user.Sex ?? false;
            model.Phone = user.Phone;
            model.Email = user.Email;
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(UserModel model)
        {
            new UserBussiness().UpdateCustomer(model); 
            return RedirectToAction("Index","Notice");
        }
    }
}