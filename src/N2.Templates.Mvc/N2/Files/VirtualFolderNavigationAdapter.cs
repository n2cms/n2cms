using System;
using System.Collections.Generic;
using System.Web;
using N2.Edit;
using N2;
using N2.Workflow;
using N2.Edit.FileSystem.Items;
using N2.Web;
using N2.Engine;

namespace N2.Management.Files
{
	[Adapts(typeof(ContentItem))]
	public class VirtualFolderNavigationAdapter : NavigationAdapter
	{
		public override IEnumerable<ContentItem> GetChildren(ContentItem parent, string userInterface)
		{
			List<string> existingDirectories = new List<string>();

			foreach (var child in BaseGetChildren(parent, userInterface))
			{
				yield return child;
				if (child is AbstractDirectory)
					existingDirectories.Add(child.Url.TrimEnd('/').ToLower());
			}

			if (userInterface == Interfaces.Managing && Host.IsStartPage(parent))
			{
				foreach (var dir in GetUploadDirectories(parent))
				{
					if (!existingDirectories.Contains(dir.VirtualPath.TrimEnd('/').ToLower()))
					{
						yield return new Directory(dir, parent);
					}
				}
			}				
		}

		private IEnumerable<ContentItem> BaseGetChildren(ContentItem parent, string userInterface)
		{
			return base.GetChildren(parent, userInterface);
		}
	}
}
