using Microsoft.Owin.Security.OAuth;
using Owin;

namespace Macrodeek.StarDustProductService
{
    public partial class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        static Startup()
        {
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
        }
        //public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // 使用不记名身份验证
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);
        }
    }
}
