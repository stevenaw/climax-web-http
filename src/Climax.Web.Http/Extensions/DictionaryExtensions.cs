﻿using System;
using System.Collections.Generic;

namespace Climax.Web.Http.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool TryGetValue<T>(this IDictionary<string, object> collection, string key, out T value)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            object valueObj;
            if (collection.TryGetValue(key, out valueObj))
            {
                if (valueObj is T)
                {
                    value = (T)valueObj;
                    return true;
                }
            }

            value = default(T);
            return false;
        }
    }
}
