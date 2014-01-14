using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using N2.Definitions.Static;
using N2.Engine;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;

namespace N2.Persistence.NH
{
    /// <summary>
    /// Generates nhibernate mappings for item types.
    /// </summary>
    [Service]
    public class ClassMappingGenerator
    {
        private readonly Engine.Logger<ClassMappingGenerator> logger;
        DefinitionMap map;

        public ClassMappingGenerator(DefinitionMap map)
        {
            this.map = map;
        }

        private IEnumerable<Pair> GetPersistables(Type attributedType)
        {
            return attributedType.GetProperties()
                .Where(pi => pi.DeclaringType == attributedType)
                .SelectMany(pi => pi.GetCustomAttributes(typeof(IPersistableProperty), false)
                    .OfType<IPersistableProperty>()
                    .Where(a => a.PersistAs == PropertyPersistenceLocation.Column)
                    .Select(a => new Pair { Attribute = a, DeclaringProperty = pi }));
        }

        private static string GetName(Type t)
        {
            return t.FullName + ", " + t.Assembly.FullName.Split(',')[0];
        }

        // helper classes

        class Pair
        {
            public IPersistableProperty Attribute { get; set; }
            public PropertyInfo DeclaringProperty { get; set; }
        }
        
        public virtual void MapTypes(List<Type> allTypes, NHibernate.Cfg.Configuration cfg, Func<string, string> formatter)
        {
            var m = new HbmMapping();
            m.Items = allTypes.Select(t =>
                {
                    var sc = new HbmSubclass();
                    sc.name = GetName(t);
                    sc.extends = GetName(t.BaseType);
                    sc.discriminatorvalue = map.GetOrCreateDefinition(t).Discriminator ?? t.Name;
                    sc.lazy = false;
                    sc.lazySpecified = true;

                    var propertyMappings = GetPersistables(t)
                        .Select(p => p.Attribute.GetPropertyMapping(p.DeclaringProperty, formatter))
                        .ToList();
                    if (propertyMappings.Count > 0)
                    {
                        if (sc.Items == null)
                            sc.Items = propertyMappings.ToArray();
                        else
                            sc.Items = sc.Items.Union(propertyMappings).ToArray();
                    }
                    logger.DebugFormat("Generating subclass {0} with discriminator {1} extending {2} with {3} items ({4} property mappings)", sc.name, sc.discriminatorvalue, sc.extends, sc.Items != null ? sc.Items.Length.ToString() : "(null)", propertyMappings.Count);
                    return sc;
                }).ToArray();
            if (Debugger.IsAttached)
            {
                var dbg = m.AsString();
            }
            cfg.AddDeserializedMapping(m, "N2");
        }
    }

}
