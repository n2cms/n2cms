namespace N2.Edit.Js
{
    public class jqueryUi : DirectoryCompiler
    {
        public override string FolderUrl
        {
            get { return N2.Context.Current.ManagementPaths.ResolveResourceUrl("{ManagementUrl}/Resources/Js/jquery.ui"); }
        }
    }

}
