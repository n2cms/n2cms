using System;
using System.Collections.Generic;
using N2.Definitions;
using N2.Engine;
using N2.Persistence.Finder;
using N2.Edit.Versioning;
using N2.Persistence;

namespace N2.Integrity
{
    /// <summary>
    /// A class that encapsulates integrity checks performed when content 
    /// items are manipulated through <see cref="Context"/>. This class' 
    /// responsibilities include maintaining system integrity integrity by 
    /// not allowing recusive parent-child relationships, siblings with the 
    /// same name and un-allowed parent-child combinations.
    /// </summary>
    [Service(typeof(IIntegrityManager))]
    public class IntegrityManager : IIntegrityManager
    {
        readonly Web.IUrlParser urlParser;
        readonly IContentItemRepository repository;
        readonly Definitions.IDefinitionManager definitions;

        #region Constructor
        /// <summary>Creates a new instance of the <see cref="IntegrityManager"/>.</summary>
        /// <param name="definitions">The definition manager.</param>
        /// <param name="finder"></param>
        /// <param name="urlParser"></param>
        public IntegrityManager(Definitions.IDefinitionManager definitions, IContentItemRepository repository, Web.IUrlParser urlParser)
        {
            this.definitions = definitions;
            this.repository = repository;
            this.urlParser = urlParser;
        }

        #endregion

        #region Methods

        /// <summary>Check if an item can be moved to a destination.</summary>
        /// <param name="source">The item to move.</param>
        /// <param name="destination">The destination below which the item should be moved to.</param>
        /// <returns>True if the item can be moved.</returns>
        public bool CanMove(ContentItem source, ContentItem destination)
        {
            return null == GetMoveException(source, destination);
        }

        /// <summary>Check if an item can be copied to a destination.</summary>
        /// <param name="source">The item to copy.</param>
        /// <param name="destination">The destination below which the item should be copied to.</param>
        /// <returns>True if the item can be copied.</returns>
        public bool CanCopy(ContentItem source, ContentItem destination)
        {
            return null == GetCopyException(source, destination);
        }

        /// <summary>Check the current state of an item to see if it ca be deleted.</summary>
        /// <param name="item">The item that should be deleted.</param>
        /// <returns>True if the item can be deleted.</returns>
        public bool CanDelete(ContentItem item)
        {
            return null == GetDeleteException(item);
        }

        /// <summary>Check the current state of an item to see if it ca be saved.</summary>
        /// <param name="item">The item that should be saved.</param>
        /// <returns>True if the item can be saved.</returns>
        public bool CanSave(ContentItem item)
        {
            return null == GetSaveException(item);
        }
        #endregion

        #region Helper Methods
        /// <summary>Checks if an item can be moved to a destination.</summary>
        /// <param name="source">The item that is to be moved.</param>
        /// <param name="destination">The destination onto which the item is to be moved.</param>
        /// <returns>Null if the item can be moved or an exception if the item can't be moved.</returns>
        public virtual Exception GetMoveException(ContentItem source, ContentItem destination)
        {
            if (IsDestinationBelowSource(source, destination))
                return new DestinationOnOrBelowItselfException(source, destination);

            if (IsNameOccupiedOnDestination(source, destination, excludeMyself: true))
                return new NameOccupiedException(source, destination);

            if (!IsTypeAllowedBelowDestination(source, destination))
                return new Definitions.NotAllowedParentException(definitions.GetDefinition(source), destination.GetContentType());

            return null;
        }

        /// <summary>Check if an item can be copied.</summary>
        /// <exception cref="NameOccupiedException"></exception>
        /// <exception cref="Exception"></exception>
        public virtual Exception GetCopyException(ContentItem source, ContentItem destination)
        {
            if (IsNameOccupiedOnDestination(source, destination, excludeMyself: false))
                return new NameOccupiedException(source, destination);

            if (!IsTypeAllowedBelowDestination(source, destination))
                return new Definitions.NotAllowedParentException(definitions.GetDefinition(source), destination.GetContentType());

            return null;
        }
        
        /// <summary>Check if an item can be deleted.</summary>
        /// <exception cref="Exception"></exception>
        public virtual Exception GetDeleteException(ContentItem item)
        {
            if (urlParser.IsRootOrStartPage(item))
                return new CannotDeleteRootException();

            return null;
        }

        /// <summary>Check if an item can be saved.</summary>
        /// <exception cref="NameOccupiedException"></exception>
        /// <exception cref="Exception"></exception>
        public virtual Exception GetSaveException(ContentItem item)
        {
            if (!IsLocallyUnique(item.Name, item))
                return new NameOccupiedException(item, item.Parent);

            return null;
        }

        /// <summary>Check if an item can be created.</summary>
        /// <param name="item"></param>
        /// <param name="parent"></param>
        /// <returns>The exception that would be thrown if the item was created.</returns>
        public Exception GetCreateException(ContentItem item, ContentItem parent)
        {
            ItemDefinition parentDefinition = definitions.GetDefinition(parent);
            ItemDefinition itemDefinition = definitions.GetDefinition(item);

            if (parentDefinition == null) throw new InvalidOperationException("Couldn't find a definition for the parent item '" + parent + "' of type '" + parent.GetContentType() + "'");
            if (itemDefinition == null) throw new InvalidOperationException("Couldn't find a definition for the item '" + item + "' of type '" + item.GetContentType() + "'");

            if (!parentDefinition.IsChildAllowed(definitions, parent, itemDefinition))
                return new NotAllowedParentException(itemDefinition, parent.GetContentType());

            return null;
        }

        /// <summary>Checks that destination have no child item with the same name.</summary>
        public virtual bool IsNameOccupiedOnDestination(ContentItem source, ContentItem destination, bool excludeMyself = true)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (destination == null) throw new ArgumentNullException("destination");

            ContentItem existingItem = destination.GetChild(source.Name);

            if (existingItem == null)
                return false;
            if (excludeMyself && existingItem == source)
                return false;
            if (!excludeMyself && source.Name == source.ID.ToString())
                return false;

            return true;
        }

        /// <summary>Checks that the destination isn't below the source.</summary>
        public virtual bool IsDestinationBelowSource(ContentItem source, ContentItem destination)
        {
            for (ContentItem parent = destination; parent != null; parent = parent.Parent)
                if (parent == source)
                    return true;
            return false;
        }

        /// <summary>Find out if an item name is occupied.</summary>
        /// <param name="name">The name to check.</param>
        /// <param name="item">The item whose siblings (other items with the same parent) might be have a clashing name.</param>
        /// <returns>True if the name is unique.</returns>
        public virtual bool IsLocallyUnique(string name, ContentItem item)
        {
            if (name == null || item == null) return true;

            ContentItem parentItem = item.Parent;
            if (parentItem != null)
            {
                var similarItems = GetItemsWithSameName(name, parentItem);
                foreach (var potentiallyClashingItem in similarItems)
                {
                    if (item.Equals(potentiallyClashingItem))
                        // it's me!
                        continue;
                    if ((item.ID == 0 && item.State != ContentState.New) ^ potentiallyClashingItem.ID == 0)
                        // it's probably a version of myself, TODO: save version key on master item
                        continue;

                    return false;
                }
            }
            return true;
        }

        private IEnumerable<ContentItem> GetItemsWithSameName(string name, ContentItem parentItem)
        {
            var siblings = (parentItem.ID != 0)
                ? repository.Find(Parameter.Equal("Parent", parentItem) & Parameter.Like("Name", name))
                : parentItem.Children;

            foreach (var sibling in siblings)
                if(string.Equals(sibling.Name, name, StringComparison.InvariantCultureIgnoreCase))
                    yield return sibling;
        }

        /// <summary>Check that the source item type is allowed below the destination. Throws an exception if the item isn't allowed.</summary>
        /// <param name="source">The child item</param>
        /// <param name="destination">The parent item</param>
        public virtual bool IsTypeAllowedBelowDestination(ContentItem source, ContentItem destination)
        {
            if (destination != null)
            {
                Definitions.ItemDefinition sourceDefinition = definitions.GetDefinition(source);
                Definitions.ItemDefinition destinationDefinition = definitions.GetDefinition(destination);

                return destinationDefinition.IsChildAllowed(definitions, destination, sourceDefinition, source);
            }
            return true;
        }
        #endregion

    }
}
