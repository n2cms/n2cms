using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DinamicoIdentity.Startup))]
namespace DinamicoIdentity
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
