using CMS.Data;
using CMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Bussiness
{
    public class ReportBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();

        public List<ReportTotalModel> GetListReport(DateTime createDate)
        {
            var rs = (from a in db.GetReportTotal(createDate)
                      select new ReportTotalModel
                      {
                          CreateDate = a.CreateDate ?? DateTime.Now,
                          Amount = a.AmountTotal ?? 0,
                          PaymentMethodId = a.PaymentMethodId,
                          Payment = a.Name
                      }).ToList();
            return rs;
        }

        public List<ReportDetailModel> GetListDetail(DateTime Date)
        {
            var rs = (from a in db.PaymentHistories
                      join b in db.PaymentMethods
                      on a.PaymentMethodId equals b.Id
                      join c in db.Users
                      on a.UserId equals c.Id
                      into ps
                      from c in ps.DefaultIfEmpty()
                      where a.CreatedDate.Date == Date.Date
                      select new ReportDetailModel
                      {
                          CreateDate = a.CreatedDate,
                          Amount = a.Amount ?? 0,
                          Payment = b.Name,
                          Note = a.Notes,
                          CustomerName = c.UserName + " (" + c.FullName + ")",
                          DisplayOrder = b.DisplayOrder
                      }).OrderBy(x => x.DisplayOrder).ThenBy(x=>x.CreateDate).ToList();
            return rs;
        }
    }
}