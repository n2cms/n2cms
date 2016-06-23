using N2.Web;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2.Engine.Globalization
{
	[Service]
	public class MissingTranslationsCollector
	{
		ConcurrentDictionary<int, ConcurrentDictionary<string, string>> cache = new ConcurrentDictionary<int, ConcurrentDictionary<string, string>>();

		internal void Add(int id, string key, string fallback)
		{
			var translations = cache.GetOrAdd(id, CreateTranslations);
			translations.GetOrAdd(key, fallback);
		}

		private ConcurrentDictionary<string, string> CreateTranslations(int id)
		{
			return new ConcurrentDictionary<string, string>();
		}

		public IDictionary<string, string> Get(int id)
		{
			return cache.GetOrAdd(id, CreateTranslations);
		}
	}

	[Service(typeof(ITranslator))]
	public class ContentTranslator : ITranslator
	{
		private IWebContext context;
		private MissingTranslationsCollector missings;

		public ContentTranslator(IWebContext context, MissingTranslationsCollector missings)
		{
			this.context = context;
			this.missings = missings;
		}

		public string Translate(string key, string fallback = null)
		{
			var translator = Find.Closest<ITranslator>(context.CurrentPage);
			if (translator == null)
				return null;

			string translation = translator.Translate(key);
			if (translation == null)
				missings.Add((translator as ContentItem).ID, key, fallback);

			return translation;
		}

		public IDictionary<string, string> GetTranslations()
		{
			var translator = Find.Closest<ITranslator>(context.CurrentPage);
			if (translator == null)
				return new Dictionary<string, string>();
            return translator.GetTranslations() ?? new Dictionary<string, string>();
		}
	}
}
