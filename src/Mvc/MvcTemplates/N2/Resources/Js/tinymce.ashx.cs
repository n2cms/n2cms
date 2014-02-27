namespace N2.Edit.Js
{
    public class tinymce : DirectoryCompiler
    {
        public override string FolderUrl
        {
            get { return N2.Context.Current.ManagementPaths.ResolveResourceUrl("{ManagementUrl}/Resources/tiny_mce"); }
        }
    }
}
