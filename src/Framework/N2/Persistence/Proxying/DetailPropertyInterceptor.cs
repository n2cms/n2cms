using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using System.Collections;
using N2.Details;
using N2.Engine;

namespace N2.Persistence.Proxying
{
    /// <summary>
    /// Intercepts detail property calls and calls Get/SetDetail.
    /// </summary>
    public class DetailPropertyInterceptor : IInterceptor, N2.Persistence.Proxying.IInterceptorBuilder
    {
        Logger<DetailPropertyInterceptor> logger;

        public IInterceptor Interceptor
        {
            get { return this; }
        }

        public IEnumerable<MethodInfo> GetInterceptedMethods()
        {
            return methods.Keys;
        }

        private readonly IDictionary<MethodInfo, Action<IInvocation>> methods = new Dictionary<MethodInfo, Action<IInvocation>>();
        private static readonly Action<IInvocation> proceedAction = (invocation) => invocation.Proceed();
        private static MethodInfo getEntityNameMethod = typeof(IInterceptedType).GetMethod("GetTypeName");
        
        public DetailPropertyInterceptor(Type interceptedType, IEnumerable<AttributedProperty> interceptedProperties)
        {
            string typeName = interceptedType.FullName;
            methods[getEntityNameMethod] = invocation => invocation.ReturnValue = typeName;
            
            Action<IInvocation> getContentTypeAction = invocation => invocation.ReturnValue = interceptedType;
            for (Type t = interceptedType; t != null; t = t.BaseType)
            {
                MethodInfo method = t.GetMethod("GetContentType");
                if (method != null)
                    methods[method] = getContentTypeAction;
            }

            foreach (var intercepted in interceptedProperties)
            {
                var property = intercepted.Property;
                string propertyName = property.Name;
                Type propertyType = property.PropertyType;
                var location = intercepted.Attribute.PersistAs;

                var getMethod = property.GetGetMethod();
                if (getMethod == null)
                    continue; // no public getter? let's move on
                if (!getMethod.IsCompilerGenerated())
                    continue; // only intercept auto-implemented properties

                object defaultValue = GetDefaultValue(propertyType, intercepted.Attribute.DefaultValue);
                var setMethod = property.GetSetMethod();

                if (location == PropertyPersistenceLocation.ValueAccessor)
                {
                    var accessor = intercepted.Attribute as IValueAccessor;
                    if (accessor == null)
                        throw new InvalidOperationException("The property '" + propertyName + "' has an attribute '" + intercepted.Attribute.GetType() + "' which specifices PropertyPersistenceLocation.ValueAccessor but the attribute doesn't implement IValueAccessor");
                    methods[getMethod] = GetValueAccessorGet(property, accessor);
                    if (setMethod != null)
                        methods[setMethod] = GetInvokeValueAccessorSet(property, accessor);
                }
                else if (location == PropertyPersistenceLocation.Detail)
                {
                    methods[getMethod] = GetGetDetail(propertyName, defaultValue);

                    if (setMethod == null)
                        continue; // no public setter? that's okay
                    methods[setMethod] = GetSetDetail(propertyName, defaultValue, propertyType);
                }
                else if (location == PropertyPersistenceLocation.DetailCollection)
                {
                    if (!typeof(IEnumerable).IsAssignableFrom(propertyType))
                        throw new InvalidCastException("The property " + propertyName + " has the property type " + propertyType + " but a type assignable from IEnumerable is required for usage of PropertyPersistenceLocation.DetailCollection");
                    if (defaultValue != null && !typeof(IEnumerable).IsAssignableFrom(defaultValue.GetType()))
                        throw new InvalidCastException("The property " + propertyName + " has a default value type " + propertyType + " but a type assignable from IEnumerable is required for usage of PropertyPersistenceLocation.DetailCollection");

                    methods[getMethod] = GetGetDetailCollection(propertyName, propertyType, defaultValue);

                    if (setMethod == null)
                        continue; // no public setter? that's okay
                    methods[setMethod] = GetSetDetailCollection(propertyName, defaultValue as IEnumerable, propertyType);
                }
                else if (location == PropertyPersistenceLocation.Child)
                {
                    if (!typeof(ContentItem).IsAssignableFrom(propertyType))
                    {
                        if (!propertyType.IsContentItemEnumeration())
                        {
                            logger.WarnFormat("{0} on {1}.{2} is not an allowed type for Child persistence location. Only property types deriving from ContentItem or IEnmerable<ContentItem> are allowed.", propertyType, interceptedType.Name, propertyName);
                            continue;
                        }
                    }

                    methods[getMethod] = GetGetChild(propertyName, propertyType);
                    if (setMethod == null)
                        continue; // no public setter? that's okay
                    methods[setMethod] = GetSetChild(propertyName, propertyType);
                }
            }
        }

        private static Action<IInvocation> GetValueAccessorGet(PropertyInfo property, IValueAccessor accessor)
        {
            return (invocation) => invocation.ReturnValue = accessor.GetValue(
                new ValueAccessorContext 
                { 
                    Instance = invocation.InvocationTarget as IInterceptableType, 
                    Property = property, 
                    BackingPropertyGetter = () => { invocation.Proceed(); return invocation.ReturnValue; },
                    BackingPropertySetter = (value) => { throw new NotSupportedException("Setting property not supported while getting"); },
                }, 
                property.Name);
        }

        private static Action<IInvocation> GetInvokeValueAccessorSet(PropertyInfo property, IValueAccessor accessor)
        {
            return (invocation) => accessor.SetValue(
                new ValueAccessorContext 
                { 
                    Instance = invocation.InvocationTarget as IInterceptableType, 
                    Property = property, 
                    BackingPropertyGetter = () => { throw new NotSupportedException("Getting property not supported while setting"); },
                    BackingPropertySetter = (value) => { invocation.Arguments[0] = value; invocation.Proceed(); } 
                }, 
                property.Name, 
                invocation.Arguments[0]);
        }

        private Action<IInvocation> GetGetChild(string propertyName, Type propertyType)
        {
            if (typeof(ContentItem).IsAssignableFrom(propertyType))
                return invocation =>
                {
                    var instance = invocation.Proxy as IInterceptableType;
                    invocation.ReturnValue = instance.GetChild(propertyName);
                };
            else
                return invocation =>
                {
                    var instance = invocation.Proxy as IInterceptableType;
                    var values = instance.GetChildren(propertyName);
                    invocation.ReturnValue = values.ConvertTo(propertyType, propertyName);
                };
        }

        private Action<IInvocation> GetSetChild(string propertyName, Type propertyType)
        {
            if (typeof(ContentItem).IsAssignableFrom(propertyType))
                return invocation =>
                {
                    var value = invocation.Arguments[0];
                    var instance = invocation.Proxy as IInterceptableType;
                    instance.SetChild(propertyName, value);
                };
            else
                return invocation =>
                {
                    var value = invocation.Arguments[0] as IEnumerable;
                    var instance = invocation.Proxy as IInterceptableType;
                    instance.SetChildren(propertyName, value);
                };
        }

        public void Intercept(IInvocation invocation)
        {
            Action<IInvocation> action;
            if (methods.TryGetValue(invocation.Method, out action))
                action(invocation);
            else
                invocation.Proceed();
        }

        private static object GetDefaultValue(Type propertyType, object defaultValue)
        {
            if (defaultValue == null)
            {
                if (propertyType.IsValueType)
                    defaultValue = Activator.CreateInstance(propertyType);
                else if (propertyType == typeof(string))
                    defaultValue = string.Empty;
            }
            return defaultValue;
        }

        private static Action<IInvocation> GetGetDetail(string propertyName, object defaultValue)
        {
            return invocation =>
            {
                var instance = invocation.Proxy as IInterceptableType;
                invocation.ReturnValue = instance.GetValue(propertyName) ?? defaultValue;
            };
        }

        private static Action<IInvocation> GetSetDetail(string propertyName, object defaultValue, Type valueType)
        {
            if (defaultValue != null)
                return invocation =>
                {
                    var instance = invocation.Proxy as IInterceptableType;
                    var value = invocation.Arguments[0];
                    if (defaultValue.Equals(value))
                        value = null;

                    instance.SetValue(propertyName, value, valueType);
                };
            else
                return invocation =>
                {
                    var instance = invocation.Proxy as IInterceptableType;
                    var value = invocation.Arguments[0];

                    instance.SetValue(propertyName, value, valueType);
                };
        }

        private Action<IInvocation> GetGetDetailCollection(string propertyName, Type propertyType, object defaultValue)
        {
            if (propertyType.IsAssignableFrom(typeof(DetailCollection)))
                return invocation =>
                {
                    var instance = invocation.Proxy as IInterceptableType;
                    var collection = instance.GetValues(propertyName);
                    invocation.ReturnValue = collection ?? defaultValue;
                };
            else
                return invocation =>
                {
                    var instance = invocation.Proxy as IInterceptableType;
                    var collection = instance.GetValues(propertyName);
                    if (collection != null)
                        collection = collection.ConvertTo(propertyType, propertyName);
                    invocation.ReturnValue = collection ?? defaultValue;
                };
        }

        private Action<IInvocation> GetSetDetailCollection(string propertyName, IEnumerable defaultValue, Type type)
        {
            if(defaultValue != null)
                return invocation =>
                {
                    var instance = invocation.Proxy as IInterceptableType;
                    var value = invocation.Arguments[0] as IEnumerable;
                    if (value == null)
                        instance.SetValues(propertyName, value);
                    else if(CollectionEquals(defaultValue, value))
                        instance.SetValues(propertyName, null);
                    else
                        instance.SetValues(propertyName, value);
                };
            else
                return invocation =>
                {
                    var instance = invocation.Proxy as IInterceptableType;
                    var value = invocation.Arguments[0];

                    instance.SetValues(propertyName, value as IEnumerable);
                };
        }

        private static bool CollectionEquals(IEnumerable defaultValue, IEnumerable value)
        {
            var dve = defaultValue.GetEnumerator();
            var ve = value.GetEnumerator();
            while (true)
            {
                var dveMoved = dve.MoveNext();
                var veMoved = ve.MoveNext();
                if (dveMoved && veMoved)
                {
                    if (dve.Current == ve.Current)
                        // elements are equal, advance
                        continue;
                    
                    // elements not equal, collections differ
                    return false;
                }
                else if (!dveMoved && !veMoved)
                    // collection ended, collections are equal
                    return true;

                // one collection ended but not the other, collections differ
                return false;
            }
        }
    }
}
