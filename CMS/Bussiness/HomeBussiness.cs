using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using CMS.Data;
using CMS.Models;

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
            int BackDate, string From, string To, double MinPrice, double MaxPrice, int pageIndex, int pageSize, ref int total)
        {
            using (var tran = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            }))
            {
                var news_new = (from c in db.News_Customer_Mappings
                                where c.CustomerId.Equals(UserId)
                                select (c.NewsId)).ToList();

                var query = from c in db.News
                            join d in db.Districts on c.DistrictId equals d.Id
                            join t in db.NewsStatus on c.StatusId equals t.Id
                            join ncm in db.News_Customer_Mappings on c.Id equals ncm.NewsId into temp
                            from tm in temp.DefaultIfEmpty()
                            where c.CreatedOn.HasValue && !c.IsDeleted //&& c.Published.HasValue
                                  && !d.IsDeleted && d.Published
                                  && !news_new.Contains(c.Id)
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
                    query = query.Where(c => c.CreatedOn >= Convert.ToDateTime(From));
                }
                if (!string.IsNullOrEmpty(To))
                {
                    query = query.Where(c => c.CreatedOn <= Convert.ToDateTime(To));
                }
                if (MinPrice != -1)
                {
                    query = query.Where(c => c.Price.Value >= Convert.ToDecimal(MinPrice));
                }
                if (MaxPrice != -1)
                {
                    query = query.Where(c => c.Price.Value <= Convert.ToDecimal(MaxPrice));
                }
                #endregion

                total = query.ToList().Count;
                return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

                var getnewsave = from c in db.News_Customer_Mappings
                                 where c.NewsId.Equals(query.Id) && c.IsReaded.Value
                                 select new
                                 {
                                     Id = c.Id
                                 };
                var news = getnewsave.FirstOrDefault();
                if (news == null)
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
                    db.SubmitChanges();
                }
            }
            return query;
        }

        public int SaveNewByUserId(List<News_Customer_Mapping> cusNews, int userId)
        {
            try
            {
                foreach (var item in cusNews)
                {
                    var query = (from c in db.News_Customer_Mappings
                                 where c.NewsId.Equals(item.NewsId) && c.IsSaved.Value
                                 select new
                                 {
                                     Id = c.Id
                                 }).FirstOrDefault();
                    if (query == null)
                    {
                        db.News_Customer_Mappings.InsertOnSubmit(item);
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
                                 where c.NewsId.Equals(item.NewsId) && c.IsDeleted.Value
                                 select new
                                 {
                                     Id = c.Id
                                 }).FirstOrDefault();
                    if (query == null)
                    {
                        db.News_Customer_Mappings.InsertOnSubmit(item);
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

        #endregion
    }
}