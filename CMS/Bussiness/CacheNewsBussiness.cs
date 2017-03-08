using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Cache;
using CMS.Models;
using CMS.Helper;

namespace CMS.Bussiness
{
    public class CacheNewsBussiness
    {
        #region member
        private readonly HomeBussiness _homeBussiness = new HomeBussiness();
        private readonly NewsBussiness _newsBussiness = new NewsBussiness();
        public CacheController cache;
        #endregion

        //home
        public CacheNewsBussiness()
        {
            this.cache = ConfigWeb.EnableCache == 1 ? CacheController.GetInstance() : null;
        }
        public List<DistrictModel> GetListDistric()
        {
            if (ConfigWeb.EnableCache == 1)
            {
                var param = String.Format("GetListDistric");
                if (cache.KeyExistsCache(param))
                {
                    var lst = (List<DistrictModel>)cache.GetCache(param);
                    if (lst == null)
                    {
                        cache.DeleteCache(param);
                        var retval = _homeBussiness.GetListDistric();
                        cache.Set(param, retval, ConfigWeb.TimeExpire);
                        return retval;
                    }
                    return lst;
                }
                else
                {
                    var lst = _homeBussiness.GetListDistric();
                    cache.Set(param, lst, ConfigWeb.TimeExpire);
                    return lst;
                }
            }
            return _homeBussiness.GetListDistric();
        }

        public List<CategoryModel> GetListCategory()
        {
            if (ConfigWeb.EnableCache == 1)
            {
                var param = String.Format("GetListCategory");
                if (cache.KeyExistsCache(param))
                {
                    var lst = (List<CategoryModel>)cache.GetCache(param);
                    if (lst == null)
                    {
                        cache.DeleteCache(param);
                        var retval = _homeBussiness.GetListCategory();
                        cache.Set(param, retval, ConfigWeb.TimeExpire);
                        return retval;
                    }
                    return lst;
                }
                else
                {
                    var lst = _homeBussiness.GetListCategory();
                    cache.Set(param, lst, ConfigWeb.TimeExpire);
                    return lst;
                }
            }
            return _homeBussiness.GetListCategory();
        }

        public List<SiteModel> GetListSite()
        {
            if (ConfigWeb.EnableCache == 1)
            {
                var param = String.Format("GetListSite");
                if (cache.KeyExistsCache(param))
                {
                    var lst = (List<SiteModel>)cache.GetCache(param);
                    if (lst == null)
                    {
                        cache.DeleteCache(param);
                        var retval = _homeBussiness.GetListSite();
                        cache.Set(param, retval, ConfigWeb.TimeExpire);
                        return retval;
                    }
                    return lst;
                }
                else
                {
                    var lst = _homeBussiness.GetListSite();
                    cache.Set(param, lst, ConfigWeb.TimeExpire);
                    return lst;
                }
            }
            return _homeBussiness.GetListSite();
        }

        public List<CategoryModel> GetChilldrenlistCategory(int parentId)
        {
            if (ConfigWeb.EnableCache == 1)
            {
                var param = String.Format("GetChilldrenlistCategory-{0}", parentId);
                if (cache.KeyExistsCache(param))
                {
                    var lst = (List<CategoryModel>)cache.GetCache(param);
                    if (lst == null)
                    {
                        cache.DeleteCache(param);
                        var retval = _homeBussiness.GetChilldrenlistCategory(parentId);
                        cache.Set(param, retval, ConfigWeb.TimeExpire);
                        return retval;
                    }
                    return lst;
                }
                else
                {
                    var lst = _homeBussiness.GetChilldrenlistCategory(parentId);
                    cache.Set(param, lst, ConfigWeb.TimeExpire);
                    return lst;
                }
            }
            return _homeBussiness.GetListCategory();
        }

        public List<NewsStatusModel> GetlistStatusModel()
        {
            if (ConfigWeb.EnableCache == 1)
            {
                var param = String.Format("NewsStatusModel");
                if (cache.KeyExistsCache(param))
                {
                    var lst = (List<NewsStatusModel>)cache.GetCache(param);
                    if (lst == null)
                    {
                        cache.DeleteCache(param);
                        var retval = _homeBussiness.GetlistStatusModel();
                        cache.Set(param, retval, ConfigWeb.TimeExpire);
                        return retval;
                    }
                    return lst;
                }
                else
                {
                    var lst = _homeBussiness.GetlistStatusModel();
                    cache.Set(param, lst, ConfigWeb.TimeExpire);
                    return lst;
                }
            }
            return _homeBussiness.GetlistStatusModel();
        }

        public List<NewsModel> GetListNewByFilter(int UserId, int CateId, int DistricId, int StatusId, int SiteId,
            int BackDate, string From, string To, double MinPrice, double MaxPrice, int pageIndex, int pageSize, bool IsRepeat, string key, ref int total)
        {
            if (ConfigWeb.EnableCache == 1)
            {
                var param = String.Format("GetListNewByFilter{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}", UserId,
                    CateId, DistricId, StatusId, SiteId, BackDate, From, To, MinPrice, MaxPrice, pageIndex, pageSize,
                    IsRepeat, Utils.RemoveSign4VietnameseString(key).Replace(" ", ""));
                if (cache.KeyExistsCache(param))
                {
                    var lst = (List<NewsModel>) cache.GetCache(param);
                    var totalCache = cache.GetCache(param + "total");
                    if (lst == null || totalCache == null)
                    {
                        cache.DeleteCache(param);
                        cache.DeleteCache(param + "total");
                        var retval = _homeBussiness.GetListNewByFilter(UserId, CateId, DistricId, StatusId, SiteId,
                            BackDate, From, To, MinPrice, MaxPrice, pageIndex, pageSize, IsRepeat, key, ref total);
                        // insert new cache
                        cache.Set(param, retval, ConfigWeb.TimeExpire);
                        cache.Set(param + "total", total, ConfigWeb.TimeExpire);
                        return retval;
                    }
                    total = (int) cache.GetCache(param + "total");

                    return lst;
                }
                else
                {
                    var lst = _homeBussiness.GetListNewByFilter(UserId, CateId, DistricId, StatusId, SiteId, BackDate,
                        From, To, MinPrice, MaxPrice, pageIndex, pageSize, IsRepeat, key, ref total);
                    cache.Set(param, lst, ConfigWeb.TimeExpire);
                    cache.Set(param + "total", total, ConfigWeb.TimeExpire);
                    return lst;
                }
            }
            return _homeBussiness.GetListNewByFilter(UserId, CateId, DistricId, StatusId, SiteId, BackDate,
                        From, To, MinPrice, MaxPrice, pageIndex, pageSize, IsRepeat, key, ref total);
        }

        public int GetRoleByUser(int userId)
        {
            if (ConfigWeb.EnableCache == 1)
            {
                var param = String.Format("GetRoleByUser");
                if (cache.KeyExistsCache(param))
                {
                    var lst = Convert.ToString(cache.GetCache(param));
                    if (string.IsNullOrEmpty(lst))
                    {
                        cache.DeleteCache(param);
                        var retval = _homeBussiness.GetRoleByUser(userId);
                        cache.Set(param, retval, ConfigWeb.TimeExpire);
                        return retval;
                    }
                    return int.Parse(lst);
                }
                else
                {
                    var lst = _homeBussiness.GetRoleByUser(userId);
                    cache.Set(param, lst, ConfigWeb.TimeExpire);
                    return lst;
                }
            }
            return _homeBussiness.GetRoleByUser(userId);
        }

        public NewsModel GetNewsDetail(int Id, int UserId)
        {
            if (ConfigWeb.EnableCache == 1)
            {
                var param = String.Format("GetNewsDetail{0}{1}", Id, UserId);
                if (cache.KeyExistsCache(param))
                {
                    var lst = (NewsModel)cache.GetCache(param);
                    if (lst == null)
                    {
                        cache.DeleteCache(param);
                        var retval = _homeBussiness.GetNewsDetail(Id, UserId);
                        cache.Set(param, retval, ConfigWeb.TimeExpire);
                        return retval;
                    }
                    return lst;
                }
                else
                {
                    var lst = _homeBussiness.GetNewsDetail(Id, UserId);
                    cache.Set(param, lst, ConfigWeb.TimeExpire);
                    return lst;
                }
            }
            return _homeBussiness.GetNewsDetail(Id, UserId);
        }

        //news
        public List<NewsModel> GetListNewStatusByFilter(int UserId, int CateId, int DistricId, int StatusId, int SiteId,
            int BackDate, string From, string To, double MinPrice, double MaxPrice, int pageIndex, int pageSize, int newsStatus, bool IsRepeat, string key, ref int total)
        {
            if (ConfigWeb.EnableCache == 1)
            {
                var param = String.Format("GetListNewStatusByFilter{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}", UserId,
                    CateId, DistricId, StatusId, SiteId, BackDate, From, To, MinPrice, MaxPrice, pageIndex, pageSize,newsStatus,
                    IsRepeat, Utils.RemoveSign4VietnameseString(key).Replace(" ", ""));
                if (cache.KeyExistsCache(param))
                {
                    var lst = (List<NewsModel>)cache.GetCache(param);
                    var totalCache = cache.GetCache(param + "total");
                    if (lst == null || totalCache == null)
                    {
                        cache.DeleteCache(param);
                        cache.DeleteCache(param + "total");
                        var retval = _newsBussiness.GetListNewStatusByFilter(UserId, CateId, DistricId, StatusId, SiteId,
                            BackDate, From, To, MinPrice, MaxPrice, pageIndex, pageSize, newsStatus, IsRepeat, key, ref total);
                        // insert new cache
                        cache.Set(param, retval, ConfigWeb.TimeExpire);
                        cache.Set(param + "total", total, ConfigWeb.TimeExpire);
                        return retval;
                    }
                    total = (int)cache.GetCache(param + "total");

                    return lst;
                }
                else
                {
                    var lst = _newsBussiness.GetListNewStatusByFilter(UserId, CateId, DistricId, StatusId, SiteId,
                            BackDate, From, To, MinPrice, MaxPrice, pageIndex, pageSize, newsStatus, IsRepeat, key, ref total);
                    cache.Set(param, lst, ConfigWeb.TimeExpire);
                    cache.Set(param + "total", total, ConfigWeb.TimeExpire);
                    return lst;
                }
            }
            return _newsBussiness.GetListNewStatusByFilter(UserId, CateId, DistricId, StatusId, SiteId,
                            BackDate, From, To, MinPrice, MaxPrice, pageIndex, pageSize, newsStatus, IsRepeat, key, ref total);
        }

        public List<NewsModel> GetListNewDeleteByFilter(int UserId, int CateId, int DistricId, int StatusId, int SiteId,
            int BackDate, string From, string To, double MinPrice, double MaxPrice, int pageIndex, int pageSize, bool IsRepeat, string key, ref int total)
        {
            if (ConfigWeb.EnableCache == 1)
            {
                var param = String.Format("GetListNewDeleteByFilter{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}", UserId,
                    CateId, DistricId, StatusId, SiteId, BackDate, From, To, MinPrice, MaxPrice, pageIndex, pageSize,
                    IsRepeat, Utils.RemoveSign4VietnameseString(key).Replace(" ", ""));
                if (cache.KeyExistsCache(param))
                {
                    var lst = (List<NewsModel>)cache.GetCache(param);
                    var totalCache = cache.GetCache(param + "total");
                    if (lst == null || totalCache == null)
                    {
                        cache.DeleteCache(param);
                        cache.DeleteCache(param + "total");
                        var retval = _newsBussiness.GetListNewDeleteByFilter(UserId, CateId, DistricId, StatusId, SiteId,
                            BackDate, From, To, MinPrice, MaxPrice, pageIndex, pageSize, IsRepeat, key, ref total);
                        // insert new cache
                        cache.Set(param, retval, ConfigWeb.TimeExpire);
                        cache.Set(param + "total", total, ConfigWeb.TimeExpire);
                        return retval;
                    }
                    total = (int)cache.GetCache(param + "total");

                    return lst;
                }
                else
                {
                    var lst = _newsBussiness.GetListNewDeleteByFilter(UserId, CateId, DistricId, StatusId, SiteId,
                            BackDate, From, To, MinPrice, MaxPrice, pageIndex, pageSize, IsRepeat, key, ref total);
                    cache.Set(param, lst, ConfigWeb.TimeExpire);
                    cache.Set(param + "total", total, ConfigWeb.TimeExpire);
                    return lst;
                }
            }
            return _newsBussiness.GetListNewDeleteByFilter(UserId, CateId, DistricId, StatusId, SiteId,
                            BackDate, From, To, MinPrice, MaxPrice, pageIndex, pageSize, IsRepeat, key, ref total);
        }

        public List<NewsModel> GetListBrokersInformationByFilter(int UserId, int CateId, int DistricId, int StatusId, int SiteId,
            int BackDate, string From, string To, double MinPrice, double MaxPrice, int pageIndex, int pageSize, bool IsRepeat, string key, ref int total)
        {
            if (ConfigWeb.EnableCache == 1)
            {
                var param = String.Format("GetListBrokersInformationByFilter{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}", UserId,
                    CateId, DistricId, StatusId, SiteId, BackDate, From, To, MinPrice, MaxPrice, pageIndex, pageSize,
                    IsRepeat, Utils.RemoveSign4VietnameseString(key).Replace(" ", ""));
                if (cache.KeyExistsCache(param))
                {
                    var lst = (List<NewsModel>)cache.GetCache(param);
                    var totalCache = cache.GetCache(param + "total");
                    if (lst == null || totalCache == null)
                    {
                        cache.DeleteCache(param);
                        cache.DeleteCache(param + "total");
                        var retval = _newsBussiness.GetListBrokersInformationByFilter(UserId, CateId, DistricId, StatusId, SiteId,
                            BackDate, From, To, MinPrice, MaxPrice, pageIndex, pageSize, IsRepeat, key, ref total);
                        // insert new cache
                        cache.Set(param, retval, ConfigWeb.TimeExpire);
                        cache.Set(param + "total", total, ConfigWeb.TimeExpire);
                        return retval;
                    }
                    total = (int)cache.GetCache(param + "total");

                    return lst;
                }
                else
                {
                    var lst = _newsBussiness.GetListBrokersInformationByFilter(UserId, CateId, DistricId, StatusId, SiteId,
                            BackDate, From, To, MinPrice, MaxPrice, pageIndex, pageSize, IsRepeat, key, ref total);
                    cache.Set(param, lst, ConfigWeb.TimeExpire);
                    cache.Set(param + "total", total, ConfigWeb.TimeExpire);
                    return lst;
                }
            }
            return _newsBussiness.GetListBrokersInformationByFilter(UserId, CateId, DistricId, StatusId, SiteId,
                            BackDate, From, To, MinPrice, MaxPrice, pageIndex, pageSize, IsRepeat, key, ref total);
        }
    }
}