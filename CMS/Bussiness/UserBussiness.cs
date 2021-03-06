﻿using CMS.Data;
using CMS.Helper;
using CMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.Bussiness
{
    public class UserBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();

        public List<User> GetAllUser()
        {
            return db.Users.OrderBy(m => m.Id).ToList();
        }

        public List<User> GetAdminUser()
        {
            return db.Users.Where(x => x.IsFree == true).OrderBy(m => m.Id).ToList();
        }

        public List<UserModel> GetCustomerUser(ref double total, ref int pageTotal, int managerId, int statusId, int pageIndex, int pageSize, string search)
        {
            try
            {
                var rs = (from a in db.Users
                          join b in db.PaymentAccepteds
                          on a.Id equals b.UserId into ps
                          from b in ps.DefaultIfEmpty()
                          where (a.ManagerBy == managerId || a.ManagerBy == null || managerId == 0)
                          && (statusId == 0 || (statusId == 1 && b.EndDate.AddDays(-2) > DateTime.Now && b.EndDate != null)
                           || (statusId == 2 && b.EndDate.AddDays(-2) <= DateTime.Now && b.EndDate > DateTime.Now && b.EndDate != null)
                           || (statusId == 3 && b.EndDate <= DateTime.Now && b.EndDate != null)
                           || (statusId == 4 && b.EndDate == null))
                           && (a.UserName.Contains(search) || a.FullName.Contains(search))
                           && a.IsFree == false
                          select new UserModel
                          {
                              Id = a.Id,
                              ManagerId = a.ManagerBy ?? 0,
                              FullName = a.FullName,
                              UserName = a.UserName,
                              Phone = a.Phone,
                              Email = a.Email,
                              IsDelete = a.IsDeleted ?? false,
                              IsMember = a.IsMember ?? false,
                              //EndTimePayment = b.EndDate == null ? DateTime.MinValue : b.EndDate,
                          });
                total = (double)rs.ToList().Count;
                pageTotal = (int)Math.Ceiling(total / (double)pageSize);
                return rs.OrderBy(x => x.IsMember).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(); ;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }

        }

        public List<UserModelApi> GetCustomerUserApi(ref double total, ref int pageTotal, int managerId, int statusId, int pageIndex, int pageSize, string search)
        {
            try
            {
                var rs = (from a in db.Users
                          join b in db.PaymentAccepteds
                          on a.Id equals b.UserId into ps
                          from b in ps.DefaultIfEmpty()
                          where (a.ManagerBy == managerId || a.ManagerBy == null || managerId == 0 || managerId == 1)
                          && (statusId == 0 || (statusId == 1 && b.EndDate.AddDays(-2) > DateTime.Now && b.EndDate != null)
                           || (statusId == 2 && b.EndDate.AddDays(-2) <= DateTime.Now && b.EndDate > DateTime.Now && b.EndDate != null)
                           || (statusId == 3 && b.EndDate <= DateTime.Now && b.EndDate != null)
                           || (statusId == 4 && b.EndDate == null))
                           && (a.UserName.Contains(search) || a.FullName.Contains(search) || string.IsNullOrEmpty(search))
                           && a.IsFree == false
                          select new UserModelApi
                          {
                              Id = a.Id,
                              ManagerId = a.ManagerBy ?? 0,
                              FullName = a.FullName,
                              UserName = a.UserName,
                              Phone = a.Phone,
                              Email = a.Email,
                              IsDelete = a.IsDeleted ?? false,
                              IsMember = a.IsMember ?? false,
                              TimeEnd = b.EndDate,
                              //EndTimeStr = b.EndDate != null ? string.Format(b.EndDate.ToString("dd/MM/yyyy")) : "Chưa đăng ký gói cước"
                          });
                total = (double)rs.ToList().Count;
                pageTotal = (int)Math.Ceiling(total / (double)pageSize);
                return rs.OrderBy(x => x.IsMember).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(); ;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }

        }
        public List<UserModel> GetCustomerByListUserId(string ListUserId)
        {
            var arrayId = ListUserId.Split(',');
            //.Where(x => arrayId.Contains(x.Id.ToString())).ToList();
            var rs = (from a in db.Users
                      where arrayId.Contains(a.Id.ToString())
                      select new UserModel
                      {
                          Id = a.Id,
                          ManagerId = a.ManagerBy ?? 0,
                          FullName = a.FullName,
                          UserName = a.UserName,
                          Phone = a.Phone,
                          Email = a.Email,
                          IsDelete = a.IsDeleted ?? false,
                          IsMember = a.IsMember ?? false
                      }).ToList();
            return rs;
        }

        public User GetUserById(int id)
        {
            return db.Users.FirstOrDefault(x => x.Id == id);
        }

        public int GetUserByName(string name)
        {
            var user = db.Users.FirstOrDefault(x => x.UserName == name);
            if (user != null)
                return user.Id;
            else return 0;
        }

        public string GetNameById(int id)
        {
            var user = db.Users.FirstOrDefault(x => x.Id == id);
            if (user != null)
                return user.FullName;
            else return "Quản trị viên ";
        }

        /// <summary>
        /// Get user with role = admin
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetManagerUser()
        {
            var managerUser = (from u in db.Users
                               where u.IsFree == true
                               select new SelectListItem
                               {
                                   Text = u.FullName != null ? u.FullName : u.UserName,
                                   Value = u.Id.ToString()
                               }).ToList();
            return managerUser;
        }

        public int Insert(User user)
        {
            db.Users.InsertOnSubmit(user);
            db.SubmitChanges();
            return user.Id;
        }

        public void Update(int id)
        {
            var user = db.Users.FirstOrDefault(x => x.Id == id);
            user.Password = Helpers.md5(user.UserName.Trim() + "ozo123456");
            db.SubmitChanges();
        }
        public void Delete(int id)
        {
            var user = db.Users.FirstOrDefault(x => x.Id == id);
            user.IsDeleted = true;
            db.SubmitChanges();
        }

        public Boolean ChangePassword(int id, string password, string oldPassword)
        {
            var user = db.Users.FirstOrDefault(x => x.Id == id);
            if (user.Password != Helpers.md5(user.UserName.Trim() + "ozo" + oldPassword.Trim())) return false;
            user.Password = Helpers.md5(user.UserName.Trim() + "ozo" + password.Trim());
            db.SubmitChanges();
            return true;
        }

        public void ResetPassword(int id)
        {
            try
            {
                var user = db.Users.FirstOrDefault(x => x.Id == id);
                user.Password = Helpers.md5(user.UserName.Trim() + "ozo123456");
                db.SubmitChanges();
            }
            catch (Exception)
            {

            }
        }

        public void Update(UserModel model)
        {
            var user = db.Users.FirstOrDefault(x => x.Id == model.Id);
            user.IsMember = model.IsMember;
            user.Notes = model.Notes;
            user.IsDeleted = model.IsRestore;//th khôi phục lại tài khoản
            user.ManagerBy = int.Parse(model.ManagerBy);
            db.SubmitChanges();
        }

        public void UpdateLastLogin(int id)
        {
            using (var db2 = new CmsDataDataContext())
            {
                try
                {
                    var user = db2.Users.FirstOrDefault(x => x.Id == id);
                    user.LastLoginDate = DateTime.Now;
                    user.LastActivityDate = DateTime.Now;
                    db2.SubmitChanges();
                }
                catch
                {
                    db2.Dispose();
                }
            }
        }

        public void UpdateProfile(UserModel model)
        {
            var user = db.Users.FirstOrDefault(x => x.Id == model.Id);
            user.FullName = model.FullName;
            user.Sex = model.Sex;
            user.Phone = model.Phone;
            user.Email = model.Email;
            db.SubmitChanges();
        }
    }
}