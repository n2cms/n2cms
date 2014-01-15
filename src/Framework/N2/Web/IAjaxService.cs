using System.Web;

namespace N2.Web
{
    /// <summary>
    /// Represents a service that can handle client side ajax requests and return a response
    /// that makes sense to the javascript method on the client.
    /// </summary>
    public interface IAjaxService
    {
        /// <summary>Gets the name of the service used to map client calls to the appropriate service.</summary>
        string Name { get; }

        /// <summary>Gets wether the service requires editor access.</summary>
        bool RequiresEditAccess { get; }

        /// <summary>Gets whether request's HTTP method is valid for this service.</summary>
        bool IsValidHttpMethod(string httpMethod);

        /// <summary>Processes the request and returns a response.</summary>
        /// <param name="context">The request to handle, usually the query string collection.</param>
        /// <returns>A response string that should make sense to the client.</returns>
        void Handle(HttpContextBase context);
    }
}
