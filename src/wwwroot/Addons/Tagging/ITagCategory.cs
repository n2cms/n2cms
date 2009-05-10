using System.Collections.Generic;

namespace N2.Addons.Tagging
{
	public interface ITagCategory
	{
		string Title { get; }
		string Name { get; }

		IEnumerable<ITag> GetTags();
		ITag GetOrCreateTag(string tagName);
	}
}
