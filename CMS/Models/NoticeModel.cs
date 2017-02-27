using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Models
{
    public class NoticeModel
    {
        public int Id { get; set; }

        public int Userid { get; set; }

        public string Newsid { get; set; }

        public DateTime DateSend { get; set; }

        public string UserName { get; set; }

        public string Title { get; set; }

        public int Type { get; set; }

        public bool Accepted { get; set; }

        public bool ViewFlag { get; set; }

        public int SendTo { get; set; }

        public int SendFlag { get; set; }

        public string AvataPath { get; set; }

        public string Time { get; set; }

        public string Desription { get; set; }

        // user information
        public string Account { get; set; }

        public string FullName { get; set; }

        public Boolean Gender { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

    }

}