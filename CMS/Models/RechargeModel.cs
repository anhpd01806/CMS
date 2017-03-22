using System;

namespace CMS.Models
{
    public class RechargeModel
    {
        //request card charging
        public string RQST { get; set; }
        //user name
        public string USR { get; set; }
        //password
        public string PWD { get; set; }
        
        public string TELCO { get; set; }
        // card seri
        public string SERIAL { get; set; }
        //card code
        public string CODE { get; set; }

        public string message { get; set; }

        public bool isError { get; set; }
    }

}