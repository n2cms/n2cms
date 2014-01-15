using System;
using System.Collections.Generic;
using N2.Definitions;

namespace N2.Persistence.Proxying
{
    /// <summary>
    /// Doesn't intercept at all.
    /// </summary>
    public class EmptyProxyFactory : IProxyFactory
    {
        #region IInterceptionFactory Members

        public virtual void Initialize(IEnumerable<ItemDefinition> interceptedTypes)
        {
        }

        public virtual object Create(string discriminator, object id)
        {
            return null;
        }

        public virtual bool OnLoaded(object instance)
        {
            return false;
        }

        public virtual bool OnSaving(object instance)
        {
            return false;
        }

        public virtual string GetTypeName(object entity)
        {
            var instance = entity as IInterceptedType;
            if (instance == null)
                return null;
            return instance.GetTypeName();
        }

        #endregion
    }
}
