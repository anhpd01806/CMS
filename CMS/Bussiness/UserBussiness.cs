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

        public List<User> GetCustomerUser(int managerId)
        {
            return db.Users.Where(x => x.IsFree == false && (x.ManagerBy == managerId || managerId == -1)).OrderBy(m => m.Id).ToList();
        }

        public User GetUserById(int id)
        {
            return db.Users.FirstOrDefault(x => x.Id == id);
        }

        public int GetUserByName(string name)
        {
            return db.Users.FirstOrDefault(x => x.UserName == name).Id;
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

        public void UpdateCustomer(UserModel model)
        {
            var user = db.Users.FirstOrDefault(x => x.Id == model.Id);
            user.IsMember = true;
            user.ManagerBy = int.Parse(model.ManagerBy);
            db.SubmitChanges();
        }

        public void Update(UserModel model)
        {
            var user = db.Users.FirstOrDefault(x => x.Id == model.Id);
            user.IsMember = model.IsMember;
            if (model.IsRestore == true) user.IsDeleted = false;//th khôi phục lại tài khoản
            user.ManagerBy = int.Parse(model.ManagerBy);
            db.SubmitChanges();
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