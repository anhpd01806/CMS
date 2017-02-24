using CMS.Data;
using CMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Bussiness
{
    public class ReportNewsBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();

        public ReportNewsViewModel GetRandomNewsReport(int id)
        {
            ReportNewsViewModel model = new ReportNewsViewModel();
            Random rnd = new Random();
            var newsWithUser = (from a in db.NewsReports
                                join b in db.Users
                                on a.CustomerId equals b.Id
                                join c in db.News
                                on a.NewsId equals c.Id
                                select new NewsReportModel
                                {
                                    Id = a.Id,
                                    StatusId = a.StatusId,
                                    NewsId = a.NewsId,
                                    Users = b.UserName,
                                    Notes = a.Notes,
                                    CreateDate = a.CreateDate.Date.ToString(),
                                    Description = c.Contents,
                                    Phone = c.Phone
                                }).Take(30).ToList();

            model.NewsReportList = newsWithUser.GroupBy(m => new { m.NewsId, m.Notes, m.CreateDate, m.Description,m.Phone })
                        .Select(g => new NewsReportModel
                        {
                            NewsId = g.Key.NewsId,
                            Notes = g.Key.Notes,
                            CreateDate = g.Key.CreateDate,
                            Users = string.Join(",", newsWithUser.Where(x => x.NewsId == g.Key.NewsId).Select(x => x.Users).ToList()),
                            Description = g.Key.Description,
                            Phone = g.Key.Phone
                        }).ToList();


            model.FirstRandomNewsReport = model.NewsReportList.FirstOrDefault(x => x.NewsId == id);
            if (model.FirstRandomNewsReport == null) model.FirstRandomNewsReport = model.NewsReportList.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            return model;
        }

        public List<NewsReport> GetNewsReportByNewsId(int newId)
        {
            return db.NewsReports.Where(x => x.NewsId == newId).ToList();
        }

        public Boolean DeleteReportNews(int id)
        {
            var reportNews = db.NewsReports.Where(x => x.NewsId == id).ToList();
            if (reportNews != null)
            {
                db.NewsReports.DeleteAllOnSubmit(reportNews);
                db.SubmitChanges();
                return true;
            }
            else return false;
        }
    }
}