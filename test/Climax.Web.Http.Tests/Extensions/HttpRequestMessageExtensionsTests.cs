using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Hosting;
using Climax.Web.Http.Extensions;
using NUnit.Framework;
using Should;

namespace Climax.Web.Http.Tests.Extensions
{
    [TestFixture]
    public class HttpRequestMessageExtensionsTests
    {
        [Test]
        public void GetSafeCorrelationIdHasGuid()
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage();

            Guid correlationId = requestMessage.GetSafeCorrelationId();

            correlationId.ShouldNotEqual(Guid.Empty);
            requestMessage.Properties.ShouldContain(new KeyValuePair<string, object>(HttpPropertyKeys.RequestCorrelationKey, correlationId));
        }
    }
}
