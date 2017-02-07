using CMS.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.ViewModel
{
    public class MenuViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CssClass { get; set; }
        public int Orders { get; set; }
        public bool IsActive { get; set; }
        public string Url { get; set; }
        public int? ParentId { get; set; }
        public string ClassSubMenu { get; set; }
        public List<OperationViewModel> Operation { get; set; }
    }

    public class MenuOperationViewModel
    {
        public List<Menus> MenuList { get; set; }
        public List<Operation> OperationList { get; set; }

        public List<SelectListItem> SelectListItem { get; set; }

        public OperationViewModel Operation { get; set; }
    }

    public class PermissionViewModel
    {
        public IEnumerable<SelectListItem> Roles { get; set; }
        public List<Menus> MenuChilds { get; set; }
        public List<Operation> Operations { get; set; }
        public List<Role_Access> RoleAccess { get; set; }
        public List<MenuViewModel> MenuView { get; set; }
        public string RoleId { get; set; }
    }
}