using CMS.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMS.ViewModel
{
    public class RoleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string Name { get; set; }
        public bool IsChecked { get; set; }
        public List<Role> RoleList { get; set; }
    }
}