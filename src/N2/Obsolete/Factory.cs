using System;
using System.ComponentModel;

namespace N2
{
	[Obsolete("This class has been renamed to N2.Context.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class Factory : Context
	{
		/// <summary>Access to the singleton N2 engine. This property has been deprecated in favor for 'Current'.</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Name changed to Current", true)]
		public static Engine.IEngine Instance
		{
			get { return Current; }
		}
	}
}
