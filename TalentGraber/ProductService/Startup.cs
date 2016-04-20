using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Macrodeek.StarDustProductService.Startup))]

namespace Macrodeek.StarDustProductService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
