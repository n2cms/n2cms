using System.Collections.Generic;

namespace N2.Workflow
{
	internal static class Binding
	{
		const string Key = "ItemsToSave";

		public static void RegisterItemToSave(this CommandContext context, ContentItem item)
		{
			var itemsToSave = context.GetItemsToSave();
			itemsToSave.Add(item);
			context.Parameters[Key] = itemsToSave;
		}

		public static IList<ContentItem> GetItemsToSave(this CommandContext context)
		{
			if (context.Parameters.ContainsKey(Key))
				return context.Parameters[Key] as IList<ContentItem> ?? new List<ContentItem>();
			else
				return new List<ContentItem>();
		}

		public static CommandContext CreateNestedContext(this CommandContext context, IBinder<CommandContext> subBinder, ContentItem subItem)
		{
			return new CommandContext(subItem, context.Interface, context.User)
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
