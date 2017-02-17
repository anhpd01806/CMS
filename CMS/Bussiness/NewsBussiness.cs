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
        #endregion

        #region News save

        public List<NewsModel> GetListNewStatusByFilter(int UserId, int CateId, int DistricId, int StatusId, int SiteId,
            int BackDate, string From, string To, double MinPrice, double MaxPrice, int pageIndex, int pageSize, int newsStatus, ref int total)
        {
            using (var tran = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            }))
            {
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
                                where c.CustomerId.Equals(UserId)
                                select (c.NewsId)).ToList();
                }

                var query = from c in db.News
                            join d in db.Districts on c.DistrictId equals d.Id
                            join t in db.NewsStatus on c.StatusId equals t.Id
                            join ncm in db.News_Customer_Mappings on c.Id equals ncm.NewsId
                            where c.CreatedOn.HasValue && !c.IsDeleted //&& c.Published.HasValue
                            && !d.IsDeleted && d.Published
                            && news_new.Contains(c.Id)
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
                                CusIsReaded = ncm.IsReaded,
                                CusIsSaved = ncm.IsSaved,
                                CusIsDeleted = ncm.IsDeleted
                            };

                if (newsStatus == Convert.ToInt32(CMS.Helper.NewsStatus.IsSave))
                {
                    query = query.Where(c => c.CusIsSaved.HasValue && c.CusIsSaved.Value && !c.CusIsDeleted.Value);
                }
                if (newsStatus == Convert.ToInt32(CMS.Helper.NewsStatus.IsDelete))
                {
                    query = query.Where(c => c.CusIsDeleted.HasValue && c.CusIsDeleted.Value );
                }
                if (newsStatus == Convert.ToInt32(CMS.Helper.NewsStatus.IsRead))
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

        public int RemoveSaveNewByUserId(List<News_Customer_Mapping> cusNews, int userId)
        {
            try
            {
                foreach (var item in cusNews)
                {
                    var query = (from c in db.News_Customer_Mappings
                                 where c.NewsId.Equals(item.NewsId) && c.IsSaved.Value
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
                                 where c.NewsId.Equals(item.NewsId) && c.IsDeleted.Value
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
    }
}