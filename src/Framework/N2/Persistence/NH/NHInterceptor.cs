using System.Diagnostics;
using N2.Engine;
using N2.Linq;
using N2.Persistence.Proxying;
using NHibernate;
using NHibernate.Type;

namespace N2.Persistence.NH
{
    /// <summary>
    /// This class is used to notify subscribers about loaded items.
    /// </summary>
    public class NHInterceptor : EmptyInterceptor
    {
        private readonly IProxyFactory interceptor;
        public ISession Session { get; set; }
        private readonly IItemNotifier notifier;
        private readonly Engine.Logger<NHInterceptor> logger;

        public NHInterceptor(IProxyFactory interceptor, IItemNotifier notifier)
        {
            this.interceptor = interceptor;
            this.notifier = notifier;
        }

        public override object Instantiate(string clazz, EntityMode entityMode, object id)
        {
            logger.Debug("Instantiate: " + clazz + " " + entityMode + " " + id);
            object instance = interceptor.Create(clazz, id);
            if (instance != null)
            {
                Session.SessionFactory.GetClassMetadata(clazz).SetIdentifier(instance, id, entityMode);
            }
            return instance;
        }

        /// <summary>Sets rewriter and definition manager on a content item object at load time.</summary>
        /// <param name="entity">The potential content item whose definition manager and rewriter will be set.</param>
        /// <param name="id">Ignored.</param>
        /// <param name="state">Ignored.</param>
        /// <param name="propertyNames">Ignored.</param>
        /// <param name="types">Ignored.</param>
        /// <returns>True if the entity was a content item.</returns>
        public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            interceptor.OnLoaded(entity);
            return NotifiyCreated(entity as ContentItem);
        }

        /// <summary>Invokes the notifier's saving event.</summary>
        /// <param name="entity">The potential content item to notify about.</param>
        /// <param name="id">Ignored.</param>
        /// <param name="currentState">Ignored.</param>
        /// <param name="previousState">Ignored.</param>
        /// <param name="propertyNames">Ignored.</param>
        /// <param name="types">Ignored.</param>
        /// <returns>True if the entity was a content item.</returns>
        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        {
            bool wasHandled = HandleSaveOrUpdate(entity as ContentItem, propertyNames, currentState);
            bool wasAltered = interceptor.OnSaving(entity);
            if (wasHandled || wasAltered)
            {
                logger.Debug("OnFlushDirty: " + entity + " " + id);
                return true;
            }
            return false;
        }

        public override string GetEntityName(object entity)
        {
            return interceptor.GetTypeName(entity);
        }

        /// <summary>Invokes the notifier's saving event.</summary>
        /// <param name="entity">The potential content item to notify about.</param>
        /// <param name="id">Ignored.</param>
        /// <param name="state">Ignored.</param>
        /// <param name="propertyNames">Ignored.</param>
        /// <param name="types">Ignored.</param>
        /// <returns>True if the entity was a content item.</returns>
        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            bool wasHandled = HandleSaveOrUpdate(entity as ContentItem, propertyNames, state);
            bool wasAltered = interceptor.OnSaving(entity);
            if (wasHandled || wasAltered)
            {
                logger.Debug("OnSave: " + entity + " " + id);
                return true;
            }
            return false;
        }

        private bool HandleSaveOrUpdate(ContentItem item, string[] propertyNames, object[] state)
        {
            if (item == null)
                return false;
            
            bool wasAltered = notifier.NotifySaving(item);
            
            for (int i = 0; i < propertyNames.Length; i++)
            {
                if(propertyNames[i] == "AncestralTrail")
                {
                    string trail = Utility.GetTrail(item.Parent);
                    if(trail != (string)state[i])
                    {
                        state[i] = trail;
                        wasAltered = true;
                    }
                }
            }
            return wasAltered;
        }

        #region IItemNotifier Members

        /// <summary>Notify subscribers that an item was loaded or created.</summary>
        /// <param name="newlyCreatedItem">The item that was loaded or created.</param>
        /// <returns>True if the item was modified.</returns>
        public bool NotifiyCreated(ContentItem newlyCreatedItem)
        {
            if (newlyCreatedItem == null)
                return false;

            return notifier.NotifiyCreated(newlyCreatedItem); 
        }

        #endregion

        public override void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            var item = entity as ContentItem;
            if (item != null)
                notifier.NotifyDeleting(item);
        }
    }
}
