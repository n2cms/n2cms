using System;
using System.Collections.Generic;
using System.Text;
using N2.Edit.Settings;

namespace N2.Tests.Edit.Settings.Services
{
	[N2.Web.UI.FieldSetContainer("default", "Default", 100)]
	public class JustOneOfThoseServices
	{
		private bool aBooleanProperty;

		[EditableCheckBox("Be useful", 100, ContainerName = "default")]
		public bool ABooleanProperty
		{
			get { return aBooleanProperty; }
			set { aBooleanProperty = value; }
		}
	}
}
