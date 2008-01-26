using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Persistence
{
	/// <summary>Used to mark that should not be copied.</summary>
	[AttributeUsage(AttributeTargets.Field)]
	internal class DoNotCopyAttribute : System.Attribute
	{
	}
}
