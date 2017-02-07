using CMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.ViewModel
{
    public class UserViewModel
    {
        public List<UserModel> UserList { get; set; }
    }

    public class UserModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Boolean IsMember { get; set; }
        public string ManagerBy { get; set; }
    }
}