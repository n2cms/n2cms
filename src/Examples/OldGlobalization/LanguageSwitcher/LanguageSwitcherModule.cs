using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace LanguageSwitcher
{
	/// <author> Michele Scaramal</author>
	/// <summary>
	/// Module to handle language internationalization 
	/// 
	/// BASIC FUNCTIONALITY:
	/// Each time a request is received by the server this module will do the following:
	/// 1- decide which language has to be served 
	/// 2- expose the language information in the public property requestlanguage. 
	/// (Aspx pages then can know which language was requested by the client reading that property.)
	/// 
	/// How the property is set:
	/// 1- check if the language information is needed 4 the request
	///     if yes{
	///             If we are in edit mode  we read the information in a parameter in the request 
	///             or if this information doesn't  exist we expose the default language value.
	///             If we are in browsing mode we expose the value read on a persistent structure(cookie/session/other)  
	///             if persistent information about language exists, otherwise we expose the default language.
	///     }
	///     else{ do nothing!}
	///
	/// 
	/// 
	/// HOW TO SET UP THE MODULE
	/// This implementation of this module is to be added to the web config
	///   <httpModules>
	///     <add name="LanguageSwitchingModule" type="LanguageSwitcherModuleImplementation" />
	///   </httpModules>
	///  to allow the LanguageSwitcherAttribute to manage the request language.
	/// </summary>
	///  You will need to set also this app setting on the web config of your edit directory
	///  to allow the module to discover if the content is requested for editing purposes 
	///  rather than for showing purposes
	///  <appSettings>
	///        <add key="editmode" value="true" />
	///  </appSettings>
	/// 
	/// 
	/// 
	/// HOW TO IMPLEMENT A REALIZATION OF THIS CLASS:
	///  
	/// 
	///  1) the method isLanguageInformationNeeded() will be called once 4 every request.
	///     It should check if the language information is needed by the request.
	///     If it returns false the module won't execute any other instruction
	///  2) the method existPersistentLanguage() should check if the application has information about
	///     the persistent language 4 browsing.
	///  3) the method getPersistentLanguage() should return the information about
	///     the persistent language 4 browsing. if it exist.
	///  4) the method setPersistentLanguage() should set the new information about the persistent
	///     language. Further calls to getPersistentLanguage() in the same request or in further requests
	///     should return the new information.
	/// 









	public abstract class LanguageSwitcherModule : IHttpModule
	{
		public LanguageSwitcherModule() { }


		#region IHttpModule Members

		void IHttpModule.Dispose()
		{

		}

		void IHttpModule.Init(HttpApplication context)
		{
			//Add the languageattribute event handler to the list of events fired on the issuing of a new request.
			context.PreRequestHandlerExecute += new EventHandler(OnRequestLanguageHandling);


		}

		#endregion






		/**
    * Get the name of the parameted used to switch language in browsing mode
    * */
		public static string getParameter4Switching()
		{
			return "language4switching";
		}


		/**
		* Get the name of the parameter used to specify the language in edit mode
		* */
		public static string getParameter4Editing()
		{
			return "language4editing";
		}






		#region to handle persistent information about languages

		/**
     * method to get the current language 4 browsing the pages
     * */
		protected abstract string getPersistentLanguage();

		/**
		 * method to set/initialize the current language 4 browsing the page
		 * 
		 * */
		protected abstract void setPersistentLanguage(string language);

		/**
		 * method to determine if the information about current language already exist or need to be initialized
		 * */
		protected abstract bool existPersistentLanguage();

		/**
		 * Method to decide if the language information are needed for this request
		 * */
		protected abstract bool isLanguageInformationNeeded();



		#endregion

		#region managing the request language

		/**
     * Managing the persistent information for the language switching (see LanguageSwitcherModule(
     * logic:
     * 1) check if languageinformation is needed(for example it's not need if we are requesting a static resource)
     * 
     * 2a) if the languageswitching attribute is provided in the request, then set a new language 4 browsing
     * 2b)if the languageswitching attribute is not provided and no language is defined, initialize the default language
     * 
     * 3) Set the request language property. 
     * */
		public void OnRequestLanguageHandling(object sender, EventArgs a)
		{
			if (this.isLanguageInformationNeeded())
			{

				//SET THE NEW PERSISTENT LANGUAGE IF IT'S THE FIRST REQUEST OR THE LANGUAGE HAS CHANGED

				if (System.Web.HttpContext.Current.Request.QueryString[getParameter4Switching()] != null)
					this.setPersistentLanguage(System.Web.HttpContext.Current.Request.QueryString[getParameter4Switching()]);
				else
					if (!this.existPersistentLanguage())
						this.setPersistentLanguage(LanguageSwitcherAttribute.getDefaultLanguage());

				// SET THE REQUEST LANGUAGE


				if (isEditMode())
					//if we are in edit mode returng the language for editing
					RequestLanguage = getEditLanguage();
				else
					//if we are in edit mode returng the content for browsing
					RequestLanguage = this.getPersistentLanguage();
			}

		}

		#endregion
		/**
     * Discover if we are in edit mode
     * 
     * */
		public static bool isEditMode()
		{
			return "true".Equals(System.Configuration.ConfigurationManager.AppSettings["editmode"]);
		}
		/**
		 * If the editing language is supplied return that language, otherwise the default language
		 * */
		public static string getEditLanguage()
		{
			return System.Web.HttpContext.Current.Request.QueryString[getParameter4Editing()] ?? LanguageSwitcherAttribute.getDefaultLanguage();
		}



		/**
		 * Facility to change edit language
		 * This will redirect the browser to the same address it has been last requested while specifying the new language for editing.
		 * this method has to be called only as an effect of a new language in the editing mode's language selectbox
		**/
		public static void changeEditLanguage(string language)
		{
			string url = System.Web.HttpContext.Current.Request.RawUrl;

			System.Text.RegularExpressions.Regex oReg;
			oReg = new Regex("([\\&\\?])(" + getParameter4Editing() + "=[^\\&]*)((&.*$)|$)");

			for (; oReg.Match(url).Success; )
			{
				url = oReg.Replace(url, "$1$4");
			}

			url += (url.Contains("?") ? "&" : "?") + getParameter4Editing() + "=" + language;
			System.Web.HttpContext.Current.Response.Redirect(url);
		}

		/**
		 * Facility to decorate the url for a page with the parameter to change the persistent language
		**/
		public static string decorateUrlForPersistentLanguageChange(string url, string newlanguage)
		{


			System.Text.RegularExpressions.Regex oReg;
			oReg = new Regex("([\\&\\?])(" + getParameter4Switching() + "=[^\\&]*)((&.*$)|$)");

			for (; oReg.Match(url).Success; )
			{
				url = oReg.Replace(url, "$1$4");
			}
			url += (url.Contains("?") ? "&" : "?") + getParameter4Switching() + "=" + newlanguage;
			return url;

		}


		/**
		  * 
		  * THIS PROPERTY NEEDS TO BE SET 4 EVERY REQUEST.
		  * 
		* */

		public static string RequestLanguage
		{
			//The property can be read safely ony after it has been set for the current request.
			get { return (string)HttpContext.Current.Items[getPersistentLanguageParameterName()]; }
			//The property needs to be set at the beginning of every request to set the language 
			set { HttpContext.Current.Items[getPersistentLanguageParameterName()] = value; }
		}

		/**
		* Name of the key for the LANGUAGE INFORMATION in the HTTPContext
		* 
		* */
		private static string getPersistentLanguageParameterName()
		{
			return "requestlanguage";
		}
	}
}