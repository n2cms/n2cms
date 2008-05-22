using N2.Engine;
using N2.Plugin;
using N2.Templates.Rss;
using N2.Templates.Syndication;

namespace N2.Templates.Rss
{
	/// <summary>
	/// Registers components needed to mark items for syndication and and write 
	/// an RSS feed to the output stream.
	/// </summary>
	[AutoInitialize]
	public class SyndicationInitializer : IPluginInitializer
	{
		public void Initialize(IEngine engine)
		{
			engine.AddComponent("n2.templates.syndication.rssWriter", typeof(RssWriter));
			engine.AddComponent("n2.templates.rss.definitionAppender", typeof(SyndicatableDefinitionAppender));
		}
	}
}