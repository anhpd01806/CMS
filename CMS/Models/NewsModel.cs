using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Models
{
    public class NewsModel
    {
        public NewsModel()
        {
            ListImage = new List<ImageModel>();
            SameNews = new List<NewsModel>();
        }

        public int Id { get; set; }

        public int? CategoryId { get; set; }

        public string Title { get; set; }

        public string Contents { get; set; }

        public string Summary { get; set; }

        public string Link { get; set; }

        public int SiteId { get; set; }

        public int? DistrictId { get; set; }

        public int? ProvinceId { get; set; }

        public string Phone { get; set; }

        public string PriceText { get; set; }

        public bool? IsUpdated { get; set; }

        public DateTime? DateOld { get; set; }

        public bool IsSpam { get; set; }

        public bool IsDeleted { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public bool IsPhone { get; set; }

        public bool IsRepeat { get; set; }

        public decimal? Price { get; set; }

        public decimal? Area { get; set; }

        public bool? Published { get; set; }

        public DateTime? PublishedOn { get; set; }

        public int? StatusId { get; set; }

        public bool IsOwner { get; set; }

        public string AdminComment { get; set; }

        public int PageView { get; set; }

        public string DistictName { get; set; }

        public string StatusName { get; set; }

        public string CateName { get; set; }

        public List<ImageModel> ListImage { get; set; }

        public List<NewsModel> SameNews { get; set; }

        public bool? CusIsReaded { get; set; }

        public bool? CusIsSaved { get; set; }

        public bool? CusIsDeleted { get; set; }

        public bool? CusIsAgency { get; set; }

        public bool? CusIsSpam { get; set; }

        public int RepeatTotal { get; set; }

        public bool IsAdmin { get; set; }

        public string Cusname { get; set; }

        public int CusId { get; set; }

        public bool IsPayment { get; set; }

        public string SiteName { get; set; }

        public bool Iscc { get; set;  }
    }
}