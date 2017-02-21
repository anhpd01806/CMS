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
    public class NewsController : Controller
    {
        #region member
        private readonly NewsBussiness _newsbussiness = new NewsBussiness();
        private readonly HomeBussiness _homebussiness = new HomeBussiness();
        #endregion

        public ActionResult Create()
        {
            return View();
        }
	}
}