using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Collections;
using N2.Linq;
using N2.Persistence.Finder;
using N2.Persistence;
using N2.Security;

namespace N2.Web
{
	/// <summary>
	/// Provides quick acccess to often used APIs.
	/// </summary>
	public class ContentHelperBase
	{
		public ContentHelperBase(IEngine engine, Func<PathData> pathGetter)
		{
			Engine = engine;
			PathGetter = pathGetter;
		}

		// accessors

		public IEngine Engine { get; protected set; }

		public virtual IServiceContainer Services
		{
			get { return Engine.Container; }
		}

		protected Func<PathData> PathGetter { get; set; }

		public PathData Path
		{
			get { return PathGetter(); }
		}

		// traversing & filtering

		public virtual TraverseHelper Traverse
		{
			get { return new TraverseHelper(Engine, Is, PathGetter); }
		}

		public virtual FilterHelper Is
		{
			get { return new FilterHelper(Engine); }
		}

		// querying & finding

		public SearchHelper Search
		{
			get { return new SearchHelper(Engine); }
		}

		// changing scope

		public virtual ContentHelperBase At(ContentItem otherContentItem)
		{
			EnsureAuthorized(otherContentItem);

			return new ContentHelperBase(Engine, () => new PathData { CurrentItem = otherContentItem, CurrentPage = Path.CurrentPage });
		}

		public IDisposable BeginScope(ContentItem newCurrentItem)
		{
			EnsureAuthorized(newCurrentItem);

			return new ContentScope(newCurrentItem, this);
		}

		private void EnsureAuthorized(ContentItem newCurrentItem)
		{
			var user = Services.Resolve<IWebContext>().User;
			if (!Engine.SecurityManager.IsAuthorized(newCurrentItem, user))
				throw new PermissionDeniedException(newCurrentItem, user);
		}

		public IDisposable BeginScope(string newCurrentItemUrlOrId)
		{
			if (newCurrentItemUrlOrId != null)
			{
				ContentItem item = Parse(newCurrentItemUrlOrId);

				if (item != null)
					return BeginScope(item);
			}
			return new EmptyDisposable();
		}

		private ContentItem Parse(string newCurrentItemUrlOrId)
		{
			int id;
			ContentItem item = null;
			if (int.TryParse(newCurrentItemUrlOrId, out id))
				item = Engine.Persister.Get(id);

			if (item == null)
				item = Services.Resolve<IUrlParser>().Parse(newCurrentItemUrlOrId);
			return item;
		}

		public IDisposable BeginScope(string newCurrentItemUrlOrId, bool reallyBeginScope)
		{
			if (!reallyBeginScope)
				return new EmptyDisposable();

			return BeginScope(newCurrentItemUrlOrId);
		}

		#region class EmptyDisposable
		class EmptyDisposable : IDisposable
		{
			#region IDisposable Members

			public void Dispose()
			{
			}

			#endregion
		}
		#endregion

		#region class ContentScope
		class ContentScope : IDisposable
		{
			ContentHelperBase contentHelper;
			Func<PathData> previousGetter;

			public ContentScope(ContentItem newCurrentItem, ContentHelperBase contentHelper)
			{
				this.contentHelper = contentHelper;
				previousGetter = contentHelper.PathGetter;
				contentHelper.PathGetter = () => new PathData { CurrentItem = newCurrentItem, CurrentPage = previousGetter().CurrentPage };
			}

			#region IDisposable Members

			public void Dispose()
			{
				contentHelper.PathGetter = previousGetter;
			}

			#endregion
		}
		#endregion
	}
}
