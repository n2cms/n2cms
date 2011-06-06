using System.Collections.Generic;
using System.Linq;
using N2.Collections;
using N2.Web;
using N2.Persistence;

namespace N2
{
	/// <summary>
	/// Provides access to common functions.
	/// </summary>
	public static class Content
	{
		/// <summary>
		/// Provides access to filters applyable to content items.
		/// </summary>
		public static FilterHelper Is
		{
			get { return new FilterHelper(Context.Current); }
		}

		/// <summary>
		/// Simplifies traversing items in the content hierarchy.
		/// </summary>
		public static TraverseHelper Traverse
		{
			get { return new TraverseHelper(Context.Current, Is, () => Context.Current.Resolve<IWebContext>().CurrentPath); }
		}


		/// <summary>
		/// Simplifies access to APIs related to search and querying.
		/// </summary>
		public static SearchHelper Search
		{
			get { return new SearchHelper(Context.Current); }
		}
	}
}
