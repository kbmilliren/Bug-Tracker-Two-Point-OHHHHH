using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BTProject1.Startup))]
namespace BTProject1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
