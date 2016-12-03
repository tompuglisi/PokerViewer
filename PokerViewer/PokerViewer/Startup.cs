using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PokerViewer.Startup))]
namespace PokerViewer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
