using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Climax.Web.Http.Extensions;
using NUnit.Framework;
using Should;

namespace Climax.Web.Http.Tests
{
    public class IpConfig
    {
        public string Address { get; set; }
    }

    [TestFixture]
    public class HttpRequestMessageExtensionsTests
    {
        [Test]
        public void IsIpAllowed_ReturnsTrue_ForLocalIps()
        {
            var req = new HttpRequestMessage();
            req.Properties.Add("MS_IsLocal", new Lazy<bool>(() => true));

            req.IsIpAllowed().ShouldBeTrue();
        }

        [Test]
        public void IsIpAllowed_ReturnsTrue_ForWhiteListedIps()
        {
            var req = new HttpRequestMessage();
            req.Properties.Add("MS_IsLocal", new Lazy<bool>(() => false));
            req.Properties.Add("System.ServiceModel.Channels.RemoteEndpointMessageProperty", new IpConfig { Address = "192.168.0.196" });

            req.IsIpAllowed().ShouldBeTrue();
        }

        [Test]
        public void IsIpAllowed_ReturnsTrue_ForNonWhiteListedIps()
        {
            var req = new HttpRequestMessage();
            req.Properties.Add("MS_IsLocal", new Lazy<bool>(() => false));

            req.IsIpAllowed().ShouldBeFalse();
        }

        [Test]
        public void IsIpAllowed_ReturnsFalse_ForBlacklistedListedIps()
        {
            var req = new HttpRequestMessage();
            req.Properties.Add("MS_IsLocal", new Lazy<bool>(() => false));
            req.Properties.Add("System.ServiceModel.Channels.RemoteEndpointMessageProperty", new IpConfig { Address = "192.168.0.197" });

            req.IsIpAllowed().ShouldBeFalse();
        } 
    }
}
