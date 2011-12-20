using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;

namespace N2.RavenDB.Tests
{
	[TestFixture]
	public class Test
	{
		public class MyItem : ContentItem
		{
		}


		[Test]
		public void SerializeContentITem()
		{
			var value = new MyItem { Title = "Hello" };
			value.Children.Add(new MyItem { Title = "Child" });
			using (var sw = new StringWriter())
			using (var jtw = new JsonTextWriter(sw) { Indentation = 1, IndentChar = '\t', Formatting = Formatting.Indented })
			{
				var js = new Newtonsoft.Json.JsonSerializer();
				js.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				js.PreserveReferencesHandling = PreserveReferencesHandling.All;
				//js.ReferenceResolver = new RavenReferenceResolver();
				js.Serialize(jtw, value);

				Debug.WriteLine(sw.ToString());
			}
		}
	}
}
