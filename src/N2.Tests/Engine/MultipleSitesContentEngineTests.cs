using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Engine;
using N2.Web;
using System.Configuration;
using NUnit.Framework.SyntaxHelpers;
using System.IO;

namespace N2.Tests.Engine
{
	[TestFixture]
	public class MultipleSitesContentEngineTests
	{
		ContentEngine engine;

		[SetUp]
		public void SetUp()
		{
			string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Engine\\MultipleSites.exe");
			var cfg = ConfigurationManager.OpenExeConfiguration(path);
			engine = new ContentEngine(cfg);
		}

		[Test]
		public void MultipleSitesParser_IsConfigurable()
		{
			var parser = engine.Resolve<IUrlParser>();
			Assert.That(parser, Is.TypeOf(typeof(MultipleSitesParser)));
		}
	}
}
