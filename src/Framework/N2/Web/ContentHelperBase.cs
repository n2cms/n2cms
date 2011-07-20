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

		/// <summary>Traverse the content hieararchy.</summary>
		public virtual TraverseHelper Traverse
		{
			get { return new TraverseHelper(Engine, Is, PathGetter); }
		}

		/// <summary>Filter collections of items.</summary>
		public virtual FilterHelper Is
		{
			get { return new FilterHelper(Engine); }
		}

		/// <summary>Search for content stored in the system.</summary>
		public SearchHelper Search
		{
			get { return new SearchHelper(Engine); }
		}

		/// <summary>Access items in the current context.</summary>
		public ContextHelper Current
		{
			get { return new ContextHelper(Engine, PathGetter); }
		}

		/// <summary>Get a content helper for an alternative scope.</summary>
		/// <param name="otherContentItem">The current item of the alternative scope.</param>
		/// <returns>Another content helper with a different scope.</returns>
		public virtual ContentHelperBase At(ContentItem otherContentItem)
		{
			EnsureAuthorized(otherContentItem);

			return new ContentHelperBase(Engine, () => new PathData { CurrentItem = otherContentItem, CurrentPage = Current.Page});
		}

		/// <summary>Begins a new scope using the current content helper.</summary>
		/// <param name="newCurrentItem">The current item to use in the new scope.</param>
		/// <returns>An object that restores the scope upon disposal.</returns>
		public IDisposable BeginScope(ContentItem newCurrentItem)
		{
			if (newCurrentItem == null) return new EmptyDisposable();

			EnsureAuthorized(newCurrentItem);

			return new ContentScope(newCurrentItem, this);
		}

		private void EnsureAuthorized(ContentItem newCurrentItem)
		{
			var user = Services.Resolve<IWebContext>().User;
			if (!Engine.SecurityManager.IsAuthorized(newCurrentItem, user))
				throw new PermissionDeniedException(newCurrentItem, user);
		}

		/// <summary>Begins a new scope using the current content helper.</summary>
		/// <param name="newCurrentItemUrlOrId">A string that is parsed as an item id or item url.</param>
		/// <returns>An object that restores the scope upon disposal.</returns>
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

		/// <summary>Begins a new scope using the current content helper.</summary>
		/// <param name="newCurrentItemUrlOrId">A string that is parsed as an item id or item url.</param>
		/// <param name="reallyBeginScope">Option to keep the current scope when the paramter is false.</param>
		/// <returns>An object that restores the scope upon disposal.</returns>
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
				contentHelper.PathGetter = () => new PathData { CurrentItem = newCurrentItem, CurrentPage = newCurrentItem.IsPage ? newCurrentItem : previousGetter().CurrentPage };
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
