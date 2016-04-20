using System.Web.Http;
using System.Web.Http.Controllers;

namespace Macrodeek.HappyZLServiceBase
{
    public class MacroDeekAuthorize : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return AuthorizeUtil.IsAuthorized(actionContext);
        }
    }
}
