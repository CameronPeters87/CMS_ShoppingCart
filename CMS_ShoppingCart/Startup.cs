using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CMS_ShoppingCart.Startup))]
namespace CMS_ShoppingCart
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
