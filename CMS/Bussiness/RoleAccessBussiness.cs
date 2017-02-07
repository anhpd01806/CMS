using CMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Bussiness
{
    public class RoleAccessBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();
        public bool IsPermission(string controller, string action, int userId)
        {
            bool isPermission = false;
            if (controller.ToLower() == "home" && action.ToLower() == "partialprofile")
            {
                isPermission = true;
            }
            else
            {
                var operationExist = db.Operations.FirstOrDefault(op => op.Controller.ToLower().Contains(controller.ToLower()) && op.Action.ToLower().Contains(action.ToLower()));
                if (operationExist != null)
                {
                    List<Operation> listMenuAction = GetMenuByUserId(userId);
                    var operation = listMenuAction.FirstOrDefault(ma => ma.Controller.ToLower().Contains(controller.ToLower()) && ma.Action.ToLower().Contains(action.ToLower()));
                    if (operation != null)
                    {
                        isPermission = true;
                    }
                }
                else
                {
                    isPermission = true;
                }
            }
            return isPermission;
        }

        public List<Operation> GetMenuByUserId(int userId)
        {
            return db.Operations.Where(m => db.Role_Accesses.Where(r => db.Role_Users.Any(ru => ru.RoleId == r.RoleId && ru.UserId == userId)).Any(ra => ra.OperationId == m.Id)).ToList();
        }

        public List<Role_Access> GetOperationInRole(int roleId)
        {
            return db.Role_Accesses.Where(m => m.RoleId == roleId).ToList();
        }

        public void Delete(Role_Access roleAccess)
        {
            if (roleAccess != null)
            {
                var rs = db.ExecuteCommand(@"DELETE Role_Access WHERE RoleId = " + roleAccess.RoleId + " and OperationId = " + roleAccess.OperationId);
            }
        }

        public void Insert(Role_Access roleAccess)
        {
            if (roleAccess != null)
            {
                var rs = db.ExecuteCommand(@"INSERT INTO Role_Access VALUES (" + roleAccess.RoleId + "," + roleAccess.OperationId + ")");
            }
        }

        public List<Role_Access> GetAllRoleAccess()
        {
            return db.Role_Accesses.ToList();
        }
    }
}