using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CMS.API.Startup))]
namespace CMS.API
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
