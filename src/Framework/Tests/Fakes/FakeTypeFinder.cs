using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using N2.Engine;

namespace N2.Tests.Fakes
{
    public class FakeTypeFinder : ITypeFinder
    {
        public Assembly[] Assemblies { get; set; }
        public Type[] Types { get; set; }

        public FakeTypeFinder(Assembly assembly, params Type[] types)
        {
            Assemblies = new[] {assembly};
            this.Types = types;
        }
        public FakeTypeFinder(params Type[] types)
        {
            Assemblies = new Assembly[0];
            this.Types = types;
        }
        public FakeTypeFinder(params Assembly[] assemblies)
        {
            this.Assemblies = assemblies;
        }

        public IEnumerable<Type> Find(Type requestedType)
        {
            return Types.Where(t => requestedType.IsAssignableFrom(requestedType)).ToList();
        }

        public IEnumerable<AttributedType<TAttribute>> Find<TAttribute>(Type requestedType, bool inherit = true) where TAttribute : class
        {
            return Find(requestedType)
                .SelectMany(t => t.GetCustomAttributes(typeof(TAttribute), inherit)
                    .OfType<TAttribute>()
                    .Select(a => new AttributedType<TAttribute> { Type = t, Attribute = a }));
        }

        public IEnumerable<Assembly> GetAssemblies()
        {
            return Assemblies.ToList();
        }
    }
}
