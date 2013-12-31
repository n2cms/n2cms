using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy;

namespace N2.Persistence.Proxying
{
    internal interface IInterceptorBuilder
    {
        IEnumerable<MethodInfo> GetInterceptedMethods();
        IInterceptor Interceptor { get; }
    }
}
