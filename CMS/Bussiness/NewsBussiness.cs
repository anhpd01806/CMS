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
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;

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
            int BackDate, string From, string To, double MinPrice, double MaxPrice, int pageIndex, int pageSize, int newsStatus, bool IsRepeat, string key, string NameOrder, bool descending, ref int total)
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
                                where c.CustomerId.Equals(UserId) && !c.IsDeleted.Value && !c.IsAgency.Value
                                select (c.NewsId)).ToList();
                }
                else
                {
                    news_new = (from c in db.News_Customer_Mappings
                                where c.CustomerId.Equals(UserId) && c.IsDeleted.Value && !c.IsAgency.Value
                                select (c.NewsId)).ToList();
                }

                //Danh sách tin đã đọc theo user
                var news_isread = (from c in db.News_Customer_Mappings
                                   where c.CustomerId.Equals(UserId) && c.IsReaded.Value && !c.IsAgency.Value
                                   select (c.NewsId)).ToList();

                //Danh sách tin đã bị xóa
                var listDelete = (from c in db.News_Trashes
                                  where (c.Isdelete || c.Isdeleted) && c.CustomerID.Equals(UserId)
                                  select (c.NewsId)).ToList();

                var query = (from c in db.News
                            join d in db.Districts on c.DistrictId equals d.Id
                            join t in db.NewsStatus on c.StatusId equals t.Id
                            join ncm in db.News_Customer_Mappings on c.Id equals ncm.NewsId
                            join s in db.Sites on c.SiteId equals s.ID
                            join ac in db.News_customer_actions on c.Id equals ac.NewsId into temp2
                            from nac in temp2.DefaultIfEmpty()
                            where c.CreatedOn.HasValue && !c.IsDeleted && !c.IsSpam && !s.Deleted && s.Published
                            && !d.IsDeleted && d.Published
                            && news_new.Contains(c.Id)
                            && !listDelete.Contains(c.Id)
                            select new NewsModel
                            {
                                Id = c.Id,
                                Title = c.Title,
                                CategoryId = c.CategoryId,
                                SiteId = c.SiteId,
                                Contents = c.Contents,
                                Link = c.Link,
                                Phone = c.Phone,
                                Price = c.Price,
                                PriceText = c.PriceText,
                                DistrictId = d.Id,
                                DistictName = d.Name,
                                StatusId = t.Id,
                                StatusName = t.Name,
                                CreatedOn = c.CreatedOn,
                                CusIsReaded = news_isread.Contains(c.Id),
                                CusIsSaved = ncm.IsSaved,
                                CusIsDeleted = ncm.IsDeleted,
                                IsRepeat = c.IsRepeat,
                                RepeatTotal = c.TotalRepeat.HasValue ? c.TotalRepeat.Value : 1,
                                Iscc = nac.Iscc.HasValue && nac.Iscc.Value,
                                IsReason = false//CheckReason(UserId, c.Id)
                            }).Distinct();

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
                    query = BackDate == 0 ? query.Where(c => c.CreatedOn == Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00.00")) : query.Where(c => c.CreatedOn >= Convert.ToDateTime(DateTime.Now.AddDays(-BackDate).ToString("yyyy/MM/dd") + " 00:00:00.00"));
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
                if (string.IsNullOrEmpty(NameOrder))
                {
                    query = query.OrderByDescending(c => c.CreatedOn);
                }
                else
                {
                    query = descending ? QueryableHelper.OrderByDescending(query, NameOrder) : QueryableHelper.OrderBy(query, NameOrder);
                }
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
                var query = "INSERT INTO News (CategoryId,Title,Contents,Summary,Link,SiteId,DistrictId,ProvinceId,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,StatusId,Phone,IsUpdated,DateOld,IsDeleted,IsPhone,PriceText,IsRepeat,Area,Price,IsSpam,TotalRepeat)" +
                           "VALUES(" + newsItem.CategoryId + ", N'" + newsItem.Title + "', N'" + newsItem.Contents + "', N'" + newsItem.Contents + "', '" + newsItem.Link + "', " + newsItem.SiteId + ", " + newsItem.DistrictId +
                           "," + newsItem.ProvinceId + "," + (newsItem.CreatedBy ?? 0) + ",'" + (newsItem.CreatedOn ?? DateTime.Now) + "'," + (newsItem.ModifiedBy ?? 0) + ",'" + (newsItem.ModifiedOn ?? DateTime.Now) + "' ," + (newsItem.StatusId ?? 0) + ",'" + newsItem.Phone + "',0,'" + newsItem.CreatedOn +
                           "',0," + (newsItem.IsPhone ? 1 : 0) + ",N'" + newsItem.PriceText + "',0," + newsItem.Area + "," + newsItem.Price + ",0,1);";

                db.ExecuteCommand(query);

                return 1;
            }
            catch (Exception ex)
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
                        join ncm in db.News_Customer_Mappings on c.Id equals ncm.NewsId
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
                            StatusId = c.StatusId,
                            CreatedOn = c.CreatedOn,
                            Cusname = u.FullName,
                            CusId = u.Id,
                            IsPayment = false
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
            //if (!IsRepeat)
            //{
            //    query = query.Where(c => !c.IsRepeat);
            //}
            #endregion

            total = query.ToList().Count;
            var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var newslist = new List<NewsModel>();
            if (list.Any())
            {
                foreach (var item in list)
                {
                    item.IsPayment = Convert.ToDouble(string.IsNullOrEmpty(_payment.GetCashPaymentByUserId(item.CusId)) ? "0" : _payment.GetCashPaymentByUserId(item.CusId)) >= minPayment ? true : false;
                    newslist.Add(item);
                }
            }
            return newslist;

        }
        #endregion

        #region Active or delete news
        /// <summary>
        /// Status:  0 thất bại
        ///          1 thành công
        ///          2 tin không tồn tại
        ///          3 không đủ tiền thanh toán
        ///          4 dữ liệu truyền vào bị null
        /// </summary>
        /// <param name="newsId"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        public int ActiveOrDelete(string[] newsId, bool isDelete)
        {
            try
            {
                if (newsId.Length > 0)
                {
                    for (int i = 0; i < newsId.Length; i++)
                    {

                        var userid = Convert.ToInt32(string.IsNullOrEmpty(newsId[i].Split('-')[1]) ? "0" : newsId[i].Split('-')[1]);
                        var newsid = Convert.ToInt32(string.IsNullOrEmpty(newsId[i].Split('-')[0]) ? "0" : newsId[i].Split('-')[0]);
                        var minPayment = Convert.ToDouble(ConfigWeb.MinPayment);
                        var totalMoney = Convert.ToDouble(string.IsNullOrEmpty(_payment.GetCashPaymentByUserId(userid)) ? "0" : _payment.GetCashPaymentByUserId(userid));

                        var news = (from c in db.News_Customer_Mappings
                                    where c.CustomerId.Equals(userid) && c.NewsId.Equals(newsid) && c.IsAgency.HasValue && c.IsAgency.Value
                                    select c).ToList();
                        if (news.Any())
                        {
                            foreach (var item in news)
                            {
                                if (isDelete)
                                {
                                    db.News_Customer_Mappings.DeleteOnSubmit(item);
                                    db.News.DeleteOnSubmit(GetNewsById(item.NewsId));
                                }
                                else
                                {
                                    if (totalMoney >= minPayment)
                                    {
                                        db.News_Customer_Mappings.DeleteOnSubmit(item);
                                    }
                                    else
                                    {
                                        return 3;
                                    }
                                }
                            }
                            db.SubmitChanges();
                            return 1;
                        }
                        return 2;
                    }
                }
                return 4;
            }
            catch
            {
                return 0;
            }
        }

        public NewsModel GetNewsDetail(int Id, int UserId)
        {
            var query = (from c in db.News
                         join d in db.Districts on c.DistrictId equals d.Id
                         join ncm in db.News_Customer_Mappings on c.Id equals ncm.NewsId
                         join u in db.Users on ncm.CustomerId equals u.Id
                         where c.CreatedOn.HasValue
                         && c.Id.Equals(Id)
                         select new NewsModel
                         {
                             Id = c.Id,
                             Title = c.Title,
                             Link = c.Link,
                             SiteId = c.SiteId,
                             Phone = c.Phone,
                             Contents = c.Contents,
                             Price = c.Price,
                             PriceText = c.PriceText,
                             DistrictId = d.Id,
                             DistictName = d.Name,
                             CreatedOn = c.CreatedOn,
                             Cusname = u.FullName,
                             CusId = u.Id,
                             Iscc = CheckCCByUser(c.Id, UserId)
                         }).FirstOrDefault();

            return query;
        }
        #endregion

        #region News delete
        public List<NewsModel> GetListNewDeleteByFilter(int UserId, int CateId, int DistricId, int StatusId, int SiteId,
            int BackDate, string From, string To, double MinPrice, double MaxPrice, int pageIndex, int pageSize, bool IsRepeat, string key, string NameOrder, bool descending, ref int total)
        {
            using (var tran = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            }))
            {
                //Danh sách tin đã đọc theo user
                var news_isread = (from c in db.News_Customer_Mappings
                                   where c.CustomerId.Equals(UserId) && c.IsReaded.Value && !c.IsAgency.Value
                                   select (c.NewsId)).ToList();

                var query = (from c in db.News
                            join d in db.Districts on c.DistrictId equals d.Id
                            join t in db.NewsStatus on c.StatusId equals t.Id
                            join nd in db.News_Trashes on c.Id equals nd.NewsId
                            join ac in db.News_customer_actions on c.Id equals ac.NewsId into temp2
                            from nac in temp2.DefaultIfEmpty()
                            where c.CreatedOn.HasValue && !c.IsDeleted && !c.IsSpam //&& c.Published.HasValue
                            && !d.IsDeleted && d.Published
                            && nd.Isdelete && !nd.Isdeleted
                            select new NewsModel
                            {
                                Id = c.Id,
                                Title = c.Title,
                                CategoryId = c.CategoryId,
                                SiteId = c.SiteId,
                                Contents = c.Contents,
                                Link = c.Link,
                                Phone = c.Phone,
                                Price = c.Price,
                                PriceText = c.PriceText,
                                DistrictId = d.Id,
                                DistictName = d.Name,
                                StatusId = t.Id,
                                StatusName = t.Name,
                                CreatedOn = c.CreatedOn,
                                CusIsReaded = news_isread.Contains(c.Id),
                                IsRepeat = c.IsRepeat,
                                RepeatTotal = c.TotalRepeat.HasValue ? c.TotalRepeat.Value : 1,
                                Iscc = nac.Iscc.HasValue && nac.Iscc.Value,
                                IsReason = false//CheckReason(UserId, c.Id)
                            }).Distinct();

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
                    query = BackDate == 0 ? query.Where(c => c.CreatedOn == Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00.00")) : query.Where(c => c.CreatedOn >= Convert.ToDateTime(DateTime.Now.AddDays(-BackDate).ToString("yyyy/MM/dd") + " 00:00:00.00"));
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
                if (string.IsNullOrEmpty(NameOrder))
                {
                    query = query.OrderByDescending(c => c.CreatedOn);
                }
                else
                {
                    query = descending ? QueryableHelper.OrderByDescending(query, NameOrder) : QueryableHelper.OrderBy(query, NameOrder);
                }
                return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        public int RestoreNews(int[] listnews, int userId)
        {
            try
            {
                foreach (var t in listnews)
                {
                    var query = (from c in db.News_Trashes
                        where c.NewsId.Equals(t) && c.CustomerID.Equals(userId) && !c.Isdeleted
                        select c).ToList();
                    if (query.Any())
                    {
                        foreach (var newsTrash in query)
                        {
                            db.News_Trashes.DeleteOnSubmit(newsTrash);
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

        public int DeleteNews(int[] listnews, int userId)
        {
            try
            {
                foreach (var t in listnews)
                {
                    var query = (from c in db.News_Trashes
                        where c.NewsId.Equals(t) && c.CustomerID.Equals(userId) && !c.Isdeleted && !c.IsSpam
                        select c).ToList();
                    if (query.Any())
                    {
                        foreach (var newsTrash in query)
                        {
                            newsTrash.Isdelete = false;
                            newsTrash.Isdeleted = true;
                            newsTrash.IsSpam = false;
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
        #endregion

        #region Brokers Information

        public List<NewsModel> GetListBrokersInformationByFilter(int UserId, int CateId, int DistricId, int StatusId, int SiteId,
            int BackDate, string From, string To, double MinPrice, double MaxPrice, int pageIndex, int pageSize, bool IsRepeat, string key, string NameOrder, bool descending, ref int total)
        {
            using (var tran = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            }))
            {
                //var listBlacklist = (from c in db.Blacklists
                //                     select (c.Words)).ToList();
                
                //Danh sách tin đã đọc theo user
                var news_isread = (from c in db.News_Customer_Mappings
                                   where c.CustomerId.Equals(UserId) && c.IsReaded.Value && !c.IsAgency.Value
                                   select (c.NewsId)).ToList();

                //Danh sách tin đã bị xóa
                var listDelete = (from c in db.News_Trashes
                                  where (c.Isdelete || c.Isdeleted) && c.CustomerID.Equals(UserId)
                                  select (c.NewsId)).ToList();

                var query = from c in db.News
                            join s in db.Sites on c.SiteId equals s.ID
                            join d in db.Districts on c.DistrictId equals d.Id
                            join ac in db.News_customer_actions on c.Id equals ac.NewsId into temp2
                            from nac in temp2.DefaultIfEmpty()
                            where  !listDelete.Contains(c.Id) && c.IsSpam
                            orderby c.CreatedOn descending
                            select new NewsModel
                            {
                                Id = c.Id,
                                Title = c.Title,
                                CategoryId = c.CategoryId,
                                SiteId = c.SiteId,
                                Contents = c.Contents,
                                Link = c.Link,
                                Phone = c.Phone,
                                Price = c.Price,
                                PriceText = c.PriceText,
                                DistrictId = d.Id,
                                DistictName = d.Name,
                                SiteName = s.Name,
                                CreatedOn = c.CreatedOn,
                                StatusId = c.StatusId,
                                CusIsReaded = news_isread.Contains(c.Id),
                                IsRepeat = c.IsRepeat,
                                RepeatTotal = c.TotalRepeat.HasValue ? c.TotalRepeat.Value : 1,
                                Iscc = nac.Iscc.HasValue && nac.Iscc.Value,
                                IsReason = false//CheckReason(UserId, c.Id)
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
                    query = BackDate == 0 ? query.Where(c => c.CreatedOn == Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00.00")) : query.Where(c => c.CreatedOn >= Convert.ToDateTime(DateTime.Now.AddDays(-BackDate).ToString("yyyy/MM/dd") + " 00:00:00.00"));
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
                if (string.IsNullOrEmpty(NameOrder))
                {
                    query = query.OrderByDescending(c => c.CreatedOn);
                }
                else
                {
                    query = descending ? QueryableHelper.OrderByDescending(query, NameOrder) : QueryableHelper.OrderBy(query, NameOrder);
                }
                return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        public int DeleteBlacklist(int[] listnews)
        {
            try
            {
                foreach (var t in listnews)
                {
                    var query = (from c in db.News
                        where c.Id.Equals(t)
                        select c).ToList();

                    if (query.Any())
                    {
                        foreach (var item in query)
                        {
                            item.IsSpam = false;
                            var qr = (from c in db.Blacklists where c.Words.Equals(item.Phone) select c).FirstOrDefault();
                            if (qr != null)
                            {
                                db.Blacklists.DeleteOnSubmit(qr);
                            }
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
            //var listBlacklist = (from c in db.Blacklists
            //                     select (c.Words)).ToList();
            var query = (from c in db.News
                         join d in db.Districts on c.DistrictId equals d.Id
                         where c.CreatedOn.HasValue && !c.IsDeleted
                         && !d.IsDeleted && d.Published
                         && c.DistrictId.Equals(districId)
                         && c.Phone.Contains(phone)
                         //&& !listBlacklist.Contains(c.Phone)
                         select c).ToList();

            var total = query.Count;
            return total;
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

        public bool CheckCC(int newsId)
        {
            var query = (from c in db.News_customer_actions
                         where c.NewsId.Equals(newsId) && c.Iscc.HasValue && c.Iscc.Value
                         select c).ToList();
            if (query.Any())
                return true;
            return false;
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

        public New GetNewsById(int id)
        {
            return db.News.FirstOrDefault(x => x.Id == id);
        }

        public void UpdateSpam(int id)
        {
            var news = db.News.FirstOrDefault(x => x.Id == id);
            news.IsSpam = true;
            db.SubmitChanges();
        }

        public void UpdateCountNews(string Phone)
        {
            if (!string.IsNullOrEmpty(Phone))
            {
                var newsList = db.News.Where(x => x.Phone.Contains(Phone) && x.IsSpam == false).ToList();
                if (newsList != null)
                {
                    foreach (var item in newsList)
                    {
                        item.TotalRepeat = newsList.Count();
                    }
                    db.SubmitChanges();
                }
            }
        }
    }
}