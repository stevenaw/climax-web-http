using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Climax.Web.Http.Configuration;
using Climax.Web.Http.Extensions;

namespace Climax.Web.Http.Handlers
{
    public class IpFilterHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.IsIpAllowed())
            {
                return await base.SendAsync(request, cancellationToken);
            }

            return request.CreateErrorResponse(HttpStatusCode.Forbidden, "Cannot view this resource");
        }
    }
}