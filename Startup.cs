using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GradeDiagramTest.Startup))]
namespace GradeDiagramTest
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
