using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine.Globalization;
using N2.Engine;

namespace N2.Web
{
	public class ContextHelper
	{
		IEngine engine;
		private Func<PathData> PathGetter { get; set; }

		public ContextHelper(IEngine engine, Func<PathData> pathGetter)
		{
			this.engine = engine;
			this.PathGetter = pathGetter;
		}

		public PathData Path
		{
			get { return PathGetter() ?? PathData.Empty; }
		}

		public ContentItem Page
		{
			get { return Path.CurrentPage; }
		}

		public ContentItem Item
		{
			get { return Path.CurrentItem; }
		}

		public ContentItem Part
		{
			get { return Path.CurrentItem != null && !Path.CurrentItem.IsPage ? Path.CurrentItem : null; }
		}

		public ILanguage Language
		{
			get { return engine.Resolve<ILanguageGateway>().GetLanguage(Page); }
		}

		public bool IsPage
		{
			get { return Item != null && Item.IsPage; }
		}

		public bool IsEmpty
		{
			get { return Item != null; }
		}
	}
}
