using System.Collections.Generic;

namespace N2.Addons.Tagging
{
    public interface IGroup
    {
        string Title { get; }
        string Name { get; }

        IEnumerable<ITag> GetTags();
        ITag GetOrCreateTag(string tagName);
    }
}
