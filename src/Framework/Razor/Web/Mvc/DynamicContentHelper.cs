using System.Web;
using System.Web.Mvc;
using N2.Engine;
using System;
using N2.Web.Mvc.Html;
using N2.Engine.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace N2.Web.Mvc
{
    /// <remarks>This code is here since it has dependencies on ASP.NET 3.0 which isn't a requirement for N2 in general.</remarks>
    public class DynamicContentHelper : ViewContentHelper
    {
        public DynamicContentHelper(HtmlHelper html)
            : base(html)
        {
        }

        public DynamicContentHelper(HtmlHelper html, Func<IEngine> engine, Func<PathData> pathGetter)
            : base(html, engine, pathGetter)
        {
        }

        public dynamic Display
        {
            get { return new DisplayHelper { Html = Html, Current = Current.Item }; }
        }

        public dynamic Has
        {
            get 
			{
				return new HasValueHelper(HasValue);
			}
        }

        public dynamic Data
        {
            get
            {
                if (Current.Item == null)
                    return new DataHelper(() => Current.Item);

                string key = "DataHelper" + Current.Item.ID;
                var data = Html.ViewContext.ViewData[key] as DataHelper;
                if (data == null)
                    Html.ViewContext.ViewData[key] = data = new DataHelper(() => Current.Item);
                return data;
            }
        }

        public TranslateHelper Translate
        {
            get { return new TranslateHelper(Engine.Container.ResolveAll<N2.Engine.Globalization.ITranslator>()); }
        }

        public class TranslateHelper
        {
			private IEnumerable<ITranslator> translators;

			public TranslateHelper(IEnumerable<ITranslator> translators)
			{
				this.translators = translators ?? new ITranslator[0];
			}

			public IHtmlString this[string key]
			{
				get { return Html(key, key); }
			}

			public IHtmlString this[string key, string fallback]
			{
				get { return Html(key, fallback); }
			}

			public IHtmlString Html(string key, string fallback = null)
            {
                return new HtmlString(Text(key));
            }

            public string Text(string key, string fallback = null)
            {
                return translators.Select(t => t.Translate(key, fallback ?? key)).FirstOrDefault(t => t != null) ?? fallback ?? key;
            }
        }

        /// <summary>Gives a content helper that with the given item in scope. The scope of the surrouding page is not changed.</summary>
        /// <param name="otherContentItem">The content item to operate on.</param>
        /// <returns>A content helper with a different scope than the surrounding page.</returns>
        public new virtual DynamicContentHelper At(ContentItem otherContentItem)
        {
			if (otherContentItem == null)
				throw new ArgumentNullException("otherContentItem");

            EnsureAuthorized(otherContentItem);

            return new DynamicContentHelper(Html, Current.EngineGetter, () => new PathData { CurrentItem = otherContentItem, CurrentPage = otherContentItem.IsPage ? otherContentItem : Current.Page });
        }
    }
}
