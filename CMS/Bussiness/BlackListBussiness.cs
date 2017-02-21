using CMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Bussiness
{
    public class BlackListBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();

        public List<Blacklist> GetBlackListByParam(ref int totalCount, string search, int pageSize, int pageIndex)
        {
            totalCount = db.Blacklists.Where(x => (x.Words.ToLower().Contains(search.ToLower()) || x.Description.ToLower().Contains(search.ToLower()) || string.IsNullOrEmpty(search))).Count();

            var rs = db.Blacklists.Where(x => (x.Words.ToLower().Contains(search.ToLower()) || x.Description.ToLower().Contains(search.ToLower()) || string.IsNullOrEmpty(search)))
                                            .OrderByDescending(x => x.Id).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            return rs;
        }

        public void Insert(Blacklist model)
        {
            var blaclist = db.Blacklists.FirstOrDefault(x => x.Words == model.Words);
            if (blaclist != null)
                blaclist.Description = model.Description;
            else
                db.Blacklists.InsertOnSubmit(model);
            db.SubmitChanges();
        }

        public void Delete(int id)
        {
            var blackList = db.Blacklists.FirstOrDefault(x => x.Id == id);
            db.Blacklists.DeleteOnSubmit(blackList);
            db.SubmitChanges();
        }
    }
}