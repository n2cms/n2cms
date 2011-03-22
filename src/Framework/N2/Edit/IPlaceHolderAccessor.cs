using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace N2.Edit
{
	public interface IPlaceHolderAccessor
	{
		Control GetPlaceHolder(string name);
	}
}
