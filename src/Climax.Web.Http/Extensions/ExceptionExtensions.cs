using System;
using System.Collections.Generic;

namespace Climax.Web.Http.Extensions
{
    public static class ExceptionExtensions
    {
        public static IEnumerable<Exception> Flatten(this Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            var innerException = exception;
            do
            {
                yield return innerException;
                innerException = innerException.InnerException;
            }
            while (innerException != null);
        }
    }
}
