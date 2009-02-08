using System;
using System.Collections.Generic;

namespace N2.Edit.Settings
{
	[Obsolete]
	public interface ISettingsProvider
	{
		N2.Definitions.IEditableContainer RootContainer { get; set; }
		IList<IServiceEditable> Settings { get; set; }
	}
}
