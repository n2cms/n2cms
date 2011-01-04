using System;
using System.Web;

namespace N2.Edit
{
	public class PersonalSettings
	{
		public static bool DisplayDataItems
		{
			get { return Context.Current.Resolve<Settings.NavigationSettings>().DisplayDataItems; }
		}
	}
}
