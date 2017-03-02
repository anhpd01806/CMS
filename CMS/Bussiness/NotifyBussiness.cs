using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using CMS.Data;
using CMS.Helper;
using CMS.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace CMS.Bussiness
{

    public class NotifyBussiness
    {
        #region define
        CmsDataDataContext db = new CmsDataDataContext();
        #endregion

        #region News in home

        public NoticeModel GetNotifyById(int Id)
        {
            var notify = (from c in db.Notifies
                          join d in db.Users on c.Userid equals d.Id
                          where c.Id.Equals(Id)
                          select new NoticeModel
                          {
                              Id = c.Id,
                              Account = d.UserName,
                              FullName = d.FullName,
                              Phone = d.Phone,
                              Email = d.Email,
                              Gender = d.Sex ?? false,
                              Type = c.Type ?? 0,
                              Userid = c.Userid ?? 0,
                              Desription = c.Description,
                              Title = c.Title,
                              DateSend = c.DateSend ?? DateTime.Now
                          }).FirstOrDefault();
            return notify;
        }


        public List<NoticeModel> GetNotify(bool IsUser, int UserId)
        {
            if (IsUser)
            {
                return (from c in db.Notifies
                        where c.SendFlag == false && c.Accepted == false && c.SendTo.Equals(UserId) && c.ViewFlag == false 
                        orderby c.ViewFlag ascending, c.DateSend descending 
                        select new NoticeModel
                        {
                            Id = c.Id,
                            DateSend = c.DateSend ?? DateTime.Now,
                            UserName = c.UserName,
                            Title = c.Title,
                            ViewFlag = c.ViewFlag ?? false,
                            Type = c.Type ?? 0,
                            Userid = c.Userid ?? 0

                        }).ToList();
            }
            else
            {
                //if user is admin
                return (from c in db.Notifies
                        where c.SendFlag == true && c.Accepted == false && c.ViewFlag == false
                        orderby c.ViewFlag ascending, c.DateSend descending
                        select new NoticeModel
                        {
                            Id = c.Id,
                            DateSend = c.DateSend ?? DateTime.Now,
                            UserName = c.UserName,
                            Title = c.Title,
                            ViewFlag = c.ViewFlag ?? false,
                            Type = c.Type ?? 0,
                            Userid = c.Userid ?? 0
                        }).ToList();
            }

        }

        //get all notice
        public List<Notify> GetAllNotice(bool IsUser, int UserId, int page)
        {
            if (IsUser)
            {
                return (from c in db.Notifies
                        where c.SendFlag == false && c.SendTo.Equals(UserId)
                        orderby c.ViewFlag ascending, c.DateSend descending
                        select c).Skip(page*20).Take(20).ToList();
            }
            else
            {
                //if user is admin
                return (from c in db.Notifies
                        where c.SendFlag == true
                        orderby c.ViewFlag ascending, c.DateSend descending
                       select c).Skip(page * 20).Take(20).ToList();
            }

        }

        //update status notify , set view flag = true
        public int UpdateNotifyView(int Id)
        {
            try
            {
                var notify = (from c in db.Notifies
                              where c.Id.Equals(Id)
                              select c).FirstOrDefault();
                //set view flag = true
                notify.ViewFlag = true;
                db.SubmitChanges();
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        //update status notify , set accepted = true
        public int UpdateNotifyStatus(int Id)
        {
            try
            {
                var notify = (from c in db.Notifies
                              where c.Id.Equals(Id)
                              select c).FirstOrDefault();
                //set view flag = true
                notify.Accepted = true;
                db.SubmitChanges();
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        //update user to user can be login
        public int UpdateUser(int Id)
        {
            try
            {
                var user = (from c in db.Users
                            where c.Id.Equals(Id)
                            select c).FirstOrDefault();
                //set view flag = true
                user.IsMember = true;
                db.SubmitChanges();
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public int Insert(Notify notify)
        {
            db.Notifies.InsertOnSubmit(notify);
            db.SubmitChanges();
            return notify.Id;
        }


        public List<Notify> ReportNews(List<New> listNewsReport, int userReport, string userName)
        {
            try
            {
                List<Notify> lstNotify = new List<Notify>();
                foreach (var item in listNewsReport)
                {
                    //var notify = new Notify();
                    var notify = new Notify
                    {
                        UserName = userName,
                        Userid = userReport,
                        SendFlag = true,
                        DateSend = DateTime.Now,
                        Title = "Báo tin mô giới",
                        Accepted = false,
                        ViewFlag = false,
                        Description = item.Title,
                        Type = 2
                    };
                    db.Notifies.InsertOnSubmit(notify);
                    lstNotify.Add(notify);
                }
                db.SubmitChanges();
                return lstNotify;
            }
            catch
            {
                return null;
            }
        }

        #endregion

    }
}