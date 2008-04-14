using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Edit.Web;
using System.Reflection;
using System.Collections.Generic;
using N2.Engine;
using N2.Definitions;

namespace N2.Edit.Install
{
	public partial class MediumTrust : EditPage
	{
		protected override void OnInit(EventArgs e)
		{
			Response.ContentType = "text/plain";

			base.OnInit(e);
		}

		protected override void RegisterScripts()
		{
			// do not register
		}

		protected int StartPageID
		{
			get { return Engine.Resolve<N2.Web.Site>().StartPageID; }
		}

		protected int RootItemID
		{
			get { return Engine.Resolve<N2.Web.Site>().RootItemID; }
		}

		protected IEnumerable<string> Assemblies
		{
			get
			{
				foreach (Assembly a in Engine.Resolve<ITypeFinder>().GetAssemblies())
					if(!a.FullName.StartsWith("App_") && !a.FullName.Contains("0.0.0.0"))
						yield return a.FullName.Split(',')[0];
			}
		}

		protected IEnumerable<string> ItemTypes
		{
			get
			{
				foreach (ItemDefinition definition in Engine.Definitions.GetDefinitions())
				{
					string[] parts = definition.ItemType.AssemblyQualifiedName.Split(',');
					yield return parts[0] + "," + parts[1];
				}
			}
		}

		protected IEnumerable NhProperties
		{
			get
			{
				try
				{
					IDictionary properties = Engine.Resolve<N2.Persistence.NH.IConfigurationBuilder>().BuildConfiguration().Properties;
					return properties;
				}
				catch(Exception ex)
				{
					return new DictionaryEntry[] { new DictionaryEntry("error", ex.Message) };
				}
			}
		}
	}
}
