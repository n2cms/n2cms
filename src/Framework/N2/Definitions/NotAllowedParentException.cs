using System;

namespace N2.Definitions
{
    /// <summary>This exceptions is thrown when trying to add an item to an unsupported parent item.</summary>
    public class NotAllowedParentException : N2Exception
    {
        public NotAllowedParentException(ItemDefinition itemDefinition, Type parentType)
            : base("The item '{0}' isn't allowed below a destination of type '{1}'.", 
                itemDefinition.Title,
                parentType.AssemblyQualifiedName)
        {
            this.itemDefinition = itemDefinition;
            this.parentType = parentType;
        }

        private ItemDefinition itemDefinition;
        private Type parentType;

        public ItemDefinition ItemDefinition
        {
            get { return itemDefinition; }
        }

        public Type ParentType
        {
            get { return parentType; }
        }
    }
}
