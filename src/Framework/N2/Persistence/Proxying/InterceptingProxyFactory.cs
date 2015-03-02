using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using N2.Engine;
using N2.Definitions;

namespace N2.Persistence.Proxying
{
    public class AttributedProperty
    {
        public PropertyInfo Property { get; set; }
        public IInterceptableProperty Attribute { get; set; }
    }

    /// <summary>
    /// Creates a proxy that rewires auto-generated properties to detail get/set.
    /// </summary>
    [Service(typeof(IProxyFactory))]
    public class InterceptingProxyFactory : EmptyProxyFactory
    {
        Logger<InterceptingProxyFactory> logger;

        class Tuple
        {
            public Type Type;
            public IInterceptorBuilder Builder;
            public Func<IInterceptableType, bool>[] UnproxiedSaveSetters;
        }

        private readonly Dictionary<string, Tuple> types = new Dictionary<string, Tuple>();
        private readonly ProxyGenerator generator = new ProxyGenerator();
        private readonly Type[] additionalInterfacesToProxy = new Type[] { typeof(IInterceptedType) };

		public InterceptingProxyFactory()
		{
		}

        public override void Initialize(IEnumerable<ItemDefinition> interceptedTypes)
        {
            foreach (var definition in interceptedTypes)
            {
                var type = definition.ItemType;
                var interceptableProperties = GetInterceptableProperties(definition).ToList();
                if (interceptableProperties.Count == 0)
                    continue;

                var interceptor = new DetailPropertyInterceptor(type, interceptableProperties);
                types[type.FullName] = new Tuple 
                { 
                    Type = type, 
                    Builder = interceptor,
                    UnproxiedSaveSetters = GetSaveSetters(interceptableProperties).ToArray()
                };
            }
        }

        private IEnumerable<Func<IInterceptableType, bool>> GetSaveSetters(IEnumerable<AttributedProperty> interceptableProperties)
        {
            foreach (var property in interceptableProperties)
            {
                var pi = property.Property;
                Type propertyType = pi.PropertyType;
                string propertyName = pi.Name;
                MethodInfo getter = pi.GetGetMethod();
                MethodInfo setter = pi.GetSetMethod();

                if (property.Attribute.PersistAs == PropertyPersistenceLocation.DetailCollection)
                {
                    if (!typeof(IEnumerable).IsAssignableFrom(propertyType))
                        throw new InvalidOperationException("The property type of '" + propertyName + "' on '" + pi.DeclaringType + "' does not implement IEnumerable which is required for properties stored in a detail collection");

                    yield return (interceptable) => StoreDetailCollectionValues(propertyName, getter, interceptable);
                }
                else if (property.Attribute.PersistAs == PropertyPersistenceLocation.Child)
                {
                    if (typeof(ContentItem).IsAssignableFrom(propertyType))
                        yield return (interceptable) => StoreChildValue(propertyName, getter, interceptable);
                    else if (propertyType.IsContentItemEnumeration())
                        yield return (interceptable) => StoreChildrenValue(propertyName, getter, interceptable);
                    else
                        logger.WarnFormat("{0} on {1}.{2} is not an allowed type for Child persistence location. Only property types deriving from ContentItem or IEnmerable<ContentItem> are allowed.", propertyType, pi.DeclaringType.Name, propertyName);
                }
                else if (property.Attribute.PersistAs == PropertyPersistenceLocation.ValueAccessor)
                {
                    var accessor = property.Attribute as IValueAccessor;
                    if (accessor == null)
                        throw new InvalidOperationException("The property '" + propertyName + "' has an attribute '" + property.Attribute.GetType() + "' which specifices PropertyPersistenceLocation.ValueAccessor but the attribute doesn't implement IValueAccessor");

                    yield return (interceptable) => StoreValueAccessorValue(pi, getter, setter, accessor, interceptable);
                }
                else
                {
                    yield return (interceptable) => StoreDetailValue(propertyType, propertyName, getter, interceptable);
                }
            }
        }

        private bool StoreValueAccessorValue(PropertyInfo property, MethodInfo getter, MethodInfo setter, IValueAccessor accessor, IInterceptableType interceptable)
        {
            var value = getter.Invoke(interceptable, null);
            var context = new ValueAccessorContext
            {
                Instance = interceptable,
                Property = property,
                BackingPropertyGetter = () => { throw new NotSupportedException("Getting property not supported while setting"); },
                BackingPropertySetter = (v) => setter.Invoke(interceptable, new[] { v })
            };
            return accessor.SetValue(context, property.Name, value);
        }

        private static bool StoreChildrenValue(string propertyName, MethodInfo getter, IInterceptableType interceptable)
        {
            var newChildren = getter.Invoke(interceptable, null) as IEnumerable;
            var existingChildren = interceptable.GetChildren(propertyName);

            interceptable.SetChildren(propertyName, newChildren);

            return (newChildren == null && existingChildren == null)
                && !(newChildren == null && existingChildren != null)
                && !(newChildren != null && existingChildren == null)
                && Enumerable.SequenceEqual(newChildren.OfType<object>(), existingChildren.OfType<object>());
        }

        private static bool StoreChildValue(string propertyName, MethodInfo getter, IInterceptableType interceptable)
        {
            var newChild = getter.Invoke(interceptable, null);
            var existingChild = interceptable.GetChild(propertyName);

            interceptable.SetChild(propertyName, newChild);

            return newChild != existingChild;
        }

        private static bool StoreDetailValue(Type propertyType, string propertyName, MethodInfo getter, IInterceptableType interceptable)
        {
            object propertyValue = getter.Invoke(interceptable, null);
            object detailValue = interceptable.GetValue(propertyName);

            if (propertyValue == null && detailValue == null)
                return false;
            if (propertyValue != null && propertyValue.Equals(detailValue))
                return false;

            interceptable.SetValue(propertyName, propertyValue, propertyType);
            return true;
        }

        private static bool StoreDetailCollectionValues(string propertyName, MethodInfo getter, IInterceptableType interceptable)
        {
            IEnumerable propertyValue = (IEnumerable)getter.Invoke(interceptable, null);
            var collectionValues = interceptable.GetValues(propertyName);

            if (propertyValue == null && collectionValues == null)
                return false;

            interceptable.SetValues(propertyName, collectionValues);
            return true;
        }

        private IEnumerable<AttributedProperty> GetInterceptableProperties(ItemDefinition definition)
        {
            // Also include properties on base classes since properties are matched by reference and
            // and we want to intercept calls to properties on base classes with the same name
            for (Type t = definition.ItemType; t != null; t = t.BaseType)
            {
                foreach (var property in t.GetProperties())
                {
                    IEnumerable<IInterceptableProperty> attributes;
                    if (!definition.IsInterceptable(property, out attributes))
                        continue;

                    yield return new AttributedProperty { Property = property, Attribute = attributes.First() };
                }
            }
        }

        public override object Create(string typeName, object id)
        {
            Tuple tuple;
            if (!types.TryGetValue(typeName, out tuple))
                return null;

            return generator.CreateClassProxy(tuple.Type, additionalInterfacesToProxy, tuple.Builder.Interceptor);
        }

        public override bool OnSaving(object instance)
        {
            return ApplyToDetailsOnUnproxiedInstance(instance);
        }

        private bool ApplyToDetailsOnUnproxiedInstance(object instance)
        {
            if (instance is IInterceptedType)
                return false;

            Tuple tuple;
            if (!types.TryGetValue(instance.GetType().FullName, out tuple))
                return false;

            var interceptable = instance as IInterceptableType;
            bool altered = false;
            foreach (var setter in tuple.UnproxiedSaveSetters)
            {
                altered = setter(interceptable) || altered;
            }
            return altered;
        }
    }
}
