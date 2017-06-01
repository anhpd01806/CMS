using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.API.Data;
using Elmah;
using CMS.API.Models;
using CMS.API.Helper;

namespace CMS.API.Bussiness
{
    public class AccountBussiness : InitDB
    {
        public void UpdateLastLogin(int id)
        {
            using (var db = new CmsDataDataContext())
            {
                try
                {
                    var user = db.Users.FirstOrDefault(x => x.Id == id);
                    user.LastLoginDate = DateTime.Now;
                    user.LastActivityDate = DateTime.Now;
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    ErrorLog.GetDefault(HttpContext.Current).Log(new Error(ex));
                    db.Dispose();
                }
            }
        }

        public UserItem Login(string username, string password)
        {
            try
            {
                using (var db = new CmsDataDataContext())
                {
                    var genpassword = Common.Common.md5(username.Trim() + "ozo" + password.Trim());
                    var user = db.Users.FirstOrDefault(x => x.UserName.Equals(username) && x.Password.Equals(genpassword) && x.IsDeleted == false && x.IsMember == true);
                    if (user != null)
                    {
                        UpdateLastLogin(user.Id);
                        var userItem = new UserItem
                        {
                            Id = user.Id,
                            Username = user.UserName,
                            Phone = user.Phone,
                            FullName = user.FullName,
                            IsPayment = CheckAcceptedUser(user.Id, user.IsFree.ToString()),
                            IsUser = CheckRole(user.Id, user.IsFree.ToString())
                        };
                        return userItem;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(HttpContext.Current).Log(new Error(ex));
                return null;
            }
        }
       
        private bool CheckAcceptedUser(int userId, string isFree)
        {
            try
            {
                if (isFree.ToLower().Trim() == "true")
                {
                    return true;
                }
                else
                {
                    return Instance.PaymentAccepteds.Any(x => x.UserId == userId && DateTime.Now <= x.EndDate);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(HttpContext.Current).Log(new Error(ex));
                return false;
            }
        }

        private bool CheckRole(int userId, string isFree)
        {
            try
            {
                if (isFree.ToLower().Trim() == "true")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(HttpContext.Current).Log(new Error(ex));
                return false;
            }
        }

        public int Insert(User user)
        {
            var check = Instance.Users.FirstOrDefault(x => x.UserName.Equals(user.UserName));
            if (check == null)
            {
                Instance.Users.InsertOnSubmit(user);
                Instance.SubmitChanges();
                return user.Id;
            }
            else
            {
                return 0;
            }
           
        }

    }
}