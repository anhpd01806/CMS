using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMS.ViewModel
{
    public class OperationViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Mô tả không được để trống.")]
        public string Name { get; set; }
        public int MenuId { get; set; }

        [Required(ErrorMessage = "Controller không được để trống.")]
        public string Controller { get; set; }

        [Required(ErrorMessage = "Action không được để trống.")]
        public string Action { get; set; }
        public int Order { get; set; }
        public bool IsSelected { get; set; }
    }
}