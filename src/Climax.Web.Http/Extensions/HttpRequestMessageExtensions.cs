using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Hosting;
using Climax.Web.Http.Configuration;

namespace Climax.Web.Http.Extensions
{
    public static class HttpRequestMessageExtensions
    {
        private const string HttpContext = "MS_HttpContext";
        private const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
        private const string OwinContext = "MS_OwinContext";

        [Obsolete("This does not work with OWIN. Additionally, the funcionality is built into Web API 2 - please use request.GetRequestContext().IsLocal property.")]
        public static bool IsLocal(this HttpRequestMessage request)
        {
            var localFlag = request.Properties["MS_IsLocal"] as Lazy<bool>;
            return localFlag != null && localFlag.Value;
        }

        public static Guid GetSafeCorrelationId(this HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            Guid correlationId;
            if (!request.Properties.TryGetValue<Guid>(HttpPropertyKeys.RequestCorrelationKey, out correlationId))
            {
                if (correlationId == Guid.Empty)
                {
                    correlationId = Guid.NewGuid();
                }

                request.Properties.Add(HttpPropertyKeys.RequestCorrelationKey, correlationId);
            }

            return correlationId;
        }

        public static string GetClientIpAddress(this HttpRequestMessage request)
        {
            //Web-hosting
            if (request.Properties.ContainsKey(HttpContext))
            {
                dynamic ctx = request.Properties[HttpContext];
                if (ctx != null)
                {
                    return ctx.Request.UserHostAddress;
                }
            }

            //Self-hosting
            if (request.Properties.ContainsKey(RemoteEndpointMessage))
            {
                dynamic remoteEndpoint = request.Properties[RemoteEndpointMessage];
                if (remoteEndpoint != null)
                {
                    return remoteEndpoint.Address;
                }
            }

            //Owin-hosting
            if (request.Properties.ContainsKey(OwinContext))
            {
                dynamic ctx = request.Properties[OwinContext];
                if (ctx != null)
                {
                    return ctx.Request.RemoteIpAddress;
                }
            }
            return null;
        }

        public static T Get<T>(this HttpRequestMessage request, string key)
        {
            object value;
            request.Properties.TryGetValue(key, out value);

            return value != null ? (T)value : default(T);
        }

        public static string GetHeaderValue(this HttpRequestMessage requestMessage, string key)
        {
            IEnumerable<string> values;
            if (requestMessage.Headers.TryGetValues(key, out values) && values != null)
            {
                return values.FirstOrDefault();
            }

            return null;
        }

        public static T GetPropertyValue<T>(this HttpRequestMessage requestMessage, string key)
        {
            if (requestMessage.Properties.ContainsKey(key))
            {
                var obj = requestMessage.Properties[key];
                if (obj != null)
                {
                    try
                    {
                        return (T)obj;
                    }
                    catch
                    {
                    }
                }
            }

            return default(T);
        }

        public static string GetHeader(this HttpRequestMessage request, string key)
        {
            IEnumerable<string> keys = null;
            if (!request.Headers.TryGetValues(key, out keys))
                return null;

            return keys.First();
        }

        public static string[] GetXForwardedFor(this HttpRequestMessage request)
        {
            var xForwardedFor = request.GetHeader("X-Forwarded-For");
            if (xForwardedFor == null) return new string[0];

            var addresses = xForwardedFor.Split(',');

            return addresses.Select(x =>
            {
                if (x.Contains(':'))
                    return x.Split(':')[0];

                return x;
            }).ToArray();
        }

        public static bool IsIpAllowed(this HttpRequestMessage request)
        {
            if (!request.GetRequestContext().IsLocal)
            {
                var ipAddress = request.GetClientIpAddress();

                var ipFiltering = ConfigurationManager.GetSection("ipFiltering") as IpFilteringSection;
                if (ipFiltering != null && ipFiltering.IpAddresses != null && ipFiltering.IpAddresses.Count > 0)
                {
                    if (ipFiltering.IpAddresses.Cast<IpAddressElement>().Any(ip => (ipAddress == ip.Address && !ip.Denied)))
                    {
                        return true;
                    }

                    return false;
                }
            }

            return true;
        }
    }
}
