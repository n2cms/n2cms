namespace N2.Addons.Wiki.UI.Views
{
    public partial class Version : WikiTemplatePage
    {
        protected Items.WikiArticle CurrentVersion
        {
            get
            {
                int versionID = int.Parse(CurrentArguments);
                return Engine.Persister.Get<Items.WikiArticle>(versionID);
            }
        }
    }
}
