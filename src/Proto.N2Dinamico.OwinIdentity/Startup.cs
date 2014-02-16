using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Proto.OwinIdentity.Startup))]
namespace Proto.OwinIdentity
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
