using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using CMS.Data;
using CMS.Helper;
using CMS.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace CMS.Bussiness
{
    public class HomeBussiness : InitDB
    {
        #region define
        CmsDataDataContext Instance = new CmsDataDataContext();
        #endregion

        

        #region Role

        public int GetRoleByUser(int userId)
        {
            try
            {
                var query = (from c in Instance.Role_Users
                             where c.UserId.Equals(userId)
                             select new
                             {
                                 RoleId = c.RoleId
                             }).FirstOrDefault();
                if (query != null)
                {
                    return query.RoleId;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }
        #endregion
#region News in home

        public List<DistrictModel> GetListDistric()
        {
            var listdistric = (from c in Instance.Districts
                               where !c.IsDeleted && c.Published
                               select new DistrictModel
                               {
                                   Id = c.Id,
                                   Name = c.Name
                               }).ToList();
            return listdistric;
        }

        public List<CategoryModel> GetListCategory()
        {
            var listcategory = (from c in Instance.Categories
                                where !c.Deleted && c.Published
                                select new CategoryModel
                                {
                                    Id = c.Id,
                                    Name = c.Name,
                                    ParentCategoryId = c.ParentCategoryId
                                }).ToList();
            return listcategory;
        }

        public List<SiteModel> GetListSite()
        {
            var listsite = (from c in Instance.Sites
                            where !c.Deleted && c.Published
                            orderby c.DisplayOrder
                            select new SiteModel
                            {
                                ID = c.ID,
                                Name = c.Name
                            }).ToList();
            return listsite;
        }

        public List<CategoryModel> GetChilldrenlistCategory(int parentId)
        {
            var listcategory = (from c in Instance.Categories
                                where !c.Deleted && c.Published
                                && c.ParentCategoryId.Equals(parentId)
                                select new CategoryModel
                                {
                                    Id = c.Id,
                                    Name = c.Name,
                                    ParentCategoryId = c.ParentCategoryId
                                }).ToList();
            return listcategory;
        }

        public List<NewsStatusModel> GetlistStatusModel()
        {
            var liststatus = (from c in Instance.NewsStatus
                              select new NewsStatusModel()
                              {
                                  Id = c.Id,
                                  Name = c.Name
                              }).ToList();
            return liststatus;
        }

        public List<NewsModel> GetListNewByFilter(int UserId, int CateId, int DistricId, int StatusId,int GovermentID, int SiteId,
            int BackDate, string From, string To, double MinPrice, double MaxPrice, int pageIndex, int pageSize, bool IsRepeat, string key, string NameOrder, bool descending, ref int total)
        {
            #region Orther

            DateTime? from;
            DateTime? to;

            if (!string.IsNullOrEmpty(From))
            {
                from = Convert.ToDateTime((From.Split('-')[2] + "/" + From.Split('-')[1] + "/" + From.Split('-')[0]));
            }
            else
            {
                from = null;
            }
            if (!string.IsNullOrEmpty(To))
            {
                to = Convert.ToDateTime((To.Split('-')[2] + "/" + To.Split('-')[1] + "/" + To.Split('-')[0]));
            }
            else
            {
                to = null;
            }

            decimal? minPrice;
            if (MinPrice != -1)
            {
                minPrice = Convert.ToDecimal(MinPrice);
            }
            else
            {
                minPrice = null;
            }
            decimal? maxPrice;
            if (MaxPrice != -1)
            {
                maxPrice = Convert.ToDecimal(MaxPrice);
            }
            else
            {
                maxPrice = null;
            }
            int? backdate;
            if (BackDate != -1)
            {
                backdate = BackDate;
            }
            else
            {
                backdate = null;
            }

            int? totalout = 0;
            #endregion
            Instance.CommandTimeout = 30;
            var listItem =
                (Instance.PROC_GetListNewsInHome(UserId, CateId, DistricId, StatusId, GovermentID, SiteId, backdate, from, to,
                    minPrice, maxPrice, pageIndex, pageSize, IsRepeat, key, NameOrder, descending, ref totalout)
                    .Select(c => new NewsModel
                    {
                        Id = c.Id.Value,
                        Title = c.Title,
                        CategoryId = c.CategoryId,
                        SiteId = c.SiteId.Value,
                        Link = c.Link,
                        Phone = c.Phone,
                        Contents = c.Contents,
                        Price = c.Price,
                        PriceText = c.PriceText,
                        DistrictId = c.DistrictId,
                        DistictName = c.DistictName,
                        StatusId = c.StatusId,
                        SiteName = c.SiteName,
                        StatusName = c.StatusName,
                        CreatedOn = c.CreatedOn,
                        CusIsReaded = c.CusIsReaded,
                        IsRepeat = c.IsRepeat.HasValue && c.IsRepeat.Value,
                        RepeatTotal = c.RepeatTotal.Value,
                        Iscc = c.Iscc.HasValue && c.Iscc.Value
                    })).ToList();

            total = totalout.HasValue ? Convert.ToInt32(totalout) : 0;
            return listItem;
        }

        //Export excel
        public List<NewsModel> ExportExcel(List<int> listNewsId)
        {
            using (var tran = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            }))
            {
                var query = (from c in Instance.News
                             join d in Instance.Districts on c.DistrictId equals d.Id
                             join t in Instance.NewsStatus on c.StatusId equals t.Id
                             join ncm in Instance.News_Customer_Mappings on c.Id equals ncm.NewsId into temp
                             from tm in temp.DefaultIfEmpty()
                             where c.CreatedOn.HasValue && !c.IsDeleted //&& c.Published.HasValue
                             && listNewsId.Contains(c.Id)
                             orderby c.StatusId ascending, c.Price descending
                             select new NewsModel
                             {
                                 Id = c.Id,
                                 Title = c.Title,
                                 CategoryId = c.CategoryId,
                                 Link = c.Link,
                                 Contents = c.Contents,
                                 Phone = c.Phone,
                                 Price = c.Price,
                                 PriceText = c.PriceText,
                                 DistrictId = d.Id,
                                 DistictName = d.Name,
                                 StatusId = t.Id,
                                 StatusName = t.Name,
                                 CreatedOn = c.CreatedOn,
                                 IsRepeat = c.IsRepeat,
                                 RepeatTotal = 1
                             }).Distinct().ToList();
                return query;
            }
        }

        public NewsModel GetNewsDetail(int Id, int UserId)
        {
            var query = (from c in Instance.News
                         join d in Instance.Districts on c.DistrictId equals d.Id
                         join t in Instance.NewsStatus on c.StatusId equals t.Id
                         join ct in Instance.Categories on c.CategoryId equals ct.Id
                         join st in Instance.Sites on c.SiteId equals st.ID
                         where c.CreatedOn.HasValue && !c.IsDeleted //&& c.Published.HasValue
                               && !d.IsDeleted && d.Published
                               && c.Id.Equals(Id)
                         select new NewsModel
                         {
                             Id = c.Id,
                             Title = c.Title,
                             Link = c.Link,
                             SiteId = c.SiteId,
                             SiteName = st.Name,
                             Phone = c.Phone,
                             Contents = c.Contents,
                             Price = c.Price,
                             PriceText = c.PriceText,
                             DistrictId = d.Id,
                             DistictName = d.Name,
                             StatusId = t.Id,
                             StatusName = t.Name,
                             CreatedOn = c.CreatedOn,
                             CategoryId = ct.Id,
                             CateName = ct.Name,
                             ListImage = GetImageByNewsId(c.Id),
                             SameNews = GetSameNewsByNewsId(Id, c.Title, c.CategoryId.Value, c.DistrictId.Value, c.Phone, UserId),
                             Iscc = CheckCCByUser(c.Id, UserId),
                             PersionalReport = GetNameReasonReport(c.Id),
                             PersonCheck = GetPersonCheckNews(c.Id),
                             IsReason = CheckReason(UserId, c.Id)
                         }).FirstOrDefault();

            if (query != null)
            {

                var getnewsave = (from c in Instance.News_Customer_Mappings
                                  where c.NewsId.Equals(Id) && c.CustomerId.Equals(UserId)
                                  select c).ToList();
                if (!getnewsave.Any())
                {
                    var newItem = new News_Customer_Mapping();
                    newItem.CustomerId = UserId;
                    newItem.NewsId =
                        newItem.NewsId = Id;
                    newItem.IsSaved = false;
                    newItem.IsDeleted = false;
                    newItem.IsReaded = true;
                    newItem.IsAgency = false;
                    newItem.IsSpam = false;
                    newItem.CreateDate = DateTime.Now;
                    Instance.News_Customer_Mappings.InsertOnSubmit(newItem);

                }
                else
                {
                    foreach (var newsCustomerMapping in getnewsave)
                    {
                        newsCustomerMapping.IsReaded = true;
                    }
                }
                Instance.SubmitChanges();
            }
            return query;
        }

        public New GetNewsDetail(int Id)
        {
            var query = (from c in Instance.News
                         where c.Id.Equals(Id)
                         orderby c.Id ascending
                         select c).FirstOrDefault();
            return query;
        }

        public int SaveNewByUserId(List<News_Customer_Mapping> cusNews, int userId)
        {
            try
            {
                foreach (var item in cusNews)
                {
                    var query = (from c in Instance.News_Customer_Mappings
                                 where c.NewsId.Equals(item.NewsId) && c.CustomerId.Equals(userId)
                                 select c).FirstOrDefault();
                    if (query == null)
                    {
                        Instance.News_Customer_Mappings.InsertOnSubmit(item);
                    }
                    else
                    {
                        query.IsSaved = true;
                        query.IsDeleted = false;
                    }
                }
                Instance.SubmitChanges();
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public int HideNewByUserId(List<News_Customer_Mapping> cusNews, int userId)
        {
            try
            {
                foreach (var item in cusNews)
                {
                    var query = (from c in Instance.News_Customer_Mappings
                                 where c.NewsId.Equals(item.NewsId) && c.CustomerId.Equals(userId)
                                 select c).ToList();
                    if (!query.Any())
                    {
                        Instance.News_Customer_Mappings.InsertOnSubmit(item);
                    }
                    else
                    {
                        foreach (var newsCustomerMapping in query)
                        {
                            newsCustomerMapping.IsDeleted = true;
                            newsCustomerMapping.IsSaved = false;
                        }
                    }
                }
                Instance.SubmitChanges();
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public List<ImageModel> GetImageByNewsId(int NewsID)
        {
            var query = (from c in Instance.Images
                         where c.NewsId.Equals(NewsID)
                         select new ImageModel { Id = c.Id, NewsId = c.NewsId, ImageUrl = c.ImageUrl }).ToList();
            return query;
        }

        public List<NewsModel> GetSameNewsByNewsId(int Id, string Title, int CateId, int DistricId, string phone, int UserId)
        {
            var query = (from c in Instance.News
                         join d in Instance.Districts on c.DistrictId equals d.Id
                         join t in Instance.NewsStatus on c.StatusId equals t.Id
                         join st in Instance.Sites on c.SiteId equals st.ID

                         where
                         //c.CreatedOn.HasValue && !c.IsDeleted //&& c.Published.HasValue
                         //&& !d.IsDeleted && d.Published
                         //&& !news_new.Contains(c.Id)
                         //&& c.CategoryId.Equals(CateId)
                         //&& c.DistrictId.Equals(DistricId)
                         c.Phone.Contains(phone) && c.Phone != "" && c.Title != Title && !c.IsRepeat 
                         && !c.Id.Equals(Id) && (!c.Published.HasValue || c.Published.Value) && !c.IsDeleted
                         group new { c, st} by new {c.Title, c.Link, st.Name} into g
                         //orderby c.StatusId ascending, c.Price descending
                         select new NewsModel
                         {
                             Title = g.Key.Title,
                             Link = g.Key.Link,
                             SiteName = g.Key.Name
                         }).Skip(0).Take(3).ToList();
            var listItem = new List<NewsModel>();
            foreach (var item in query)
            {
                var newsItem = (from c in Instance.News
                                join t in Instance.NewsStatus on c.StatusId equals t.Id
                                join st in Instance.Sites on c.SiteId equals st.ID
                                where c.Title.Equals(item.Title)
                                select new NewsModel
                                {
                                    Id = c.Id,
                                    Title = c.Title,
                                    Link = c.Link,
                                    SiteName = st.Name
                                }).FirstOrDefault();
                listItem.Add(newsItem);
            }
            return listItem;
        }

        public String GetNameReasonReport(int newsId)
        {
            string rs = "";
            var reasonlst = Instance.ReasonReportNews.Where(x => x.NewsId == newsId).Take(3).ToList();
            if (reasonlst.Any())
            {
                foreach (var item in reasonlst)
                {
                    rs += "<li><span>" + new UserBussiness().GetNameById(item.UserId)
                        + "(" + item.DateCreate.ToString("dd/MM/yyyy") + ":) </br></span>"
                        + "<span>- " + item.Note + "</span></li>";
                }
            }
            else
            {
                rs = "Chưa có ai";
            }
            return rs;
        }

        public String GetPersonCheckNews(int newsId)
        {
            string rs = "";
            var reasonlst = Instance.News_customer_actions.Where(x => x.NewsId == newsId && x.Iscc == true).OrderByDescending(x => x.DateCreate).Take(3).ToList();
            if (reasonlst.Any())
            {
                foreach (var item in reasonlst)
                {
                    rs += "<li><span>" + new UserBussiness().GetNameById(item.CustomerId)
                        + "(" + item.DateCreate.ToString("dd/MM/yyyy") + ")</br></span></li>";
                }
            }
            else
            {
                rs = "Chưa có ai";
            }
            return rs;
        }
        public int ReportNews(List<New> listNewsReport, int userReport)
        {
            try
            {
                foreach (var item in listNewsReport)
                {
                    var reportItem = new NewsReport
                    {
                        StatusId = Convert.ToInt32(item.StatusId),
                        NewsId = item.Id,
                        CreateDate = DateTime.Now,
                        Notes = "Tin môi giới: " + item.Title,
                        CustomerId = userReport
                    };
                    Instance.NewsReports.InsertOnSubmit(reportItem);
                }
                Instance.SubmitChanges();
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public int Spam(List<New> listNewReport, int userReport)
        {
            try
            {
                foreach (var item in listNewReport)
                {
                    var newsqr = (from c in Instance.News
                                  where c.Id.Equals(item.Id)
                                  select c).FirstOrDefault();

                    if (newsqr != null)
                    {
                        if (!string.IsNullOrEmpty(newsqr.Phone))
                        {
                            newsqr.IsSpam = true;
                            var listPhone = newsqr.Phone.Split(',');
                            foreach (var itemPhone in listPhone)
                            {
                                var bll =
                            (from c in Instance.Blacklists where c.Words.Contains(itemPhone) select c).FirstOrDefault();
                                if (bll == null)
                                {
                                    var reportItem = new Blacklist
                                    {
                                        Words = itemPhone,
                                        Description = item.Title,
                                        LinkUrl = item.Link,
                                        CreatedOn = DateTime.Now,
                                        Type = 1
                                    };
                                    Instance.Blacklists.InsertOnSubmit(reportItem);
                                }
                            }

                            var query = (from c in Instance.News_customer_actions
                                         where
                                             c.NewsId.Equals(item.Id) && c.CustomerId.Equals(userReport) && c.IsReport.HasValue &&
                                             c.IsReport.Value
                                         select c).ToList();
                            if (!query.Any())
                            {
                                var action = new News_customer_action
                                {
                                    NewsId = item.Id,
                                    CustomerId = userReport,
                                    Iscc = CheckCC(item.Id) ? true : false,
                                    IsSpam = false,
                                    Ischeck = false,
                                    IsReport = true,
                                    DateCreate = DateTime.Now
                                };
                                Instance.News_customer_actions.InsertOnSubmit(action);
                            }
                            Instance.SubmitChanges();
                        }
                    }
                }
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public int Delete(int[] listNewDelete, int userId)
        {
            try
            {
                for (int i = 0; i < listNewDelete.Length; i++)
                {
                    var query = (from c in Instance.News_Customer_Mappings
                                 where c.CustomerId.Equals(userId) && c.NewsId.Equals(listNewDelete[i]) && !c.IsAgency.Value
                                 select c).ToList();
                    if (query.Any())
                    {
                        foreach (var item in query)
                        {
                            item.IsDeleted = false;
                            item.IsSaved = false;
                        }
                    }

                    var query2 = (from c in Instance.News_Trashes
                                  where c.CustomerID.Equals(userId) && c.NewsId.Equals(listNewDelete[i])
                                  select c).ToList();
                    if (!query2.Any())
                    {
                        var itemtrash = new News_Trash
                        {
                            CustomerID = userId,
                            NewsId = listNewDelete[i],
                            Isdelete = true,
                            Isdeleted = false,
                            IsSpam = false
                        };
                        Instance.News_Trashes.InsertOnSubmit(itemtrash);
                    }
                    Instance.SubmitChanges();
                }
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public int CountRepeatnews(int newId, int userId, int districId)
        {
            var listBlacklist = (from c in Instance.Blacklists
                                 select (c.Words)).ToList();

            var news_new = new List<int>();
            if (GetRoleByUser(userId) == Convert.ToInt32(CmsRole.Administrator))
            {
                news_new = (from c in Instance.News_Customer_Mappings
                            where (c.IsDeleted.Value || c.IsSaved.Value) //c.CustomerId.Equals(userId) &&
                            select (c.NewsId)).ToList();
            }
            else
            {
                news_new = (from c in Instance.News_Customer_Mappings
                            where c.CustomerId.Equals(userId) && (c.IsDeleted.Value || c.IsSaved.Value)
                            select (c.NewsId)).ToList();
            }

            var news = GetNewsDetail(newId);
            if (news != null)
            {
                var query = from c in Instance.News
                            join d in Instance.Districts on c.DistrictId equals d.Id
                            join t in Instance.NewsStatus on c.StatusId equals t.Id
                            join ncm in Instance.News_Customer_Mappings on c.Id equals ncm.NewsId into temp
                            from tm in temp.DefaultIfEmpty()
                            where c.CreatedOn.HasValue && !c.IsDeleted //&& c.Published.HasValue
                            && !d.IsDeleted && d.Published
                            && !news_new.Contains(c.Id)
                            && !listBlacklist.Contains(c.Phone)
                            && c.DistrictId.Equals(news.DistrictId)
                            && c.Phone.Contains(news.Phone)
                            orderby c.StatusId ascending, c.Price descending
                            select new NewsModel
                            {
                                Id = c.Id,
                                Title = c.Title,
                                CategoryId = c.CategoryId,
                                Link = c.Link,
                                Phone = c.Phone,
                                Price = c.Price,
                                PriceText = c.PriceText,
                                DistrictId = d.Id,
                                SiteId = c.SiteId,
                                DistictName = d.Name,
                                StatusId = t.Id,
                                StatusName = t.Name,
                                CreatedOn = c.CreatedOn,
                                CusIsReaded = tm.IsReaded
                            };

                var total = query.ToList().Count;
                return total;
            }
            return 0;
        }

        public int CountRepeatByPhone(string phone, int userID)
        {
            try
            {
                var listBlacklist = (from c in Instance.Blacklists
                                     select (c.Words)).ToList();
                var listDelete = (from c in Instance.News_Trashes
                                  where (c.Isdelete || c.Isdeleted) && c.CustomerID.Equals(userID)
                                  select (c.NewsId)).ToList();
                var query = (from c in Instance.News
                             where c.CreatedOn.HasValue && !c.IsDeleted
                             && c.Phone.Contains(phone)
                             && !listDelete.Contains(c.Id)
                             && !listBlacklist.Contains(c.Phone)
                             select c).ToList();
                return query.Count;
            }
            catch
            {
                return 0;
            }
        }

        public int NewsforUser(int[] listnews, int userId)
        {
            try
            {
                for (int i = 0; i < listnews.Length; i++)
                {
                    var checkcc = (from c in Instance.News_customer_actions
                                   where c.NewsId.Equals(listnews[i])
                                   select c).ToList();
                    var query = (from c in Instance.News_customer_actions
                                 where c.NewsId.Equals(listnews[i]) && c.CustomerId.Equals(userId) && c.Iscc.HasValue && c.Iscc.Value
                                 select c).ToList();
                    if (!query.Any())
                    {
                        var item = new News_customer_action();
                        item.CustomerId = userId;
                        item.NewsId = listnews[i];
                        item.Iscc = true;
                        item.Ischeck = false;
                        item.IsSpam = false;
                        item.DateCreate = DateTime.Now;
                        Instance.News_customer_actions.InsertOnSubmit(item);
                    }
                    foreach (var item in checkcc)
                    {
                        item.Iscc = true;
                    }
                    Instance.SubmitChanges();
                }
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public int RemoveNewsforUser(int[] listnews, int userId)
        {
            try
            {
                foreach (var t in listnews)
                {
                    var query = (from c in Instance.News_customer_actions
                                 where c.NewsId.Equals(t) && c.CustomerId.Equals(userId) && c.Iscc.HasValue && c.Iscc.Value
                                 select c).ToList();
                    if (query.Any())
                    {
                        foreach (var newsCustomerAction in query)
                        {
                            Instance.News_customer_actions.DeleteOnSubmit(newsCustomerAction);
                        }
                    }
                    Instance.SubmitChanges();
                }
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public bool CheckCC(int newsId)
        {
            var query = (from c in Instance.News_customer_actions
                         where c.NewsId.Equals(newsId) && c.Iscc.HasValue && c.Iscc.Value
                         select c).ToList();
            if (query.Any())
                return true;
            return false;
        }

        public bool CheckReason(int userId, int newsId)
        {
            return Instance.ReasonReportNews.Any(x => x.UserId == userId && x.NewsId == newsId);
        }

        public bool CheckCCByUser(int newsId, int userId)
        {
            var query = (from c in Instance.News_customer_actions
                         where c.NewsId.Equals(newsId) && c.Iscc.HasValue && c.Iscc.Value && c.CustomerId.Equals(userId)
                         select c).ToList();
            if (query.Any())
                return true;
            return false;
        }
        #endregion
        #region ReasonReportNews
        public void InsertReasonReportNews(ReasonReportNew model)
        {
            Instance.ReasonReportNews.InsertOnSubmit(model);
            Instance.SubmitChanges();
        }

        public void DeleteReasonReportNews(int newsId, int userId)
        {
            var check = Instance.ReasonReportNews.FirstOrDefault(x => x.UserId == userId && x.NewsId == newsId);
            if (check != null)
            {
                Instance.ReasonReportNews.DeleteOnSubmit(check);
                Instance.SubmitChanges();
            }
        }
        #endregion
    }
}