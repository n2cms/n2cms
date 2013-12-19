using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Web
{
    /// <summary>
    /// An exception filter that always wants to send a message.
    /// </summary>
    /// <remarks>
    /// <para>Create a custom exception filter by implementing <c>IExceptionFilter</c> and applying the <c>Service</c> attribute</para>
    /// <example>
    /// <code>
    /// [Service(typeof(IExceptionFilter), Replaces=typeof(DefaultExceptionFilter))]
    /// {
    ///     public bool IsMessageworthy(Exception ex)
    ///     {
    ///         // Custom logic here.
    ///     }
    /// }
    /// public class CustomExceptionFilter : IExceptionFilter
    /// </code>
    /// </example>
    /// </remarks>
    [Service(typeof(IExceptionFilter))]
    public class DefaultExceptionFilter : IExceptionFilter
    {
        public bool IsMessageworthy(Exception ex)
        {
            // I never met an exception I didn't like.
            return true;
        }
    }
}
