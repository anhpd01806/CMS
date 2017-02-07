using CMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Bussiness
{
    public class OperationBussiness
    {
        CmsDataDataContext db = new CmsDataDataContext();
        public List<Operation> GetMenuByUserId(int userId)
        {
            return db.Operations.Where(m => db.Role_Accesses.Where(r => db.Role_Users.Any(ru => ru.RoleId == r.RoleId && ru.UserId == userId)).Any(ra => ra.OperationId == m.Id)).ToList();
        }

        public List<Operation> GetAll()
        {
            return db.Operations.ToList();
        }

        public void Insert(Operation operation)
        {
            db.Operations.InsertOnSubmit(operation);
            db.SubmitChanges();
        }

        public Operation GetById(int id)
        {
            return db.Operations.FirstOrDefault(o => o.Id == id);
        }

        public void Update(Operation operation)
        {
            var operationUpdate = db.Operations.FirstOrDefault(u => u.Id == operation.Id);
            if (operationUpdate != null)
            {
                operationUpdate.Name = operation.Name;
                operationUpdate.Controller = operation.Controller;
                operationUpdate.Action = operation.Action;
                operationUpdate.MenuId = operation.MenuId;
                operationUpdate.Orders = operationUpdate.Orders;
            }
            db.SubmitChanges();
        }

        public List<Operation> GetOperations()
        {
            return db.Operations.OrderBy(op => op.Orders).ToList();
        }

        public Operation Delete(int? id)
        {
            var itemToRemove = db.Operations.SingleOrDefault(x => x.Id == id);

            if (itemToRemove != null)
            {
                //db.Operations.Attach(itemToRemove);
                db.Operations.DeleteOnSubmit(itemToRemove);
                db.SubmitChanges();
            }
            return itemToRemove;
        }
    }
}