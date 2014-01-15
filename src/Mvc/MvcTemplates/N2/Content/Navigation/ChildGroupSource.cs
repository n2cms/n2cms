using N2.Collections;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Edit;
using N2.Edit.Navigation;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.Sources;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Content.Navigation
{
    [ContentSource]
    public class ChildGroupSource : SourceBase<ChildGroupContainer>
    {
        private DefinitionMap map;
        public ChildGroupSource(DefinitionMap map)
        {
            this.map = map;
        }

        private ContentSource Sources
        {
            get { return Engine.Resolve<ContentSource>(); }
        }

        public override PathData ResolvePath(ContentItem startingPoint, string path)
        {
            if (path.Contains("virtual-grouping"))
            {
                var parentPath = startingPoint.FindPath(path);
                if (parentPath.StopItem != null)
                {
                    var parent = parentPath.StopItem;

                    if (parent == null)
                        return base.ResolvePath(startingPoint, path);

                    var attributes = map.GetOrCreateDefinition(parent).GetCustomAttributes<GroupChildrenAttribute>();
                    foreach (var attribute in attributes)
                    {
                        var name = HttpUtility.UrlDecode(parentPath.Argument).Trim('/');
                        var argument = name.Split('/')[1];

                        var group = Sources.GetChildren(new Query { Parent = parent, OnlyPages = null }).FirstOrDefault(i => i.Name == name);
                        if (group != null)
                            return new PathData(group);
                    }
                }
            }
            return base.ResolvePath(startingPoint, path);
        }

        public override IEnumerable<ContentItem> AppendChildren(IEnumerable<ContentItem> previousChildren, Query query)
        {
            return previousChildren;
        }

        public override IEnumerable<ContentItem> FilterChildren(IEnumerable<ContentItem> previousChildren, Query query)
        {
            if (query.Parent is ChildGroupContainer)
            {
                return query.Parent.Children;
            }

            var attributes = map.GetOrCreateDefinition(query.Parent).GetCustomAttributes<GroupChildrenAttribute>();
            foreach (var attribute in attributes)
            {
                return attribute.FilterChildren(previousChildren, query, ChildGroupContainer.Create);
            }

            return previousChildren;
        }

        public override bool IsProvidedBy(ContentItem item)
        {
            return item is ChildGroupContainer;
        }

        public override ContentItem Get(object id)
        {
            return null;
        }

        public override bool ProvidesChildrenFor(ContentItem parent)
        {
            return !(parent is IRootPage);
        }

        public override void Save(ContentItem item)
        {
        }

        public override void Delete(ContentItem item)
        {
            if (item is ChildGroupContainer)
            {
                foreach (var child in item.Children)
                    Sources.Delete(child);
            }
        }

        public override ContentItem Move(ContentItem source, ContentItem destination)
        {
            if (source is ChildGroupContainer)
            {
                foreach (var child in source.Children)
                    Sources.Move(child, destination);
            }
            return new ChildGroupContainer(destination, source.Title, source.Name, () => destination.Children);
        }

        public override ContentItem Copy(ContentItem source, ContentItem destination)
        {
            if (source is ChildGroupContainer)
            {
                foreach (var child in source.Children)
                    Sources.Copy(child, destination);
            }
            return new ChildGroupContainer(destination, source.Title, source.Name, () => destination.Children);
        }
    }
}
