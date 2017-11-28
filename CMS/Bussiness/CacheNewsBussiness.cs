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
        public List<DistrictModel> GetListDistric(int provinId)
        {
            if (ConfigWeb.EnableCache == 1)
            {
                var param = String.Format("GetListDistric-" + provinId);
                if (cache.KeyExistsCache(param))
                {
                    var lst = (List<DistrictModel>)cache.GetCache(param);
                    if (lst == null)
                    {
                        cache.DeleteCache(param);
                        var retval = _homeBussiness.GetListDistric(provinId);
                        cache.Set(param, retval, ConfigWeb.TimeExpire);
                        return retval;
                    }
                    return lst;
                }
                else
                {
                    var lst = _homeBussiness.GetListDistric(provinId);
                    cache.Set(param, lst, ConfigWeb.TimeExpire);
                    return lst;
                }
            }
            return _homeBussiness.GetListDistric(provinId);
        }

        public List<ProvinceModel> GetListProvince()
        {
            if (ConfigWeb.EnableCache == 1)
            {
                var param = String.Format("GetListProvince");
                if (cache.KeyExistsCache(param))
                {
                    var lst = (List<ProvinceModel>)cache.GetCache(param);
                    if (lst == null)
                    {
                        cache.DeleteCache(param);
                        var retval = _homeBussiness.GetListProvince();
                        cache.Set(param, retval, ConfigWeb.TimeExpire);
                        return retval;
                    }
                    return lst;
                }
                else
                {
                    var lst = _homeBussiness.GetListProvince();
                    cache.Set(param, lst, ConfigWeb.TimeExpire);
                    return lst;
                }
            }
            return _homeBussiness.GetListProvince();
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
            //if (ConfigWeb.EnableCache == 1)
            //{
            //    var param = String.Format("GetNewsDetail{0}{1}", Id, UserId);
            //    if (cache.KeyExistsCache(param))
            //    {
            //        var lst = (NewsModel)cache.GetCache(param);
            //        if (lst == null)
            //        {
            //            cache.DeleteCache(param);
            //            var retval = _homeBussiness.GetNewsDetail(Id, UserId);
            //            cache.Set(param, retval, ConfigWeb.TimeExpire);
            //            return retval;
            //        }
            //        return lst;
            //    }
            //    else
            //    {
            //        var lst = _homeBussiness.GetNewsDetail(Id, UserId);
            //        cache.Set(param, lst, ConfigWeb.TimeExpire);
            //        return lst;
            //    }
            //}
            return _homeBussiness.GetNewsDetail(Id, UserId);
        }
    }
}