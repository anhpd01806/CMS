using CMS.API.Data;
using CMS.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.API.Bussiness
{
    public class UserBussiness : InitDB
    {
        public string GetNameById(int id)
        {
            return Instance.Users.FirstOrDefault(x => x.Id == id).UserName;
        }

        public List<User> GetAdminUser()
        {
            return Instance.Users.Where(x => x.IsFree == true).OrderBy(m => m.Id).ToList();
        }

        public List<UserModel> GetCustomerUser(ref int pageTotal, int managerId, int statusId, int pageIndex, int pageSize, string search)
        {
            try
            {
                var rs = (from a in Instance.Users
                          join b in Instance.PaymentAccepteds
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
                          });
                pageTotal = (int)Math.Ceiling((double)rs.ToList().Count / (double)pageSize);
                return rs.OrderBy(x => x.IsMember).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(); ;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }
        public User GetUserById(int id)
        {
            return Instance.Users.FirstOrDefault(x => x.Id == id);
        }
    }
}