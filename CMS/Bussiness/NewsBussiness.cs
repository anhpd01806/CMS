using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using CMS.Data;
using CMS.Models;
using CMS.Helper;

namespace CMS.Bussiness
{
    public class NewsBussiness
    {
        #region member
        private readonly CmsDataDataContext db = new CmsDataDataContext();
        private readonly HomeBussiness _homeBussiness = new HomeBussiness();
        private readonly PaymentBussiness _payment = new PaymentBussiness();
        #endregion

        #region News save

        public List<NewsModel> GetListNewStatusByFilter(int UserId, int CateId, int DistricId, int StatusId, int SiteId,
            int BackDate, string From, string To, double MinPrice, double MaxPrice, int pageIndex, int pageSize, int newsStatus, bool IsRepeat, string key, ref int total)
        {
            using (var tran = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            }))
            {
                var listBlacklist = (from c in db.Blacklists
                                     select (c.Words)).ToList();

                var news_new = new List<int>();

                if (newsStatus != Convert.ToInt32(Helper.NewsStatus.IsDelete))
                {
                    news_new = (from c in db.News_Customer_Mappings
                                where c.CustomerId.Equals(UserId) && !c.IsDeleted.Value
                                select (c.NewsId)).ToList();
                }
                else
                {
                    news_new = (from c in db.News_Customer_Mappings
                                where c.CustomerId.Equals(UserId) && c.IsDeleted.Value
                                select (c.NewsId)).ToList();
                }

                //Danh sách tin đã đọc theo user
                var news_isread = (from c in db.News_Customer_Mappings
                                   where c.CustomerId.Equals(UserId) && c.IsReaded.Value
                                   select (c.NewsId)).ToList();

                var query = from c in db.News
                            join d in db.Districts on c.DistrictId equals d.Id
                            join t in db.NewsStatus on c.StatusId equals t.Id
                            join ncm in db.News_Customer_Mappings on c.Id equals ncm.NewsId
                            join s in db.Sites on c.SiteId equals s.ID
                            where c.CreatedOn.HasValue && !c.IsDeleted && !s.Deleted && s.Published //&& c.Published.HasValue
                            && !d.IsDeleted && d.Published
                            && news_new.Contains(c.Id)
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
                                CusIsReaded = news_isread.Contains(c.Id) ? true : false,
                                CusIsSaved = ncm.IsSaved,
                                CusIsDeleted = ncm.IsDeleted,
                                IsRepeat = c.IsRepeat,
                                RepeatTotal = 1,//CountRepeatnews(c.Id, UserId, d.Id),
                                IsAdmin = GetRoleByUser(UserId) == Convert.ToInt32(CmsRole.Administrator) ? true : false
                            };

                if (newsStatus == Convert.ToInt32(Helper.NewsStatus.IsSave))
                {
                    query = query.Where(c => c.CusIsSaved.HasValue && c.CusIsSaved.Value && !c.CusIsDeleted.Value);
                }
                if (newsStatus == Convert.ToInt32(Helper.NewsStatus.IsDelete))
                {
                    query = query.Where(c => c.CusIsDeleted.HasValue && c.CusIsDeleted.Value);
                }
                if (newsStatus == Convert.ToInt32(Helper.NewsStatus.IsRead))
                {
                    query = query.Where(c => c.CusIsReaded.HasValue && c.CusIsReaded.Value);
                }

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
                    query = query.Where(c => c.CreatedOn <= Convert.ToDateTime((To.Split('-')[2] + "/" + To.Split('-')[1] + "/" + To.Split('-')[0] + " 23:59:59.999")));
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

                total = query.Distinct().ToList().Count;
                return query.Distinct().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        public int RemoveSaveNewByUserId(List<News_Customer_Mapping> cusNews, int userId)
        {
            try
            {
                foreach (var item in cusNews)
                {
                    var query = (from c in db.News_Customer_Mappings
                                 where c.NewsId.Equals(item.NewsId) && c.IsSaved.Value && c.CustomerId.Equals(userId)
                                 select c).ToList();
                    if (query != null)
                    {
                        foreach (var newsCustomerMapping in query)
                        {
                            if (newsCustomerMapping.IsDeleted.Value || newsCustomerMapping.IsReaded.Value)
                            {
                                newsCustomerMapping.IsSaved = false;
                            }
                            else
                            {
                                db.News_Customer_Mappings.DeleteOnSubmit(newsCustomerMapping);
                            }
                        }
                        db.SubmitChanges();
                    }
                }

                return 1;
            }
            catch
            {
                return 0;
            }
        }

        #endregion

        #region News Hide
        public int RemoveHideNewByUserId(List<News_Customer_Mapping> cusNews, int userId)
        {
            try
            {
                foreach (var item in cusNews)
                {
                    var query = (from c in db.News_Customer_Mappings
                                 where c.NewsId.Equals(item.NewsId) && c.IsDeleted.Value && c.CustomerId.Equals(userId)
                                 select c).ToList();
                    if (query != null)
                    {
                        foreach (var newsCustomerMapping in query)
                        {
                            if (newsCustomerMapping.IsSaved.Value || newsCustomerMapping.IsReaded.Value)
                            {
                                newsCustomerMapping.IsDeleted = false;
                            }
                            else
                            {
                                db.News_Customer_Mappings.DeleteOnSubmit(newsCustomerMapping);
                            }
                        }
                        db.SubmitChanges();
                    }
                }

                return 1;
            }
            catch
            {
                return 0;
            }
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

        #region Edit and Insert news

        public int Createnew(New newsItem, int userId)
        {
            try
            {
                db.News.InsertOnSubmit(newsItem);
                db.SubmitChanges();

                var cusNews = new News_Customer_Mapping
                {
                    CustomerId = userId,
                    NewsId = newsItem.Id,
                    IsSaved = false,
                    IsDeleted = false,
                    IsReaded = false,
                    IsAgency = true,
                    IsSpam = false,
                    CreateDate = DateTime.Now
                };
                db.News_Customer_Mappings.InsertOnSubmit(cusNews);
                db.SubmitChanges();

                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public List<NewsModel> GetListNewNotActiveByFilter(int CateId, int DistricId, int StatusId, int SiteId,
            int BackDate, string From, string To, double MinPrice, double MaxPrice, int pageIndex, int pageSize, bool IsRepeat, string key, ref int total)
        {
            var minPayment = Convert.ToDouble(ConfigWeb.MinPayment);
            var query = from c in db.News
                            join d in db.Districts on c.DistrictId equals d.Id
                            join t in db.NewsStatus on c.StatusId equals t.Id
                            join ncm in db.News_Customer_Mappings on c.Id equals ncm.NewsId
                            join s in db.Sites on c.SiteId equals s.ID
                            join u in db.Users on ncm.CustomerId equals u.Id
                            where ncm.IsAgency.HasValue && ncm.IsAgency.Value
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
                            Cusname = u.FullName,
                            IsPayment =  Convert.ToDouble(_payment.GetCashPaymentByUserId(ncm.CustomerId)) >= minPayment ? true : false
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
                query = query.Where(c => c.CreatedOn <= Convert.ToDateTime((To.Split('-')[2] + "/" + To.Split('-')[1] + "/" + To.Split('-')[0] + " 23:59:59.999")));
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
            //if (!IsRepeat)
            //{
            //    query = query.Where(c => !c.IsRepeat);
            //}
            #endregion

            total = query.ToList().Count;
            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

        }
        #endregion

        #region Help
        public int CountRepeatnews(int newId, int userId, int districId)
        {
            var listBlacklist = (from c in db.Blacklists
                                 select (c.Words)).ToList();

            //Danh sách tin đã lưu hoặc đã ẩn theo user
            var news_new = (from c in db.News_Customer_Mappings
                            where c.CustomerId.Equals(userId) && (c.IsDeleted.Value || c.IsSaved.Value)
                            select (c.NewsId)).ToList();

            //Danh sách tin đã đọc theo user
            var news_isread = (from c in db.News_Customer_Mappings
                               where c.CustomerId.Equals(userId) && c.IsReaded.Value
                               select (c.NewsId)).ToList();
            var news = _homeBussiness.GetNewsDetail(newId);
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

        public int CheckRepeatNews(string phone, int districId, int userId)
        {
            var listBlacklist = (from c in db.Blacklists
                                 select (c.Words)).ToList();
            var query = (from c in db.News
                         join d in db.Districts on c.DistrictId equals d.Id
                         where c.CreatedOn.HasValue && !c.IsDeleted
                         && !d.IsDeleted && d.Published
                         && c.DistrictId.Equals(districId)
                         && c.Phone.Contains(phone)
                         && !listBlacklist.Contains(c.Phone)
                         select c).ToList();

            var total = query.Count;
            return total;
        }
        #endregion        

        public New GetNewsById(int id)
        {
            return db.News.FirstOrDefault(x => x.Id == id);
        }
    }
}