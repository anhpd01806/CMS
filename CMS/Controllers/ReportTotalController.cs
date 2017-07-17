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
    public class ReportTotalController : BaseAuthedController
    {
        // GET: ReportTotal
        public ActionResult Index()
        {
            ReportTotalViewModel model = new ReportTotalViewModel();
            model.DateTime = GetDates(DateTime.Now.Year, DateTime.Now.Month);
            model.PaymentMethodList = new PaymentBussiness().GetPaymentList();
            model.ReportTotal = new ReportBussiness().GetListReport(DateTime.Now);
            model.DatePick = DateTime.Now;
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ReportTotalViewModel model)
        {
            model.DateTime = GetDates(model.DatePick.Year, model.DatePick.Month);
            model.PaymentMethodList = new PaymentBussiness().GetPaymentList();
            model.ReportTotal = new ReportBussiness().GetListReport(model.DatePick);
            model.DatePick = model.DatePick;
            return View(model);
        }

        public ActionResult Detail(string DateInput)
        {
            ReportTotalViewModel model = new ReportTotalViewModel();
            model.ReportDetail = new ReportBussiness().GetListDetail(DateTime.ParseExact(DateInput, "dd/MM/yyyy",null));
            return View(model);
        }

        private List<DateTime> GetDates(int year, int month)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))  // Days: 1, 2 ... 31 etc.
                             .Select(day => new DateTime(year, month, day)) // Map each day to a date
                             .ToList(); // Load dates into a list
        }
    }
}