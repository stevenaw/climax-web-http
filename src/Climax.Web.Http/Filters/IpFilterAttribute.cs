using System.Web.Http;
using System.Web.Http.Controllers;
using Climax.Web.Http.Extensions;

namespace Climax.Web.Http.Filters
{
    public class IpFilterAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return actionContext.Request.IsIpAllowed();
        }
    }
}