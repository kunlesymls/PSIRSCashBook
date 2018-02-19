using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PSIRSCashBook.Startup))]
namespace PSIRSCashBook
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
