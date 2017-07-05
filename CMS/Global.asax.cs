using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using static CMS.Common.Common;

namespace CMS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        void Session_End(object sender, EventArgs e)
        {
            //Application.Remove("usr_" + GetSession()["SS-USERID"]);
            var currentApp = (List<LoginInfomation>)System.Web.HttpContext.Current.Application["LoginInfomation"];
            var tokenLogin = currentApp.FirstOrDefault(x => x.UserId == int.Parse(GetSession()["SS-USERID"].ToString()));
            if (tokenLogin != null) tokenLogin.PrivateKey = "";
        }

        public HttpSessionState GetSession()
        {
            //Check if current context exists
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Session;
            }
            else
            {
                return this.Session;
            }
        }
    }
}
