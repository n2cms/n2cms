using N2.Definitions;
using N2.Details;
using N2.Engine;
using N2.Plugin;
using N2.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Collaboration
{
	[Service]
	public class CollaborationInitializer : IAutoStart
	{
		private IDefinitionManager definitions;
		
		public CollaborationInitializer(IDefinitionManager definitions)
		{
			this.definitions = definitions;
		}

		public void Start()
		{
			foreach(var definition in definitions.GetDefinitions())
			{
				if (typeof(IRootPage).IsAssignableFrom(definition.ItemType))
				{
					if (!definition.Containers.Any(c => c.Name == "Collaboration"))
						definition.Add(new TabContainerAttribute("Collaboration", "Collaboration", 130) 
						{ 
							ContainerName = definition.Containers.Any(c => c.Name == "RootSettings")
								? "RootSettings"
								: null
						});

					if (!definition.Editables.Any(e => e.Name == "Messages"))
						definition.Add(new EditableChildrenAttribute("Messages", "Collaboration", "Messages", 100) { ContainerName = "Collaboration", MinimumTypeName = typeof(Items.ICollaborationPart).AssemblyQualifiedName });
				}
			}
		}

		public void Stop()
		{
		}
	}
}