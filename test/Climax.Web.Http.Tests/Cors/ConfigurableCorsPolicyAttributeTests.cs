using System.Net.Http;
using System.Threading;
using Climax.Web.Http.Cors;
using NUnit.Framework;
using Should;

namespace Climax.Web.Http.Tests.Cors
{
    [TestFixture]
    class ConfigurableCorsPolicyAttributeTests
    {
        [Test]
        public void Ctor_CanSetAllAllowSettings_ToStar()
        {
            var elements = new CorsElementCollection();
            var fooElement = new CorsElement
            {
                Name = "foo",
                Headers = "*",
                Methods = "*",
                Origins = "*"
            };
            elements.Add(fooElement);

            var corsSection = new CorsSection
            {
                CorsPolicies = elements
            };

            var attr = new ConfigurableCorsPolicyAttribute("foo", corsSection);

            var policy = attr.GetCorsPolicyAsync(new HttpRequestMessage(), default(CancellationToken)).Result;
            policy.AllowAnyMethod.ShouldEqual(true);
            policy.AllowAnyHeader.ShouldEqual(true);
            policy.AllowAnyOrigin.ShouldEqual(true);
        }

        [Test]
        public void Ctor_CanSetAllAllowSettings_ToRequiredValues()
        {
            var elements = new CorsElementCollection();
            var fooElement = new CorsElement
            {
                Name = "foo",
                Headers = "X-Api-Rate;Foo",
                Methods = "GET;POST;PUT",
                Origins = "http://foo.com;http://www.abc.com",
                ExposedHeaders = "X-Response-Header;Bar"
            };
            elements.Add(fooElement);

            var corsSection = new CorsSection
            {
                CorsPolicies = elements
            };

            var attr = new ConfigurableCorsPolicyAttribute("foo", corsSection);
            var policy = attr.GetCorsPolicyAsync(new HttpRequestMessage(), default(CancellationToken)).Result;

            policy.AllowAnyMethod.ShouldEqual(false);
            policy.Methods.ShouldContain("GET");
            policy.Methods.ShouldContain("POST");
            policy.Methods.ShouldContain("PUT");

            policy.AllowAnyHeader.ShouldEqual(false);
            policy.Headers.ShouldContain("X-Api-Rate");
            policy.Headers.ShouldContain("Foo");

            policy.AllowAnyOrigin.ShouldEqual(false);
            policy.Origins.ShouldContain("http://foo.com");
            policy.Origins.ShouldContain("http://www.abc.com");

            policy.ExposedHeaders.ShouldContain("X-Response-Header");
            policy.ExposedHeaders.ShouldContain("Bar");
        }

        [Test]
        public void Ctor_TrimsUnnecessaryWhitespaceAroundOrigins()
        {
            var elements = new CorsElementCollection();
            var fooElement = new CorsElement
            {
                Name = "foo",
                Headers = "*",
                Methods = "*",
                Origins = "\r\n     http://foo.com;\r\n     http://www.abc.com"
            };
            elements.Add(fooElement);

            var corsSection = new CorsSection
            {
                CorsPolicies = elements
            };

            var attr = new ConfigurableCorsPolicyAttribute("foo", corsSection);
            var policy = attr.GetCorsPolicyAsync(new HttpRequestMessage(), default(CancellationToken)).Result;
            
            policy.Origins.Count.ShouldEqual(2);
            policy.Origins[0].ShouldEqual("http://foo.com");
            policy.Origins[1].ShouldEqual("http://www.abc.com");
        }

        [Test]
        public void Ctor_TrimsUnnecessaryWhitespaceAroundMethods()
        {
            var elements = new CorsElementCollection();
            var fooElement = new CorsElement
            {
                Name = "foo",
                Headers = "*",
                Methods = "\r\n    GET;\r\n    POST",
                Origins = "*"
            };
            elements.Add(fooElement);

            var corsSection = new CorsSection
            {
                CorsPolicies = elements
            };

            var attr = new ConfigurableCorsPolicyAttribute("foo", corsSection);
            var policy = attr.GetCorsPolicyAsync(new HttpRequestMessage(), default(CancellationToken)).Result;

            policy.Methods.Count.ShouldEqual(2);
            policy.Methods[0].ShouldEqual("GET");
            policy.Methods[1].ShouldEqual("POST");
        }

        [Test]
        public void Ctor_TrimsUnnecessaryWhitespaceAroundHeaders()
        {
            var elements = new CorsElementCollection();
            var fooElement = new CorsElement
            {
                Name = "foo",
                Headers = "\r\n    X-Api-Rate;\r\n    Foo",
                Methods = "*",
                Origins = "*"
            };
            elements.Add(fooElement);

            var corsSection = new CorsSection
            {
                CorsPolicies = elements
            };

            var attr = new ConfigurableCorsPolicyAttribute("foo", corsSection);
            var policy = attr.GetCorsPolicyAsync(new HttpRequestMessage(), default(CancellationToken)).Result;

            policy.Headers.Count.ShouldEqual(2);
            policy.Headers[0].ShouldEqual("X-Api-Rate");
            policy.Headers[1].ShouldEqual("Foo");
        }

        [Test]
        public void Ctor_TrimsUnnecessaryWhitespaceAroundEsposedHeaders()
        {
            var elements = new CorsElementCollection();
            var fooElement = new CorsElement
            {
                Name = "foo",
                Headers = "*",
                Methods = "*",
                Origins = "*",
                ExposedHeaders = "\r\n    X-Api-Rate;\r\n    Foo"
            };
            elements.Add(fooElement);

            var corsSection = new CorsSection
            {
                CorsPolicies = elements
            };

            var attr = new ConfigurableCorsPolicyAttribute("foo", corsSection);
            var policy = attr.GetCorsPolicyAsync(new HttpRequestMessage(), default(CancellationToken)).Result;

            policy.ExposedHeaders.Count.ShouldEqual(2);
            policy.ExposedHeaders[0].ShouldEqual("X-Api-Rate");
            policy.ExposedHeaders[1].ShouldEqual("Foo");
        }
    }
}
