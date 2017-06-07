using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Boolean IsMember { get; set; }
        public string ManagerBy { get; set; }
        public string RoleName { get; set; }
        public Boolean IsDelete { get; set; }
        public Boolean IsOnline { get; set; }
        public DateTime EndTimePayment { get; set; }
        public int ManagerId { get; set; }
    }

}