namespace N2.Edit.Js
{
	public class plugins : DirectoryCompiler
	{
		public override string FolderUrl
		{
			get { return N2.Context.Current.EditManager.ResolveManagementInterfaceUrl("|Management|/Resources/Js/plugins"); }
		}
	}
}
