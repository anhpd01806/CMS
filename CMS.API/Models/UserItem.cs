using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Models
{
    public class UserItem
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public bool IsPayment { get; set; }
        public bool IsUser { get; set; }
    }
}