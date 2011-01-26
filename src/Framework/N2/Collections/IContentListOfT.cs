using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Collections
{
	public interface IContentList<T> : IList<T>, INamedList<T>, IPageableList<T> where T : class, INameable
	{
	}
}
