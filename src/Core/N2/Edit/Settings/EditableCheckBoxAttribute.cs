using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Engine;

namespace N2.Edit.Settings
{
	public class EditableCheckBoxAttribute : AbstractEditableAttribute
	{
		public EditableCheckBoxAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
		}

		public override void UpdateService(IEngine engine, Control editor)
		{
			object item = engine.Resolve(ServiceName);
			CheckBox cb = editor as CheckBox;
			PropertyInfo pi = item.GetType().GetProperty(Name);
			pi.SetValue(item, cb.Checked, null);
		}

		public override void UpdateEditor(IEngine engine, Control editor)
		{
			object item = engine.Resolve(ServiceName);
			CheckBox cb = editor as CheckBox;
			PropertyInfo pi = item.GetType().GetProperty(Name);
			cb.Checked = (bool) pi.GetValue(item, null);
		}

		public override Control AddTo(Control container)
		{
			CheckBox cb = new CheckBox();
			cb.Text = Title;
			container.Controls.Add(cb);
			return cb;
		}
	}
}