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
	/// <summary>
	/// Attribute to allow the user to localize the contents,
	/// 
	/// The only external reference is that on language change, this method will be called
	/// LanguageSwitcherModule.changeEditLanguage(ddl.SelectedValue);
	/// will be called.
	/// 
	/// 
	///

	public class LanguageSwitcherAttribute : N2.Details.AbstractEditableAttribute
	{


		/**
		 * Get the list of languages available.
		 * the first can be considered the default language.
		 * The content in the default language can be delivered when the 
		 * content requested doesn't exist in the language selected
		 * */

		public static string[] getLanguageList()
		{
			return new string[] { "ENG", "FRE" };
		}

		public static string getDefaultLanguage()
		{
			return getLanguageList()[0];
		}






		public LanguageSwitcherAttribute(string title)
		{
			this.Title = title;
			this.Name = "CurrentLanguage";
		}

		/**
		 * Event handler 4 handling the change in the selectbox
		 **/
		public override void UpdateEditor(N2.ContentItem item, Control editor)
		{
			DropDownList ddl = (DropDownList)editor;

			ddl.SelectedValue = LanguageSwitcherModule.RequestLanguage;
		}

		public override bool UpdateItem(N2.ContentItem item, Control editor)
		{
			return false;
		}

		/**
		 * This method is called when the selectbox 4 the languages is being inserted in the page
		 * 
		 * */

        protected override Control AddEditor(Control container)
        {
            DropDownList ddl = new DropDownList();
            foreach (string lang in getLanguageList())
                ddl.Items.Add(lang);
            ddl.AutoPostBack = true;
            ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
            container.Controls.Add(ddl);
            return ddl;
        }

		/**
		 * Invoked when the language is changed
		 * */
		void ddl_SelectedIndexChanged(object sender, EventArgs e)
		{
			DropDownList ddl = (DropDownList)sender;
			LanguageSwitcherModule.changeEditLanguage(ddl.SelectedValue);

		}


	}
}
