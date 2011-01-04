using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Tests.Engine.Items
{
	public interface IInterfacedItem
	{
	}

	public class ItemAC : ItemA, IInterfacedItem
	{
	}
}
