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
	/// This module will handle language information by means of cookies
	/// 
	/// to add this module simply add
	///   <httpModules>
	///     <add name="LanguageSwitchingModule" type="LanguageSwitcherCookieManaged" />
	///   </httpModules>
	///   to your web.config.
	///   
	/// check LanguageSwitcherModule.cs documentation for additional information

	public class LanguageSwitcherCookieManaged : LanguageSwitcherModule
	{
		public LanguageSwitcherCookieManaged()
			: base()
		{

		}



		/**
	   * Get the name of the cookie to store language information
	   * */
		public static string getCookieName()
		{
			return "languagecookie";
		}



		/**
		   * method to get the current language 4 browsing the pages
		   * */

		protected override string getPersistentLanguage()
		{
			System.Web.HttpCookie updatedlanguagecookie = System.Web.HttpContext.Current.Response.Cookies[getCookieName()];
			if (updatedlanguagecookie != null && updatedlanguagecookie.Value != null && updatedlanguagecookie.Value != "")
				return updatedlanguagecookie.Value;
			else
				//Checking if a response cookie exist will implicitly add it to the response with a null value if it doesn't exist.So we need to remove it.
				System.Web.HttpContext.Current.Response.Cookies.Remove(getCookieName());

			System.Web.HttpCookie originalanguagecookie = System.Web.HttpContext.Current.Request.Cookies.Get(getCookieName());
			if (originalanguagecookie != null && originalanguagecookie.Value != null && originalanguagecookie.Value != "")
				return originalanguagecookie.Value;

			//No language defined
			return LanguageSwitcherAttribute.getDefaultLanguage();
		}
		/**
		 * method to set/initialize the current language 4 browsing the page
		 * 
		 * */
		protected override void setPersistentLanguage(string language)
		{
			System.Web.HttpCookie newlanguagecookie = new System.Web.HttpCookie(getCookieName(), language);
			System.Web.HttpContext.Current.Response.AppendCookie(newlanguagecookie);
		}
		/**
		 * method to determine if the information about current language already exist or need to be initialized
		 * */
		protected override bool existPersistentLanguage()
		{
			if (System.Web.HttpContext.Current.Request.Cookies.Get(getCookieName()) != null
				 && System.Web.HttpContext.Current.Request.Cookies.Get(getCookieName()).Value != null
				&& System.Web.HttpContext.Current.Request.Cookies.Get(getCookieName()).Value != "")
				return true;
			else
				return false;
		}


		protected override bool isLanguageInformationNeeded()
		{
			return true;
		}

	}
}