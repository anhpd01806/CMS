using CMS.Data;
using CMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Bussiness
{
    public class ProviceBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();

        public List<Province> GetAllProvice()
        {
            return db.Provinces.Where(x=>x.Published == true).ToList();
        }

        public void Insert(Province model)
        {
            db.Provinces.InsertOnSubmit(model);
            db.SubmitChanges();
        }

        public Province GetProviceById(int id)
        {
            return db.Provinces.FirstOrDefault(x=>x.Id == id && x.Published == true);
        }

        public void Update(ProviceModel model)
        {
            var provice =  db.Provinces.FirstOrDefault(x=>x.Id == model.Id);
            provice.Name = model.Name;
            provice.Description = model.Description;
            provice.Published = model.Active;
            db.SubmitChanges();
        }

        public void Delete(int id)
        {
            var provice = db.Provinces.FirstOrDefault(x => x.Id == id);
            db.Provinces.DeleteOnSubmit(provice);
            db.SubmitChanges();
        }
    }
}