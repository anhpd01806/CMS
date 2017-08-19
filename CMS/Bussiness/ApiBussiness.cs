using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Models;
using CMS.Data;
using Elmah;

namespace CMS.Bussiness
{
    public class ApiBussiness : InitDB
    {
        public int GetIdByAccount(string userName)
        {
            int id = 0;
            using (var db = new CmsDataDataContext())
            {
                id = db.Users.FirstOrDefault(x => x.UserName.Equals(userName)).Id;
            }
            return id;
        }

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

            // get all admin
            var allAdmin = new UserBussiness().GetAdminUser();

            try
            {
                using (var db = new CmsDataDataContext())
                {
                    var genpassword = Common.Common.md5(username.Trim() + "ozo" + password.Trim());
                    var user = db.Users.FirstOrDefault(x => x.UserName.Equals(username) && x.Password.Equals(genpassword) && x.IsDeleted == false && x.IsMember == true);
                    if (user != null)
                    {
                        var accepted = CheckAcceptedUser(user.Id, user.IsFree.ToString());
                        UpdateLastLogin(user.Id);
                        var userItem = new UserItem
                        {
                            Id = user.Id,
                            Username = user.UserName,
                            Phone = user.Phone,
                            FullName = user.FullName,
                            IsPayment = accepted.IsAccepted,
                            IsUser = CheckRole(user.Id, user.IsFree.ToString()),
                            ManagerName = allAdmin.Where(x => x.Id == user.ManagerBy) == null ? "Không có người quản lý" : allAdmin.Where(x => x.Id == user.ManagerBy).Select(x => x.FullName).FirstOrDefault(),
                            Amount = new PaymentBussiness().GetCashPaymentByUserId(user.Id),
                            DateEnd = accepted.DateEnd
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

        private AcceptedModel CheckAcceptedUser(int userId, string isFree)
        {
            AcceptedModel model = new AcceptedModel();
            try
            {
                if (isFree.ToLower().Trim() == "true")
                {
                    model.IsAccepted = true;
                    model.DateEnd = null;
                    return model;
                }
                else
                {
                    var rs = Instance.PaymentAccepteds.FirstOrDefault(x => x.UserId == userId && DateTime.Now <= x.EndDate);
                    model.IsAccepted = rs != null ? true : false;
                    if (rs != null)
                    {
                        model.DateEnd = rs.EndDate;
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(HttpContext.Current).Log(new Error(ex));
                model.IsAccepted = false;
                model.DateEnd = null;
                return model;
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

            using (var db = new CmsDataDataContext())
            {
                try
                {
                    var checkUser = db.Users.FirstOrDefault(x => x.UserName == user.UserName);
                    if (checkUser == null)
                    {
                        db.Users.InsertOnSubmit(user);
                        db.SubmitChanges();
                        return user.Id;
                    }
                    return 0;
                }
                catch
                {
                    db.Dispose();
                    return 0;
                }
            }
        }

        public UserItem GetUserDetail(int Id)
        {
            try
            {
                using (var db = new CmsDataDataContext())
                {
                    var user = db.Users.FirstOrDefault(x => x.Id == Id);
                    if (user != null)
                    {
                        var accepted = CheckAcceptedUser(user.Id, user.IsFree.ToString());
                        var userItem = new UserItem
                        {
                            Id = user.Id,
                            Username = user.UserName,
                            Phone = user.Phone,
                            FullName = user.FullName,
                            IsPayment = accepted.IsAccepted,
                            IsUser = CheckRole(user.Id, user.IsFree.ToString()),
                            DateEnd = null,
                            ManagerName = "Không có người quản lý"
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

        public class AcceptedModel
        {
            public bool IsAccepted { get; set; }
            public DateTime? DateEnd { get; set; }
        }
    }
}