using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Tests.Edit.Items
{
	[Definition]
	public class NormalItem : ContentItem
	{
		public override bool IsPage
		{
			get
			{
				return false;
			}
		}
	}
}
