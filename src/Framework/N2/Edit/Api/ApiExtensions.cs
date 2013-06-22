using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Management.Api
{
	public static class ApiExtensions
	{
		public static Node<InterfaceMenuItem> FindRecursive(this Node<InterfaceMenuItem> startingPoint, string requestedName)
		{
			if (startingPoint.Current != null && startingPoint.Current.Name == requestedName)
				return startingPoint;
			if (startingPoint.Children != null)
				foreach (var child in startingPoint.Children)
				{
					var node = FindRecursive(child, requestedName);
					if (node != null)
						return node;
				}
			return null;
		}

		public static void Add(this Node<InterfaceMenuItem> startingPoint, string parentName, Node<InterfaceMenuItem> nodeToAdd, bool requireParent = false, int insertIndex = -1, string insertBeforeSiblingWithName = null, string insertAfterSiblingWithName = null)
		{
			var parentNode = FindRecursive(startingPoint, parentName);
			if (parentNode == null && requireParent && parentName != null)
				throw new Exception("Missing parent with name " + parentName);
			if (parentNode == null)
				parentNode = startingPoint;

			var siblings = (parentNode.Children ?? new Node<InterfaceMenuItem>[0]).ToList();

			if (insertBeforeSiblingWithName != null)
				insertIndex = siblings.FindIndex(n => n.Current != null && n.Current.Name == insertBeforeSiblingWithName);
			else if (insertAfterSiblingWithName != null)
			{
				insertIndex = siblings.FindIndex(n => n.Current != null && n.Current.Name == insertAfterSiblingWithName);
				if (insertIndex >= 0)
					insertIndex++;
			}

			if (insertIndex >= 0)
				siblings.Insert(insertIndex, nodeToAdd);
			else
				siblings.Add(nodeToAdd);

			parentNode.Children = siblings;
		}
	}
}
