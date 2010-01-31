using System.Text;
using N2.Web;
using System;

namespace N2.Engine
{
	public interface IContentAdapterProvider
	{		
		/// <summary>Resolves the controller for the current Url.</summary>
		/// <returns>A suitable controller for the given Url.</returns>
		T ResolveAdapter<T>(Type contentType) where T : AbstractContentAdapter;

		/// <summary>Resolves the controller for the current Url.</summary>
		/// <returns>A suitable controller for the given Url.</returns>
		[Obsolete("Use T ResolveAdapter<T>(ContentItem item)", true)]
		T ResolveAdapter<T>(PathData path) where T : AbstractContentAdapter;
	}
}
