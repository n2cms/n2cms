using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Raven.Tests
{
	public class MyItem : ContentItem
	{
		public string Text
		{
			get { return GetDetail("Text", ""); }
			set { SetDetail("Text", value, ""); }
		}
	}
}
