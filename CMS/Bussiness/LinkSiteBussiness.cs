using CMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Bussiness
{
    public class LinkSiteBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();

        public List<Site> GetAllSite()
        {
            return db.Sites.Where(x => x.Deleted == false && x.Published == true).ToList();
        }

        public List<LinkSite> GetLinkSiteByParam(ref int totalCount, string url, int siteId, int categorySiteId, int districtId, int provinceId, int pageSize, int pageIndex)
        {
            totalCount = db.LinkSites.Where(x => (x.Url.ToLower().Contains(url.ToLower()) || string.IsNullOrEmpty(url))
                                            && x.SiteId == siteId && x.CategorySiteId == categorySiteId
                                            && x.DistrictId == districtId && x.ProvinceId == provinceId && x.Published == true && x.Deleted == false).Count();

            var rs = db.LinkSites.Where(x => (x.Url.ToLower().Contains(url.ToLower()) || string.IsNullOrEmpty(url))
                                            && x.SiteId == siteId && x.CategorySiteId == categorySiteId 
                                            && x.DistrictId == districtId && x.ProvinceId == provinceId && x.Published == true && x.Deleted == false)
                                            .Skip(pageIndex*pageSize).Take(pageSize).ToList();
            return rs;
        }

        public string GetNameSiteById(int siteId)
        {
            var site = db.Sites.FirstOrDefault(x => x.ID == siteId && x.Published == true && x.Deleted == false);
            return site!= null? site.Name:"";
        }

        public void Insert(LinkSite model)
        {
            db.LinkSites.InsertOnSubmit(model);
            db.SubmitChanges();
        }

        public void Delete(int id)
        {
            var linkSite = db.LinkSites.FirstOrDefault(x => x.Id == id);
            linkSite.Deleted = true;
            db.SubmitChanges();
        }
    }
}