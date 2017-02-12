using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Models
{
    public class News_Customer_MappingModel
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int NewsId { get; set; }

        public bool IsSaved { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsReaded { get; set; }

        public bool IsAgency { get; set; }

        public bool IsSpam { get; set; }

        public DateTime CreateDate { get; set; }

        public string Note { get; set; }

    }

}