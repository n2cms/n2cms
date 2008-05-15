using System;
using System.Collections.Generic;
using System.Text;
using N2.Engine;
using N2.Plugin;
using N2.Templates.SEO;

namespace N2.Templates.SEO
{
	[AutoInitialize]
	public class SEOInitializer : IPluginInitializer
	{
		public void Initialize(IEngine engine)
		{
			engine.AddComponent("n2.templates.seo.definitions", typeof(SEODefinitionAppender));
			engine.AddComponent("n2.templates.seo.modifier", typeof(SEOPageModifier));
		}
	}
}
