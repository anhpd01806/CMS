using CMS.Data;
using CMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Bussiness
{
    public class DistrictBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();

        public List<District> GetAllDistrict()
        {
            return db.Districts.Where(x => x.IsDeleted == false && x.Published == true).ToList();
        }

        public List<Province> GetAllProvince()
        {
            return db.Provinces.ToList();
        }

        public void Insert(District district)
        {
            db.Districts.InsertOnSubmit(district);
            db.SubmitChanges();
        }

        public District GetDistrictById(int id)
        {
            return db.Districts.FirstOrDefault(x => x.IsDeleted == false && x.Published == true && x.Id == id);
        }

        public void Ụpdate(DistrictModel model)
        {
            var district = db.Districts.FirstOrDefault(x => x.Id == model.Id && x.IsDeleted == false);
            district.Name = model.Name;
            district.Description = model.Description;
            district.ProvinceId = model.ProvinceId;
            district.Published = model.Active;
            db.SubmitChanges();
        }

        public void Delete(int id)
        {
            var district = db.Districts.FirstOrDefault(x => x.Id == id);
            district.IsDeleted = true;
            db.SubmitChanges();
        }

        public List<District> GetDistrictByProvinceId(int provinceId)
        {
            return db.Districts.Where(x => x.ProvinceId == provinceId && x.IsDeleted == false && x.Published == true).ToList();
        }

        public string GetNameProvinceById(int id)
        {
            return db.Provinces.FirstOrDefault(x => x.Id == id && x.Published == true).Name;
        }
    }
}