namespace N2.Edit.Js
{
	public class jqueryUi : DirectoryCompiler
	{
		public override string FolderUrl
		{
			get { return N2.Context.Current.EditManager.ResolveManagementInterfaceUrl("|Management|/Resources/Js/jquery.ui"); }
		}
	}

}
