using System;
using System.Collections.Generic;
using System.Linq;
using N2.Edit;
using N2.Edit.FileSystem.Items;
using N2.Engine;
using N2.Persistence.Sources;
using N2.Web;
using N2.Persistence;
using N2.Management.Files;
using N2.Definitions;

namespace N2.Management.Files
{
	[Service(typeof(SourceBase))]
	public class FileSystemSource : SourceBase
	{
		private FolderNodeProvider nodes;
		
		public FileSystemSource(FolderNodeProvider nodes)
		{
			this.nodes = nodes;
			BaseContentType = typeof(AbstractNode);
		}

		public override IEnumerable<ContentItem> AppendChildren(IEnumerable<ContentItem> previousChildren, Query query)
		{
			if(query.Interface != Interfaces.Managing)
				return previousChildren;

			return previousChildren.Union(nodes.GetChildren(query.Parent.Path));
		}

		public override bool IsProvidedBy(ContentItem item)
		{
			return item is AbstractNode;
		}

		public override PathData ResolvePath(ContentItem startingPoint, string path)
		{
			return ResolvePath(startingPoint.Path + path.TrimStart('/'));
		}

		public override PathData ResolvePath(string path)
		{
			var item = nodes.Get(path);
			if (item == null)
				return PathData.Empty;

			return item.FindPath(PathData.DefaultAction);
		}

		public override void Save(ContentItem item)
		{
			Active(item).Save();
		}

		public override void Delete(ContentItem item)
		{
			Active(item).Delete();
		}

		public override void Move(ContentItem source, ContentItem destination)
		{
			Active(source).MoveTo(destination);
		}

		public override ContentItem Copy(ContentItem source, ContentItem destination)
		{
			return Active(source).CopyTo(destination);
		}

		private IActiveContent Active(ContentItem item)
		{
			return (IActiveContent)item;
		}
	}
}