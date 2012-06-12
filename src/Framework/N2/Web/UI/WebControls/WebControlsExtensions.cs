﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Edit;
using System.Web.UI.WebControls;
using System.Web.UI;

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

        public static void Placeholder(this IAttributeAccessor control, string placeholder)
        {
            control.SetAttribute("placeholder", placeholder);
        }
    }
}
