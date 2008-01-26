using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace LanguageSwitcher
{
	/// <author> Michele Scaramal</author>
	/// <summary>
	/// Summary description for LanguageSwitcherSessionManaged
	/// 
	/// 
	/// This module will handle the language information by means of session state
	/// 
	/// /// 
	/// to add this module simply add
	///   <httpModules>
	///     <add name="LanguageSwitchingModule" type="LanguageSwitcherSessionManaged" />
	///   </httpModules>
	///   to your web.config.
	/// 
	/// You have to add a sessionstate line in your web config....for example:
	/// <system.web>
	///    <sessionState mode="InProc" />
	/// </summary>
	/// 
	/// read the additional documentation in languageswitchermodule.cs

	public class LanguageSwitcherSessionManaged : LanguageSwitcherModule
	{
		public LanguageSwitcherSessionManaged()
			: base()
		{

		}


		/**
	* Get the name of the cookie to store language information
	* */
		public static string getLanguageKeyName()
		{
			return "languagecookie";
		}




		protected override string getPersistentLanguage()
		{
			if (HttpContext.Current.Session[getLanguageKeyName()] == null)
				return LanguageSwitcherAttribute.getDefaultLanguage();
			else
				return (string)HttpContext.Current.Session[getLanguageKeyName()];
		}

		protected override void setPersistentLanguage(string language)
		{
			HttpContext.Current.Session[getLanguageKeyName()] = language;

		}

		protected override bool existPersistentLanguage()
		{
			if (HttpContext.Current.Session[getLanguageKeyName()] == null) return false;
			else return true;

		}


		protected override bool isLanguageInformationNeeded()
		{
			//Session=null it means that the session module hasn't initialized the session,
			//so the content requestet must be static and won't need language information
			return (HttpContext.Current.Session != null);
		}
	}
}