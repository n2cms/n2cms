namespace N2.Edit.Js
{
	public class jqueryUi : DirectoryCompiler
	{
		public override string FolderUrl
		{
			get { return N2.Context.Current.EditUrlManager.ResolveManagementInterfaceUrl("|Management|/Resources/Js/jquery.ui"); }
		}
	}

}
