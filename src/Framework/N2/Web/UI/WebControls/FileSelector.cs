using N2.Resources;
using N2.Definitions;

namespace N2.Web.UI.WebControls
{
    /// <summary>An input box that can be updated with the url to a file through a popup window.</summary>
    public class FileSelector : UrlSelector
    {
        public FileSelector()
        {
            this.CssClass = "fileSelector selector";
            this.DefaultMode = UrlSelectorMode.Files;
            this.AvailableModes = UrlSelectorMode.Files;
            this.SelectableTypes = typeof(IFileSystemFile).Name;
            BrowserUrl = N2.Web.Url.Parse(Engine.ManagementPaths.EditTreeUrl).AppendQuery("location=filesselection");
        }

        protected override void RegisterClientScripts()
        {
            Page.JavaScript("$('#" + ClientID + "').n2autocomplete({ filter: 'io', selectableTypes:'" + SelectableTypes + "', selectableExtensions:'" + SelectableExtensions + "' });", ScriptPosition.Bottom, ScriptOptions.DocumentReady | ScriptOptions.ScriptTags);
        }

        public static string ImageExtensions = ".bmp,.gif,.png,.jpg,.jpeg";
        public static string MovieExtensions = ".swf,.mpg,.mpeg,.mp4,.avi,.wmv";
        public static string AudioExtensions = ".aif,.m4a,.mid,.mp3,.wav,.wma";
    }
}
