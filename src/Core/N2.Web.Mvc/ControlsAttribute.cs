using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Mvc
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ControlsAttribute : Attribute, IComparable<ControlsAttribute>
	{
		private readonly Type itemType;
		private Type controllerType;

		public ControlsAttribute(Type itemType)
		{
			this.itemType = itemType;
		}

		public Type ItemType
		{
			get { return itemType; }
		}

		public Type ControllerType
		{
			get { return controllerType; }
			set { controllerType = value; }
		}

		#region IComparable<ControlsAttribute> Members

		public int CompareTo(ControlsAttribute other)
		{
			Type otherType = other.ItemType;

			if (otherType.IsSubclassOf(ItemType))
				return int.MaxValue;
			else if (ItemType.IsSubclassOf(otherType))
				return int.MinValue;
			else
				return ItemType.Name.CompareTo(otherType.Name);
		}

		#endregion
	}
}
