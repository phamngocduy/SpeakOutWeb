using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SpeakOutWeb.Startup))]
namespace SpeakOutWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
