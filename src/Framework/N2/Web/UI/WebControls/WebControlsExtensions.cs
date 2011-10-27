using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Edit.Workflow;

namespace N2.Web.UI.WebControls
{
    public static class WebControlsExtensions
    {
        public static bool IsManaging(this ControlPanelState state)
        {
            switch (state)
            {
                case ControlPanelState.DragDrop:
                case ControlPanelState.Editing:
                case ControlPanelState.Previewing:
                    return true;
                default:
                    return false;
            }
        }

        public static string GetInterface(this ControlPanelState state)
        {
            return state.IsManaging() ? Interfaces.Managing : Interfaces.Viewing;
        }
    }
}
