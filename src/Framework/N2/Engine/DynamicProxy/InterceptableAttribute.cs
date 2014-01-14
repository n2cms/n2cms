using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.DynamicProxy
{
    /// <summary>
    /// Attribute used by the proxying system to enable proxying a member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event)]
    public class NonInterceptableAttribute : Attribute
    {
    }
}
