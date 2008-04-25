using System;
using System.Collections.Generic;
using System.Text;
using N2.Persistence;
using N2.Web;
using N2.Collections;
using Castle.Core;

namespace N2.Globalization
{
	public class LanguageGateway : ILanguageGateway, IStartable
	{
		public const string LanguageKey = "LanguageKey";

		private readonly IPersister persister;
		private readonly IWebContext context;
		private readonly Site site;
		private int recursionDepth = 3;
		
		public int RecursionDepth
		{
			get { return recursionDepth; }
			set { recursionDepth = value; }
		}

		public LanguageGateway(IPersister persister, IWebContext context, Site site)
		{
			this.persister = persister;
			this.context = context;
			this.site = site;
		}

		void persister_ItemSaved(object sender, ItemEventArgs e)
		{
			ContentItem item = e.AffectedItem;

			foreach (ContentItem ancestor in Find.EnumerateParents(item))
			{
				if (ancestor is ILanguageRoot)
				{
					int languageKey = item.ID;
					if (context.QueryString[LanguageKey] != null)
						int.TryParse(context.QueryString[LanguageKey], out languageKey);
					item[LanguageKey] =  languageKey;
					break;
				}
			}
		}

		public IEnumerable<ILanguageRoot> GetLanguages()
		{
			return new RecursiveFinder().Find<ILanguageRoot>(persister.Get(site.RootItemID), RecursionDepth);
		}

		#region IStartable Members

		public void Start()
		{
			persister.ItemSaved += persister_ItemSaved;
		}

		public void Stop()
		{
			persister.ItemSaved -= persister_ItemSaved; ;
		}

		#endregion
	}
}
