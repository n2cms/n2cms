using System;
using System.Collections.Generic;
using N2.Definitions;

namespace N2.Definitions
{
	public enum AllowedDefinitionResult
	{
		Allow,
		Deny,
		DontCare
	}

	public class AllowedDefinitionContext
	{
		public IDefinitionManager Definitions { get; set; }
		public ContentItem Parent { get; set; }
		public ItemDefinition ParentDefinition { get; set; }
		public ItemDefinition ChildDefinition { get; set; }
	}

	public interface IAllowedDefinitionFilter
	{
		AllowedDefinitionResult IsAllowed(AllowedDefinitionContext context);
	}
}
