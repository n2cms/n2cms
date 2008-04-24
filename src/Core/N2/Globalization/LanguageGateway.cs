using System;
using System.Collections.Generic;
using System.Text;
using N2.Persistence;
using N2.Web;
using N2.Collections;

namespace N2.Globalization
{
	public class LanguageGateway : ILanguageGateway
	{
		private readonly IPersister persister;
		private readonly Site site;
		private int recursionDepth = 3;

		public int RecursionDepth
		{
			get { return recursionDepth; }
			set { recursionDepth = value; }
		}

		public LanguageGateway(IPersister persister, Site site)
		{
			this.persister = persister;
			this.site = site;
		}

		public IEnumerable<ILanguageRoot> GetLanguages()
		{
			return new RecursiveFinder().Find<ILanguageRoot>(persister.Get(site.RootItemID), RecursionDepth);
		}
	}
}
