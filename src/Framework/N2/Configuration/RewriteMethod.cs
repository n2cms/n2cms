namespace N2.Configuration
{
    /// <summary>
    /// Represents a way to map a friendly url to a template.
    /// </summary>
    public enum RewriteMethod
    {
        /// <summary>Do not rewrite the request. Someone else should pick it up and do something about it.</summary>
        None = 0,

        /// <summary>The pre 2.1 behavior. Rewrite at begin request.</summary>
        /// <remarks>When using this approach the return-url when using authorization and login-page will be the rewritten url.</remarks>
        BeginRequest = 1,

        /// <summary>Rewrites to page handler at PostResolveRequestCache and back again at PostMapRequestHandler.</summary>
        /// <remarks>While this approach allows for friendly return-url in authorization scenarios the drawback is that the aspx handler cannot be authenticated. So beware of authenticating the template.</remarks>
        SurroundMapRequestHandler = 2
    }
}
