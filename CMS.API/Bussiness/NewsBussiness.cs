﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.API.Models;
using CMS.API.Data;

namespace CMS.API.Bussiness
{
    public class NewsBussiness : InitDB
    {
        #region Home
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

        public List<NewsModel> GetListNewByFilter(int UserId, int CateId, int DistricId, int StatusId, int SiteId,
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

            var listItem =
                (Instance.PROC_GetListNewsInHome(UserId, CateId, DistricId, StatusId, SiteId, backdate, from, to,
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
                             SameNews = GetSameNewsByNewsId(Id, c.CategoryId.Value, c.DistrictId.Value, c.Phone, UserId),
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
        #endregion

        #region Help Method
        public List<ImageModel> GetImageByNewsId(int NewsID)
        {
            var query = (from c in Instance.Images
                         where c.NewsId.Equals(NewsID)
                         select new ImageModel { Id = c.Id, NewsId = c.NewsId, ImageUrl = c.ImageUrl }).ToList();
            return query;
        }
        public List<NewsModel> GetSameNewsByNewsId(int Id, int CateId, int DistricId, string phone, int UserId)
        {
            var query = (from c in Instance.News
                         join d in Instance.Districts on c.DistrictId equals d.Id
                         join t in Instance.NewsStatus on c.StatusId equals t.Id
                         join st in Instance.Sites on c.SiteId equals st.ID
                         where
                         c.Phone.Contains(phone) && c.Phone != ""
                         && !c.Id.Equals(Id)
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
        public bool CheckCCByUser(int newsId, int userId)
        {
            var query = (from c in Instance.News_customer_actions
                         where c.NewsId.Equals(newsId) && c.Iscc.HasValue && c.Iscc.Value && c.CustomerId.Equals(userId)
                         select c).ToList();
            if (query.Any())
                return true;
            return false;
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
                        + "(" + item.DateCreate.ToString("dd/MM/yyyy") + "</br></span></li>";
                }
            }
            else
            {
                rs = "Chưa có ai";
            }
            return rs;
        }
        public bool CheckReason(int userId, int newsId)
        {
            return Instance.ReasonReportNews.Any(x => x.UserId == userId && x.NewsId == newsId);
        }
        #endregion
    }
}