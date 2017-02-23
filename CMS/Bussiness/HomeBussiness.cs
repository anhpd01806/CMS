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

                //Danh sách tin đã lưu hoặc đã ẩn theo user
                var news_new = (from c in db.News_Customer_Mappings
                                where c.CustomerId.Equals(UserId) && (c.IsDeleted.Value || c.IsSaved.Value || c.IsAgency.Value)
                                select (c.NewsId)).ToList();

                //Danh sách tin đã đọc theo user
                var news_isread = (from c in db.News_Customer_Mappings
                                   where c.CustomerId.Equals(UserId) && c.IsReaded.Value
                                   select (c.NewsId)).ToList();

                var query = from c in db.News
                            join d in db.Districts on c.DistrictId equals d.Id
                            join t in db.NewsStatus on c.StatusId equals t.Id
                            join s in db.Sites on c.SiteId equals s.ID
                            //join ncm in db.News_Customer_Mappings on c.Id equals ncm.NewsId into temp
                            //from tm in temp.DefaultIfEmpty()
                            where c.CreatedOn.HasValue && !c.IsDeleted && !s.Deleted && s.Published //&& c.Published.HasValue
                            && !d.IsDeleted && d.Published

                            && !news_new.Contains(c.Id)
                            && !listBlacklist.Contains(c.Phone) //không cho hiển thị có số điện thoại giống số điện thoại trong blacklist                            
                            orderby c.StatusId ascending, c.Price descending
                            select new NewsModel
                            {
                                Id = c.Id,
                                Title = c.Title,
                                CategoryId = c.CategoryId,
                                SiteId = c.SiteId,
                                Link = c.Link,
                                Phone = c.Phone,
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
                                RepeatTotal = 1,//CountRepeatnews(c.Id, UserId, d.Id),
                                IsAdmin = GetRoleByUser(UserId) == Convert.ToInt32(CmsRole.Administrator) ? true : false
                            };

                #region check param
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
                    if (BackDate == 0)
                    {
                        query = query.Where(c => c.CreatedOn == DateTime.Now);
                    }
                    else
                    {
                        query = query.Where(c => c.CreatedOn >= DateTime.Now.AddDays(-BackDate));
                    }

                }
                if (!string.IsNullOrEmpty(From))
                {
                    query = query.Where(c => c.CreatedOn >= Convert.ToDateTime((From.Split('-')[2] + "/" + From.Split('-')[1] + "/" + From.Split('-')[0] + " 00:00:00.00")));
                }
                if (!string.IsNullOrEmpty(To))
                {
                    query = query.Where(c => c.CreatedOn <= Convert.ToDateTime((To.Split('-')[2] + "/" + To.Split('-')[1] + "/" + To.Split('-')[0] +" 23:59:59.999")));
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
                return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
                            }).ToList();
                return query;
            }
        }

        public NewsModel GetNewsDetail(int Id, int UserId)
        {
            var query = (from c in db.News
                         join d in db.Districts on c.DistrictId equals d.Id
                         join t in db.NewsStatus on c.StatusId equals t.Id
                         join ct in db.Categories on c.CategoryId equals ct.Id
                         where c.CreatedOn.HasValue && !c.IsDeleted //&& c.Published.HasValue
                         && !d.IsDeleted && d.Published
                         && c.Id.Equals(Id)
                         select new NewsModel
                         {
                             Id = c.Id,
                             Title = c.Title,
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
                             CategoryId = ct.Id,
                             CateName = ct.Name,
                             ListImage = GetImageByNewsId(c.Id),
                             SameNews = GetSameNewsByNewsId(c.CategoryId.Value, c.DistrictId.Value, UserId)
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
                    reportItem.CreatedOn = DateTime.Now;
                    reportItem.Type = 1;
                    db.Blacklists.InsertOnSubmit(reportItem);
                }
                db.SubmitChanges();
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
    }
}