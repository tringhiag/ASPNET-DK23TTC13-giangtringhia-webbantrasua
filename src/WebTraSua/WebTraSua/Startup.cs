using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebTraSua.Startup))]
namespace WebTraSua
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
