using System;
using System.Security.Principal;
using N2.Engine;
using N2.Persistence;
using N2.Plugin;

namespace N2.Security
{
    /// <summary>
    /// Checks against unauthorized requests, and updates of content items.
    /// </summary>
    [Service(typeof(ISecurityEnforcer))]
    public class SecurityEnforcer : ISecurityEnforcer, IAutoStart
    {
        Logger<SecurityEnforcer> logger;

        /// <summary>
        /// Is invoked when a security violation is encountered. The security 
        /// exception can be cancelled by setting the cancel property on the event 
        /// arguments.
        /// </summary>
        public event EventHandler<CancellableItemEventArgs> AuthorizationFailed;

        private readonly Persistence.IPersister persister;
        private readonly ISecurityManager security;
        private readonly ContentActivator activator;
        private readonly Web.IUrlParser urlParser;
        private readonly Web.IWebContext webContext;
        private int permissionDeniedHttpCode;

        public SecurityEnforcer(Persistence.IPersister persister, ISecurityManager security, ContentActivator activator, Web.IUrlParser urlParser, Web.IWebContext webContext, Configuration.HostSection config)
        {
            this.webContext = webContext;
            this.persister = persister;
            this.security = security;
            this.activator = activator;
            this.urlParser = urlParser;
            this.permissionDeniedHttpCode = config.Web.PermissionDeniedHttpCode;
        }

        #region Event Handlers
        private void ItemSavingEventHandler(object sender, CancellableItemEventArgs e)
        {
            if (security.Enabled && security.ScopeEnabled)
                OnItemSaving(e.AffectedItem);
        }

        private void ItemMovingEvenHandler(object sender, CancellableDestinationEventArgs e)
        {
            if (security.Enabled && security.ScopeEnabled)
                OnItemMoving(e.AffectedItem, e.Destination);
        }

        private void ItemDeletingEvenHandler(object sender, CancellableItemEventArgs e)
        {
            if (security.Enabled && security.ScopeEnabled)
                OnItemDeleting(e.AffectedItem);
        }

        private void ItemCopyingEvenHandler(object sender, CancellableDestinationEventArgs e)
        {
            if (security.Enabled && security.ScopeEnabled)
                OnItemCopying(e.AffectedItem, e.Destination);
        }

        void ItemCreatedEventHandler(object sender, ItemEventArgs e)
        {
            var item = e.AffectedItem;
            var parent = e.AffectedItem.Parent;
            if (parent == null)
                return;

            security.CopyPermissions(parent, item);
        }
        #endregion

        #region Methods

        /// <summary>Checks that the current user is authorized to access the current item.</summary>
        public virtual void AuthorizeRequest(IPrincipal user, ContentItem page, Permission requiredPermission)
        {
            if (page != null)
            {
                if (page != null && !security.IsAuthorized(user, page, requiredPermission))
                {
                    CancellableItemEventArgs args = new CancellableItemEventArgs(page);
                    if (AuthorizationFailed != null)
                        AuthorizationFailed.Invoke(this, args);

                    if (args.Cancel)
                        logger.Info("AuthorizeRequest: Default permission denied handling cancelled by event handler.");
                    else
                    {
                        logger.InfoFormat("AuthorizeRequest: Executing default permission denied action with http code {0}.", permissionDeniedHttpCode);
                        throw new PermissionDeniedException(permissionDeniedHttpCode, permissionDeniedHttpCode == 401 ? "Unauthorized" : "Not Found", page, user);
                    }
                }
            }
        }

        /// <summary>Is invoked when an item is saved.</summary>
        /// <param name="item">The item that is to be saved.</param>
        protected virtual void OnItemSaving(ContentItem item)
        {
            if (!security.IsAuthorized(item, this.webContext.User))
            {
                logger.InfoFormat("OnItemSaving: User {0} not authorized for {1}.", webContext.User, item);
                throw new PermissionDeniedException(item, this.webContext.User);
            }
            IPrincipal user = this.webContext.User;
            if (user != null)
                item.SavedBy = user.Identity.Name;
            else
                item.SavedBy = null;
        }

        /// <summary>Is Invoked when an item is moved.</summary>
        /// <param name="source">The item that is to be moved.</param>
        /// <param name="destination">The destination for the item.</param>
        protected virtual void OnItemMoving(ContentItem source, ContentItem destination)
        {
            if (!security.IsAuthorized(source, this.webContext.User) || !security.IsAuthorized(destination, this.webContext.User))
            {
                logger.InfoFormat("OnItemMoving: User {0} not authorized for {1} to {2}.", webContext.User, source, destination);
                throw new PermissionDeniedException(source, this.webContext.User);
            }
        }

        /// <summary>Is invoked when an item is to be deleted.</summary>
        /// <param name="item">The item to delete.</param>
        protected virtual void OnItemDeleting(ContentItem item)
        {
            IPrincipal user = webContext.User;
            if (!security.IsAuthorized(item, user))
            {
                logger.InfoFormat("OnItemDeleting: User {0} not authorized for {1}.", webContext.User, item);
                throw new PermissionDeniedException(item, user);
            }
        }

        /// <summary>Is invoked when an item is to be copied.</summary>
        /// <param name="source">The item that is to be copied.</param>
        /// <param name="destination">The destination for the copied item.</param>
        protected virtual void OnItemCopying(ContentItem source, ContentItem destination)
        {
            if (!security.IsAuthorized(source, this.webContext.User) || !security.IsAuthorized(destination, this.webContext.User))
            {
                logger.InfoFormat("OnItemCopying: User {0} not authorized for {1} to {2}.", webContext.User, source, destination);
                throw new PermissionDeniedException(source, this.webContext.User);
            }
        }
        #endregion

        #region IStartable Members

        public virtual void Start()
        {
            persister.ItemSaving += ItemSavingEventHandler;
            persister.ItemCopying += ItemCopyingEvenHandler;
            persister.ItemDeleting += ItemDeletingEvenHandler;
            persister.ItemMoving += ItemMovingEvenHandler;
            activator.ItemCreated += ItemCreatedEventHandler;
        }

        public virtual void Stop()
        {
            persister.ItemSaving -= ItemSavingEventHandler;
            persister.ItemCopying -= ItemCopyingEvenHandler;
            persister.ItemDeleting -= ItemDeletingEvenHandler;
            persister.ItemMoving -= ItemMovingEvenHandler;
            activator.ItemCreated -= ItemCreatedEventHandler;
        }

        #endregion
    }
}
