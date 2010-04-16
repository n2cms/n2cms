using System;
using System.Linq;
using N2.Edit.Web;
using N2.Collections;
using N2.Edit.FileSystem.Items;
using N2.Resources;
using System.Collections.Generic;

namespace N2.Edit.FileSystem
{
	public partial class Directory1 : EditPage
	{
		protected override void RegisterToolbarSelection()
		{
			string script = GetToolbarSelectScript("preview");
			Register.JavaScript(this, script, ScriptPosition.Bottom, ScriptOptions.ScriptTags);
		}

		protected IEnumerable<ContentItem> ancestors;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			hlNewFile.NavigateUrl = Engine.EditManager.GetEditNewPageUrl(Selection.SelectedItem, Engine.Definitions.GetDefinition(typeof(File)), null, CreationPosition.Below);

			ancestors = Find.EnumerateParents(Selection.SelectedItem, null, true).Where(a => a is AbstractNode).Reverse();

			var dir = Selection.SelectedItem as Directory;
			var directories = dir.GetDirectories();
			var files = dir.GetFiles();

			rptDirectories.DataSource = directories;
			rptFiles.DataSource = files;
			DataBind();

			Refresh(Selection.SelectedItem, ToolbarArea.Navigation);
		}
	}
}
