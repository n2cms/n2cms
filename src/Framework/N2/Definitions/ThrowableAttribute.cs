using System;

namespace N2.Definitions
{
	/// <summary>
	/// Whether an item can be stored in the trash can. Default is Yes.
	/// </summary>
	public enum AllowInTrash
	{
		Yes,
		No
	}

	/// <summary>
	/// When used on an item definition this attribute can prevent it from beeing 
	/// moved to trash upon deletion.
	/// </summary>
	public class ThrowableAttribute : Attribute
	{
		public ThrowableAttribute(AllowInTrash throwable)
		{
			Throwable = throwable;
		}

		public AllowInTrash Throwable { get; private set; }
	}
}
