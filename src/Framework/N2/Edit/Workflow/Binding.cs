using System.Collections.Generic;
using N2.Definitions;

namespace N2.Edit.Workflow
{
    public static class Binding
    {
        const string Key = "ItemsToSave";
        const string UpdatedDetailsKey = "UpdatedDetailsKey";
        const string DefinedDetailsKey = "DefinedDetailsKey";

        public static void RegisterItemToSave(this CommandContext context, ContentItem item)
        {
            var itemsToSave = context.GetItemsToSave();
			if (!itemsToSave.Contains(item))
				itemsToSave.Add(item);
            context.Parameters[Key] = itemsToSave;
        }

        public static void UnregisterItemToSave(this CommandContext context, ContentItem item)
        {
            var itemsToSave = context.GetItemsToSave();
            if (itemsToSave.Contains(item))
            {
                itemsToSave.Remove(item);
                context.Parameters[Key] = itemsToSave;
            }
        }

        public static IList<ContentItem> GetItemsToSave(this CommandContext context)
        {
            if (context.Parameters.ContainsKey(Key))
                return context.Parameters[Key] as IList<ContentItem> ?? new List<ContentItem>();
            else
                return new List<ContentItem>();
        }

        public static IList<string> GetUpdatedDetails(this CommandContext context)
        {
            object updatedDetails;
            if (!context.Parameters.TryGetValue(UpdatedDetailsKey, out updatedDetails))
                context.Parameters[UpdatedDetailsKey] = updatedDetails = new List<string>();
            return updatedDetails as IList<string>;
        }

        public static IList<string> GetDefinedDetails(this CommandContext context)
        {
            object definedDetails;
            if (!context.Parameters.TryGetValue(DefinedDetailsKey, out definedDetails))
                context.Parameters[DefinedDetailsKey] = definedDetails = new List<string>();
            return definedDetails as IList<string>;
        }

        public static CommandContext CreateNestedContext(this CommandContext context, IContentForm<CommandContext> subBinder, ContentItem subItem, ItemDefinition subDefinition)
        {
            return new CommandContext(subDefinition, subItem, context.Interface, context.User)
            {
                Binder = subBinder,
                Errors = context.Errors,
                Parameters = context.Parameters,
                ValidationErrors = context.ValidationErrors,
                Validator = context.Validator
            };
        }
    }
}
