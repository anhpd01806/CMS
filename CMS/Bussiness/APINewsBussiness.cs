using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Models;
using CMS.Data;

namespace CMS.Bussiness
{
    public class APINewsBussiness
    {
        #region define
        CmsDataDataContext Instance = new CmsDataDataContext();
        #endregion

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

        public List<NewsModel> GetListNewByFilter(int UserId, int CateId, int DistricId, int StatusId, int GovermentId, int SiteId,
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
                (Instance.PROC_GetListNewsInHome(UserId, CateId, DistricId, StatusId, GovermentId, SiteId, backdate, from, to,
                    minPrice, maxPrice, pageIndex, pageSize, IsRepeat, key, NameOrder, descending, ref totalout)
                    .Select(c => new NewsModel
                    {
                        Id = c.Id.Value,
                        Title = c.Title,
                        CategoryId = c.CategoryId,
                        SiteId = c.SiteId.Value,
                        Link = c.Link,
                        Phone = c.Phone,
                        Contents = HttpUtility.HtmlDecode(Helper.Utils.RemoveHtml(c.Contents)),
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
            using (var db = new CmsDataDataContext())
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
                                 Contents = HttpUtility.HtmlDecode(Helper.Utils.RemoveHtml(c.Contents)),
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
                                 PersionalReport = HttpUtility.HtmlDecode(Helper.Utils.RemoveHtml(GetNameReasonReport(c.Id))),
                                 PersonCheck = GetPersonCheckNews(c.Id),
                                 IsReason = CheckReason(UserId, c.Id)
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
        }

        public List<NewsModel> GetListNewStatusByFilter(int UserId, int CateId, int DistricId, int StatusId, int SiteId,
            int BackDate, string From, string To, double MinPrice, double MaxPrice, int pageIndex, int pageSize, int newsStatus, bool IsRepeat, string key, string NameOrder, bool descending, ref int total)
        {
            using (var db = new CmsDataDataContext())
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
                    (db.PROC_GetListNewsByStatus(UserId, CateId, DistricId, StatusId, SiteId, backdate, from, to,
                        minPrice, maxPrice, pageIndex, pageSize, newsStatus, IsRepeat, key, NameOrder, descending, ref totalout)
                        .Select(c => new NewsModel
                        {
                            Id = c.Id.Value,
                            Title = c.Title,
                            CategoryId = c.CategoryId,
                            SiteId = c.SiteId.Value,
                            Link = c.Link,
                            Phone = c.Phone,
                            Contents = HttpUtility.HtmlDecode(Helper.Utils.RemoveHtml(c.Contents)),
                            Price = c.Price,
                            PriceText = c.PriceText,
                            DistrictId = c.DistrictId,
                            SiteName = c.SiteName,
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
        }

        public List<NewsModel> GetListNewDeleteByFilter(int UserId, int CateId, int DistricId, int StatusId, int SiteId,
            int BackDate, string From, string To, double MinPrice, double MaxPrice, int pageIndex, int pageSize, bool IsRepeat, string key, string NameOrder, bool descending, ref int total)
        {
            using (var db = new CmsDataDataContext())
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

                db.CommandTimeout = 30;
                var listItem =
                    (db.PROC_GetListNewsDelete(UserId, CateId, DistricId, StatusId, SiteId, backdate, from, to,
                        minPrice, maxPrice, pageIndex, pageSize, IsRepeat, key, NameOrder, descending, ref totalout)
                        .Select(c => new NewsModel
                        {
                            Id = c.Id.Value,
                            Title = c.Title,
                            CategoryId = c.CategoryId,
                            SiteId = c.SiteId.Value,
                            Link = c.Link,
                            Phone = c.Phone,
                            Contents = HttpUtility.HtmlDecode(Helper.Utils.RemoveHtml(c.Contents)),
                            Price = c.Price,
                            PriceText = c.PriceText,
                            DistrictId = c.DistrictId,
                            SiteName = c.SiteName,
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
        }
        #endregion

        #region News Action
        /// <summary>
        /// Lưu tin theo user
        /// </summary>
        /// <param name="cusNews"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int SaveNewByUserId(List<News_Customer_Mapping> cusNews, int userId)
        {
            using (var db = new CmsDataDataContext())
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
                    db.Dispose();
                    return 0;
                }
            }
        }

        /// <summary>
        /// Ẩn tin theo user
        /// </summary>
        /// <param name="cusNews"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int HideNewByUserId(List<News_Customer_Mapping> cusNews, int userId)
        {
            using (var db = new CmsDataDataContext())
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
                    db.Dispose();
                    return 0;
                }
            }
        }

        /// <summary>
        /// Xóa tin theo user
        /// </summary>
        /// <param name="listNewDelete"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int Delete(int[] listNewDelete, int userId)
        {
            using (var db = new CmsDataDataContext())
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
                    db.Dispose();
                    return 0;
                }
            }
        }

        /// <summary>
        /// Báo chính chủ
        /// </summary>
        /// <param name="listnews"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int NewsforUser(int[] listnews, int userId)
        {
            using (var db = new CmsDataDataContext())
            {
                try
                {

                    for (int i = 0; i < listnews.Length; i++)
                    {
                        var checkcc = (from c in db.News_customer_actions
                                       where c.NewsId.Equals(listnews[i])
                                       select c).ToList();
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
                        foreach (var item in checkcc)
                        {
                            item.Iscc = true;
                        }
                        db.SubmitChanges();
                    }
                    return 1;
                }
                catch
                {
                    db.Dispose();
                    return 0;
                }
            }
        }

        /// <summary>
        /// Đăng tin
        /// </summary>
        /// <param name="newsItem"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int Createnew(New newsItem, int userId)
        {
            using (var db = new CmsDataDataContext())
            {
                try
                {
                    var query =
                        "INSERT INTO News (CategoryId,Title,Contents,Summary,Link,SiteId,DistrictId,ProvinceId,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,StatusId,Phone,IsUpdated,DateOld,IsDeleted,IsPhone,PriceText,IsRepeat,Area,Price,IsSpam,TotalRepeat)" +
                        "VALUES(" + newsItem.CategoryId + ", N'" + newsItem.Title + "', N'" + newsItem.Contents +
                        "', N'" + newsItem.Contents + "', '" + newsItem.Link + "', " + newsItem.SiteId + ", " +
                        newsItem.DistrictId +
                        "," + newsItem.ProvinceId + "," + (newsItem.CreatedBy ?? 0) + ",'" +
                        (newsItem.CreatedOn ?? DateTime.Now) + "'," + (newsItem.ModifiedBy ?? 0) + ",'" +
                        (newsItem.ModifiedOn ?? DateTime.Now) + "' ," + (newsItem.StatusId ?? 0) + ",'" + newsItem.Phone +
                        "',0,'" + (newsItem.CreatedOn ?? DateTime.Now) +
                        "',0," + (newsItem.IsPhone ? 1 : 0) + ",N'" + newsItem.PriceText + "',0," + (newsItem.Area ?? 0) +
                        "," + newsItem.Price + ",0,1); SELECT SCOPE_IDENTITY()";

                    var id = db.ExecuteQuery<decimal>(query).ToList()[0];
                    NewsforUser(new int[] { Convert.ToInt32(id) }, userId);
                    return 1;
                }
                catch
                {
                    db.Dispose();
                    return 0;
                }
            }
        }

        /// <summary>
        /// Sửa tin
        /// </summary>
        /// <param name="newsItem"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int EditNews(New newsItem, int userId)
        {
            using (var db = new CmsDataDataContext())
            {
                try
                {
                    var news = (from c in db.News where c.Id.Equals(newsItem.Id) select c).FirstOrDefault();
                    if (news != null)
                    {
                        news.Title = newsItem.Title;
                        news.DistrictId = newsItem.DistrictId;
                        news.CategoryId = newsItem.CategoryId;
                        news.Price = newsItem.Price;
                        news.Contents = newsItem.Contents;
                        news.Phone = newsItem.Phone;
                        db.SubmitChanges();
                    }
                    return 1;
                }
                catch
                {
                    db.Dispose();
                    return 0;
                }
            }
        }

        /// <summary>
        /// Bỏ tin chính chủ
        /// </summary>
        /// <param name="listnews"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int RemoveNewsforUser(int[] listnews, int userId)
        {
            using (var db = new CmsDataDataContext())
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
                    db.Dispose();
                    return 0;
                }
            }
        }

        /// <summary>
        /// Báo cáo tin - Khách hàng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertReasonReportNews(ReasonReportNew model)
        {
            try
            {
                using (var db = new CmsDataDataContext())
                {
                    db.ReasonReportNews.InsertOnSubmit(model);
                    db.SubmitChanges();
                    return 1;
                }
            }
            catch 
            {
                return 0;
            }
            
        }

        /// <summary>
        /// Hủy báo cáo tin - Khách hàng
        /// </summary>
        /// <param name="newsId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int DeleteReasonReportNews(int newsId, int userId)
        {
            try
            {
                using (var db = new CmsDataDataContext())
                {
                    var check = db.ReasonReportNews.FirstOrDefault(x => x.UserId == userId && x.NewsId == newsId);
                    if (check != null)
                    {
                        db.ReasonReportNews.DeleteOnSubmit(check);
                        db.SubmitChanges();
                    }
                    return 1;
                }
            }
            catch
            {
                return 0;
            }
        }

        public int DeleteReportNews(int newsId)
        {
            try
            {
                using (var db = new CmsDataDataContext())
                {
                    var reportNews = db.NewsReports.Where(x => x.NewsId == newsId).ToList();
                    if (reportNews != null)
                    {
                        db.NewsReports.DeleteAllOnSubmit(reportNews);
                        db.SubmitChanges();
                        return 1;
                    }
                    else return 0;
                }
            }
            catch 
            {
                return 0;
            }
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
        public List<NewsModel> GetSameNewsByNewsId(int Id, string Title, int CateId, int DistricId, string phone, int UserId)
        {
            var listItem = new List<NewsModel>();
            try
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
                         && !c.Id.Equals(Id)
                         group new { c, st } by new { c.Title, c.Link, st.Name } into g
                         //orderby c.StatusId ascending, c.Price descending
                         select new NewsModel
                         {
                             Title = g.Key.Title,
                             Link = g.Key.Link,
                             SiteName = g.Key.Name
                         }).Skip(0).Take(3).ToList();
           
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

            }
            catch (Exception ẽ)
            {

                throw;
            }
            return listItem;
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
                    rs += new UserBussiness().GetNameById(item.CustomerId)
                        + item.DateCreate.ToString("dd/MM/yyyy");
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

        public int PaymentForCreateNews(long amount, int userId)
        {
            using (var db = new CmsDataDataContext())
            {
                try
                {
                    var paymentAccepted = db.PaymentAccepteds.FirstOrDefault(x => x.UserId == userId);
                    if (paymentAccepted == null) return 0;
                    if (amount > paymentAccepted.AmountTotal) return 2;

                    // insert historypayment
                    var paymentHis = new PaymentHistory
                    {
                        PaymentMethodId = 5,
                        CreatedDate = DateTime.Now,
                        Amount = -amount,
                        Notes = "Trừ tiền đăng bài",
                        UserId = userId
                    };

                    db.PaymentHistories.InsertOnSubmit(paymentHis);
                    // minus cash on amount
                    paymentAccepted.AmountTotal = paymentAccepted.AmountTotal - amount;
                    db.SubmitChanges();

                    return 1;
                }
                catch (Exception)
                {
                    return 3;
                }
            }
        }
        public int CheckRepeatNews(string phone, int districId, int userId)
        {
            using (var db = new CmsDataDataContext())
            {
                try
                {
                    var query = (from c in db.News
                                 join d in db.Districts on c.DistrictId equals d.Id
                                 where c.CreatedOn.HasValue && !c.IsDeleted
                                 && !d.IsDeleted && d.Published
                                 && c.DistrictId.Equals(districId)
                                 && c.Phone.Contains(phone)
                                 select c).ToList();

                    var total = query.Count;
                    return total;
                }
                catch
                {
                    return 0;
                }

            }
        }

        #endregion
    }
}