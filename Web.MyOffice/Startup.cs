using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Web.MyOffice.Startup))]
namespace Web.MyOffice
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
