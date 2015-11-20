using System;
using System.Collections.Generic;
using Climax.Web.Http.Extensions;
using NUnit.Framework;

namespace Climax.Web.Http.Tests.Extensions
{
    [TestFixture]
    public class ExceptionExtensionsTests
    {
        [Test]
        public void FlattenChainSuccessful()
        {
            Exception levelA = new InvalidOperationException("Innermost");
            Exception levelB = new ArgumentException("Middle Exception", levelA);
            Exception levelC = new DivideByZeroException("Outermost Exception", levelB);

            Exception[] expected = { levelC, levelB, levelA };

            IEnumerable<Exception> actual = levelC.Flatten();

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
