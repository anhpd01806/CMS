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

        public List<LinkSite> GetLinkSiteByParam(string url, int siteId, int categorySiteId, int districtId, int provinceId, int pageSize, int pageIndex)
        {
            var rs = db.LinkSites.Where(x => (x.Url.ToLower().Contains(url.ToLower()) || string.IsNullOrEmpty(url))
                                            && x.SiteId == siteId && x.CategorySiteId == categorySiteId 
                                            && x.DistrictId == districtId && x.ProvinceId == provinceId)
                                            .Skip(pageIndex*pageSize).Take(pageSize).ToList();
            return rs;
        }

        public string GetNameSiteById(int siteId)
        {
            return db.Sites.FirstOrDefault(x => x.ID == siteId).Name;
        }

        public void Insert(LinkSite model)
        {
            db.LinkSites.InsertOnSubmit(model);
            db.SubmitChanges();
        }
    }
}