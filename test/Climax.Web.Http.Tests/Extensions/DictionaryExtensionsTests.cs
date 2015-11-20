using System;
using System.Collections.Generic;
using NUnit.Framework;
using Climax.Web.Http.Extensions;
using Should;

namespace Climax.Web.Http.Tests.Extensions
{
    [TestFixture]
    public class DictionaryExtensionsTests
    {
        [Test]
        public void TryGetValuePrimitiveMatchTypeSuccess()
        {
            TryGetValueSuccess(2);
        }

        [Test]
        public void TryGetValueMatchComplexTypeSuccess()
        {
            TryGetValueSuccess(new InvalidOperationException());
        }

        [Test]
        public void TryGetValueMatchInheritedTypeSuccess()
        {
            TryGetValueSuccess<Exception>(new InvalidOperationException());
        }

        private static void TryGetValueSuccess<TValue>(TValue expectedValue)
        {
            TValue actualValue;
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "key", expectedValue }
            };


            bool success = values.TryGetValue("key", out actualValue);

            success.ShouldBeTrue();
            actualValue.ShouldEqual(expectedValue);
        }

        [Test]
        public void TryGetValuePrimitiveMisMatchTypeFail()
        {
            double actualOutput;
            int expectedOutput = 2;
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "key", expectedOutput }
            };


            bool success = values.TryGetValue("key", out actualOutput);

            success.ShouldBeFalse();
        }
    }
}
