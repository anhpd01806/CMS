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
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace CMS.Bussiness
{

    public class HomeBussiness
    {
        #region define
        CmsDataDataContext db = new CmsDataDataContext();
        #endregion

        #region News in home

        public List<DistrictModel> GetListDistric()
        {
            var listdistric = (from c in db.Districts
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
            var listcategory = (from c in db.Categories
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
            var listsite = (from c in db.Sites
                            where !c.Deleted && c.Published
                            select new SiteModel
                            {
                                ID = c.ID,
                                Name = c.Name
                            }).ToList();
            return listsite;
        }

        public List<CategoryModel> GetChilldrenlistCategory(int parentId)
        {
            var listcategory = (from c in db.Categories
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
            var liststatus = (from c in db.NewsStatus
                              select new NewsStatusModel()
                              {
                                  Id = c.Id,
                                  Name = c.Name
                              }).ToList();
            return liststatus;
        }

        public List<NewsModel> GetListNewByFilter(int UserId, int CateId, int DistricId, int StatusId, int SiteId,
            int BackDate, string From, string To, double MinPrice, double MaxPrice, int pageIndex, int pageSize, bool IsRepeat, string key, ref int total)
        {
            using (var tran = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            }))
            {
                var listBlacklist = (from c in db.Blacklists
                                     select (c.Words)).ToList();

                var listDelete = (from c in db.News_Trashes
                                  where (c.Isdelete || c.Isdeleted) && c.CustomerID.Equals(UserId)
                                  select (c.NewsId)).ToList();

                //Danh sách tin đã lưu hoặc đã ẩn theo user
                var news_new = (from c in db.News_Customer_Mappings
                                where c.CustomerId.Equals(UserId) && (c.IsDeleted.Value || c.IsSaved.Value || c.IsAgency.Value)
                                select (c.NewsId)).ToList();

                //Danh sách tin đã đọc theo user
                var news_isread = (from c in db.News_Customer_Mappings
                                   where c.CustomerId.Equals(UserId) && c.IsReaded.Value
                                   select (c.NewsId)).ToList();

                //Nếu là admin kiểm tra tin đã được báo chính chủ chưa
                //var newsisactive = (from c in db.News_customer_actions where c.Iscc.HasValue && c.Iscc.Value select c.NewsId).ToList();

                var query = (from c in db.News
                            join d in db.Districts on c.DistrictId equals d.Id
                            join t in db.NewsStatus on c.StatusId equals t.Id
                            join s in db.Sites on c.SiteId equals s.ID
                            //join ncm in db.News_Customer_Mappings on c.Id equals ncm.NewsId into temp
                            //from tm in temp.DefaultIfEmpty()
                            where c.CreatedOn.HasValue && !c.IsDeleted && !s.Deleted && s.Published //&& c.Published.HasValue
                            && !d.IsDeleted && d.Published
                            && !listDelete.Contains(c.Id)
                            && !news_new.Contains(c.Id)
                            && (!listBlacklist.Contains(c.Phone) || !listBlacklist.Contains(c.Title) || !listBlacklist.Contains(c.Contents))
                            //orderby  c.CreatedOn descending
                            select new NewsModel
                            {
                                Id = c.Id,
                                Title = c.Title,
                                CategoryId = c.CategoryId,
                                SiteId = c.SiteId,
                                Link = c.Link,
                                Phone = c.Phone,
                                Contents = c.Contents,
                                Price = c.Price,
                                PriceText = c.PriceText,
                                DistrictId = d.Id,
                                DistictName = d.Name,
                                StatusId = t.Id,
                                StatusName = t.Name,
                                CreatedOn = c.CreatedOn,
                                CusIsReaded = news_isread.Contains(c.Id) ? true : false,  //CheckReadByUser(UserId, c.Id),
                                //CusIsSaved = tm.IsSaved,
                                //CusIsDeleted = tm.IsDeleted,
                                IsRepeat = c.IsRepeat,
                                RepeatTotal = 0,//CountRepeatByPhone(c.Phone, UserId),
                                IsAdmin = GetRoleByUser(UserId) == Convert.ToInt32(CmsRole.Administrator) ? true : false
                            }).Distinct();

                #region check param
                //check admin để không load tin đã được duyệt ra nữa
                //if (!checkuser)
                //{
                //    query = query.Where(c => !newsisactive.Contains(c.Id));
                //}

                if (CateId != 0)
                {
                    query = query.Where(c => c.CategoryId.Equals(CateId));
                }
                if (DistricId != 0)
                {
                    query = query.Where(c => c.DistrictId.Equals(DistricId));
                }
                if (StatusId != 0)
                {
                    query = query.Where(c => c.StatusId.Equals(StatusId));
                }
                if (SiteId != 0)
                {
                    query = query.Where(c => c.SiteId.Equals(SiteId));
                }
                if (BackDate != -1)
                {
                    query = BackDate == 0 ? query.Where(c => c.CreatedOn >= Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00.00") && c.CreatedOn <= Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59.999")) : query.Where(c => c.CreatedOn >= Convert.ToDateTime(DateTime.Now.AddDays(-BackDate).ToString("yyyy/MM/dd") + " 00:00:00.00"));
                }
                if (!string.IsNullOrEmpty(From))
                {
                    query = query.Where(c => c.CreatedOn >= Convert.ToDateTime((From.Split('-')[2] + "/" + From.Split('-')[1] + "/" + From.Split('-')[0] + " 00:00:00.00")));
                }
                if (!string.IsNullOrEmpty(To))
                {
                    query = query.Where(c => c.CreatedOn <= Convert.ToDateTime((To.Split('-')[2] + "/" + To.Split('-')[1] + "/" + To.Split('-')[0] + " 23:59:59.999")).AddDays(-1));
                }
                if (MinPrice != -1)
                {
                    query = query.Where(c => c.Price.Value >= Convert.ToDecimal(MinPrice));
                }
                if (MaxPrice != -1)
                {
                    query = query.Where(c => c.Price.Value <= Convert.ToDecimal(MaxPrice));
                }
                if (!string.IsNullOrEmpty(key))
                {
                    query = query.Where(c => c.Title.Contains(key) || c.Phone.Contains(key) || c.DistictName.Contains(key));
                }
                if (!IsRepeat)
                {
                    query = query.Where(c => !c.IsRepeat);
                }
                #endregion

                total = query.ToList().Count;
                var list = query.OrderByDescending(c => c.CreatedOn).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var listItem = new List<NewsModel>();
                foreach (var newsModel in list)
                {
                    newsModel.RepeatTotal = CountRepeatByPhone(newsModel.Phone, UserId);
                    newsModel.Iscc = CheckCC(newsModel.Id);
                    newsModel.IsReason = CheckReason(UserId, newsModel.Id);
                    listItem.Add(newsModel);
                }

                return listItem;

            }
        }

        //Export excel
        public List<NewsModel> ExportExcel(List<int> listNewsId)
        {
            using (var tran = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            }))
            {
                var query = (from c in db.News
                             join d in db.Districts on c.DistrictId equals d.Id
                             join t in db.NewsStatus on c.StatusId equals t.Id
                             join ncm in db.News_Customer_Mappings on c.Id equals ncm.NewsId into temp
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
            var query = (from c in db.News
                         join d in db.Districts on c.DistrictId equals d.Id
                         join t in db.NewsStatus on c.StatusId equals t.Id
                         join ct in db.Categories on c.CategoryId equals ct.Id
                         join st in db.Sites on c.SiteId equals st.ID
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
                             SameNews = GetSameNewsByNewsId(c.CategoryId.Value, c.DistrictId.Value, UserId),
                             Iscc = CheckCCByUser(c.Id, UserId)
                         }).FirstOrDefault();

            if (query != null)
            {

                var getnewsave = (from c in db.News_Customer_Mappings
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
                    db.News_Customer_Mappings.InsertOnSubmit(newItem);

                }
                else
                {
                    foreach (var newsCustomerMapping in getnewsave)
                    {
                        newsCustomerMapping.IsReaded = true;
                    }
                }
                db.SubmitChanges();
            }
            return query;
        }

        public New GetNewsDetail(int Id)
        {
            var query = (from c in db.News
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
                    var query = (from c in db.News_Customer_Mappings
                                 where c.NewsId.Equals(item.NewsId) && c.CustomerId.Equals(userId)
                                 select c).FirstOrDefault();
                    if (query == null)
                    {
                        db.News_Customer_Mappings.InsertOnSubmit(item);
                    }
                    else
                    {
                        query.IsSaved = true;
                        query.IsDeleted = false;
                    }
                }
                db.SubmitChanges();
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
                    var query = (from c in db.News_Customer_Mappings
                                 where c.NewsId.Equals(item.NewsId) && c.CustomerId.Equals(userId)
                                 select c).ToList();
                    if (!query.Any())
                    {
                        db.News_Customer_Mappings.InsertOnSubmit(item);
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
                db.SubmitChanges();
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public List<ImageModel> GetImageByNewsId(int NewsID)
        {
            var query = (from c in db.Images
                         where c.NewsId.Equals(NewsID)
                         select new ImageModel { Id = c.Id, NewsId = c.NewsId, ImageUrl = c.ImageUrl }).ToList();
            return query;
        }

        public List<NewsModel> GetSameNewsByNewsId(int CateId, int DistricId, int UserId)
        {
            var news_new = (from c in db.News_Customer_Mappings
                            where c.CustomerId.Equals(UserId)
                            select (c.NewsId)).ToList();

            var query = (from c in db.News
                         join d in db.Districts on c.DistrictId equals d.Id
                         join t in db.NewsStatus on c.StatusId equals t.Id
                         join st in db.Sites on c.SiteId equals st.ID
                         where c.CreatedOn.HasValue && !c.IsDeleted //&& c.Published.HasValue
                         && !d.IsDeleted && d.Published
                         && !news_new.Contains(c.Id)
                         && c.CategoryId.Equals(CateId)
                         && c.DistrictId.Equals(DistricId)
                         orderby c.StatusId ascending, c.Price descending
                         select new NewsModel
                         {
                             Id = c.Id,
                             Title = c.Title,
                             CategoryId = c.CategoryId,
                             Link = c.Link,
                             SiteName = st.Name,
                             Phone = c.Phone,
                             Price = c.Price,
                             PriceText = c.PriceText,
                             DistrictId = d.Id,
                             SiteId = c.SiteId,
                             DistictName = d.Name,
                             StatusId = t.Id,
                             StatusName = t.Name,
                             CreatedOn = c.CreatedOn
                         }).Skip(0).Take(3).ToList();
            return query;
        }

        public int ReportNews(List<New> listNewsReport, int userReport)
        {
            try
            {
                foreach (var item in listNewsReport)
                {
                    var reportItem = new NewsReport();
                    reportItem.StatusId = Convert.ToInt32(item.StatusId);
                    reportItem.NewsId = item.Id;
                    reportItem.CreateDate = DateTime.Now;
                    reportItem.Notes = "Tin mô giới: " + item.Title;
                    reportItem.CustomerId = userReport;
                    db.NewsReports.InsertOnSubmit(reportItem);
                }
                db.SubmitChanges();
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
                    var reportItem = new CMS.Data.Blacklist();
                    reportItem.Words = item.Phone;
                    reportItem.Description = "Tin mô giới: " + item.Title;
                    reportItem.LinkUrl = item.Link;
                    reportItem.CreatedOn = DateTime.Now;
                    reportItem.Type = 1;
                    db.Blacklists.InsertOnSubmit(reportItem);

                    var query = (from c in db.News_customer_actions
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
                            Iscc = false,
                            IsSpam = false,
                            Ischeck = false,
                            IsReport = true,
                            DateCreate = DateTime.Now
                        };
                        db.News_customer_actions.InsertOnSubmit(action);
                    }
                }
                db.SubmitChanges();
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
                    var query = (from c in db.News_Customer_Mappings
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

                    var query2 = (from c in db.News_Trashes
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
                        db.News_Trashes.InsertOnSubmit(itemtrash);
                    }
                    db.SubmitChanges();
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
            var listBlacklist = (from c in db.Blacklists
                                 select (c.Words)).ToList();

            var news_new = new List<int>();
            if (GetRoleByUser(userId) == Convert.ToInt32(CmsRole.Administrator))
            {
                news_new = (from c in db.News_Customer_Mappings
                            where (c.IsDeleted.Value || c.IsSaved.Value) //c.CustomerId.Equals(userId) &&
                            select (c.NewsId)).ToList();
            }
            else
            {
                news_new = (from c in db.News_Customer_Mappings
                            where c.CustomerId.Equals(userId) && (c.IsDeleted.Value || c.IsSaved.Value)
                            select (c.NewsId)).ToList();
            }

            var news = GetNewsDetail(newId);
            if (news != null)
            {
                var query = from c in db.News
                            join d in db.Districts on c.DistrictId equals d.Id
                            join t in db.NewsStatus on c.StatusId equals t.Id
                            join ncm in db.News_Customer_Mappings on c.Id equals ncm.NewsId into temp
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
                var listBlacklist = (from c in db.Blacklists
                                     select (c.Words)).ToList();
                var listDelete = (from c in db.News_Trashes
                                  where (c.Isdelete || c.Isdeleted) && c.CustomerID.Equals(userID)
                                  select (c.NewsId)).ToList();
                var query = (from c in db.News
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
                    var query = (from c in db.News_customer_actions
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
                        db.News_customer_actions.InsertOnSubmit(item);
                    }
                    db.SubmitChanges();
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
                    var query = (from c in db.News_customer_actions
                        where c.NewsId.Equals(t) && c.CustomerId.Equals(userId) && c.Iscc.HasValue && c.Iscc.Value
                        select c).ToList();
                    if (query.Any())
                    {
                        foreach (var newsCustomerAction in query)
                        {
                            db.News_customer_actions.DeleteOnSubmit(newsCustomerAction);
                        }
                    }
                    db.SubmitChanges();
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
            var query = (from c in db.News_customer_actions
                where c.NewsId.Equals(newsId) && c.Iscc.HasValue && c.Iscc.Value
                select c).ToList();
            if (query.Any())
                return true;
            return false;
        }

        public bool CheckReason(int userId, int newsId)
        {
            return db.ReasonReportNews.Any(x => x.UserId == userId && x.NewsId == newsId);
        }

        public bool CheckCCByUser(int newsId, int userId)
        {
            var query = (from c in db.News_customer_actions
                         where c.NewsId.Equals(newsId) && c.Iscc.HasValue && c.Iscc.Value && c.CustomerId.Equals(userId)
                         select c).ToList();
            if (query.Any())
                return true;
            return false;
        }
        #endregion

        #region Role

        public int GetRoleByUser(int userId)
        {
            try
            {
                var query = (from c in db.Role_Users
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

        #region ReasonReportNews
        public void InsertReasonReportNews(ReasonReportNew model)
        {
            db.ReasonReportNews.InsertOnSubmit(model);
            db.SubmitChanges();
        }

        public void DeleteReasonReportNews(int newsId, int userId)
        {
            var check = db.ReasonReportNews.FirstOrDefault(x => x.UserId == userId && x.NewsId == newsId);
            if(check != null)
            {
                db.ReasonReportNews.DeleteOnSubmit(check);
                db.SubmitChanges();
            }
        }
        #endregion
    }
}