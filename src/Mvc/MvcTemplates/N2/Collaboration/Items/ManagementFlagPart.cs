using N2.Definitions;
using N2.Details;
using N2.Edit.Collaboration;
using N2.Integrity;
using N2.Management.Collaboration;
using N2.Management.Myself;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Collaboration.Items
{
	public enum ManagementControlFlag
	{
		None,
		ShutdownEditing
	}

	[AllowedZones("Collaboration")]
	[PartDefinition("Management flag")]
	[Versionable(AllowVersions.No)]
	public class ManagementFlagPart : RootPartBase, IFlagSource, ICollaborationPart
	{
		[EditableEnum(typeof(ManagementControlFlag))]
		public virtual ManagementControlFlag Flag
		{
			get
			{
				ManagementControlFlag flag;
				Enum.TryParse<ManagementControlFlag>(Title, out flag);
				return flag;
			}
			set
			{
				Title = value.ToString();
			}
		}

		public IEnumerable<string> GetFlags(CollaborationContext context)
		{
			if (Flag != ManagementControlFlag.None)
				yield return Flag.ToString();
		}
	}
}