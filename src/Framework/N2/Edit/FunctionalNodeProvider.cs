using System;
using System.Collections.Generic;

namespace N2.Edit
{
    public class FunctionalNodeProvider : INodeProvider
    {
        public FunctionalNodeProvider(string path, NodeFactoryDelegate factoryMethod)
        {
            Path = path;
            ParentPath = N2.Web.Url.RemoveLastSegment(Path);
            FactoryMethod = factoryMethod;
        }

        public NodeFactoryDelegate FactoryMethod { get; protected set; }
        public string Path { get; protected set; }
        public string ParentPath { get; protected set; }

        public virtual ContentItem Get(string path)
        {
            if (path.StartsWith(Path, StringComparison.InvariantCultureIgnoreCase))
                return FactoryMethod(path.Substring(Path.Length));

            return null;
        }

        public virtual IEnumerable<ContentItem> GetChildren(string path)
        {
            if (string.Equals(ParentPath, path, StringComparison.InvariantCultureIgnoreCase))
                yield return FactoryMethod("");
        }
    }
}
