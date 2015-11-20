### 1.5.0
* Added `DictionaryExtensions.TryGetValue` to allow for strongly-typed value retrieval from a IDictionary<string,object>
* Added `ExceptionExtensions.Flatten` to collapse nested exceptions into a single IEnumerable<Exception>
* Added `HttpRequestMessageExtensions.GetSafeCorrelationId` to uniquely correlate request context such as error information
* Added `ContentNegotiatedExceptionHandler` and `ErrorData` which can be set as global `IExceptionHandler` to handle error return messages. `ContentNegotiatedExceptionHandler` can be extended to customize ErrorCode or ErrorMessage (example: by exception type)

### 1.4.0
* Added support for exposed headers in `ConfigurableCorsPolicyAttribute`
* Fixed a bug with missing reference to `Microsoft.AspNet.WebApi.Cors` NuGet package

### 1.3.2
* Fixed a CORS bug - ensure each origin is trimmed after splitting the string entry from config file.

### 1.3.1
* Mark `request.IsLocal()` extension method as obsolete, in favor of the native Web API `request.GetRequestContext().IsLocal` flag.
* Ensure IP filtering works well with OWIN hosting.

### 1.3.0
* Added IP filtering support - as `HttpRequestMessage` extension methods, as an authorization filter (`IpFilterAttribute`) and as a message handler (`IpFilterHandler`). Configured in the `web.config`/`app.config`.

### 1.2.0
* Added robust CORS configuration helpers

### 1.1.0
* Added support for a nested activator in `PerControllerConfigActivator`
* Added `StringCollectionConstraint` and `EnumConstraint` which can be used as custom route constraints.
* Added `CentralizedPrefixProvider` which allows the devloper to set a global `RoutePrefix`
* Added `SimpleArrayModelBinder<T>` which makes it easy to bind delimited arrays from URI
* Deprecated `StringArrayModelBinder` - in favor of the new `SimpleArrayModelBinder<T>`

### 1.0 - 23 March 2015
* First release.