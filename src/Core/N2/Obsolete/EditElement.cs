using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using System.ComponentModel;

namespace N2.Configuration
{
	/// <summary>The EditElement configures settings related to the edit interface.</summary>
	[Obsolete("Replaced by castle windsor configuration")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class EditElement : ConfigurationElement
	{
	}
}
