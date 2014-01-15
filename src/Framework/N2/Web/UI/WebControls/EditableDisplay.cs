using System;
using System.Web.UI;
using N2.Definitions;
using N2.Security;

namespace N2.Web.UI.WebControls
{
    /// <summary>
    /// This control will either display content using the defined editor 
    /// just as the <see cref="Display"/> does or edit using the defined 
    /// editor. The GetState method on the <see cref="ControlPanel"/> determines 
    /// wether the content should be displayed or edited.
    /// </summary>
    public class EditableDisplay : Display, IEditableEditor
    {
        private IEditable editable;
        private Control editor;
        private ControlPanelState state;

        public Control Editor
        {
            get
            {
                EnsureChildControls();
                return editor;
            }
            set { editor = value; }
        }

        public ControlPanelState State
        {
            get { return state; }
            set { state = value; }
        } 

        protected override void OnInit(EventArgs e)
        {
            State = ControlPanel.GetState(Page.GetEngine());
            if (State.IsFlagSet(ControlPanelState.Editing))
            {
                AddEditable();
            }
            else
            {
                AddDisplayable();
            }
        }

        protected void AddEditable()
        {
            editable = GetEditable(CurrentItem);
            editor = editable.AddTo(this);
            if (!Page.IsPostBack)
                editable.UpdateEditor(CurrentItem, editor);
        }

        private IEditable GetEditable(ContentItem item)
        {
            foreach (IEditable editable in N2.Context.Definitions.GetDefinition(item).Editables)
                if (editable.Name == PropertyName)
                    if (N2.Context.Current.SecurityManager.IsAuthorized(editable, Page.User, CurrentItem))
                        return editable;
            return null;
        }
    }
}
