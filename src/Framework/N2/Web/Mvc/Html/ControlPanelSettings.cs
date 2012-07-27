using System;

namespace N2.Web.Mvc.Html
{
    public class ControlPanelSettings
    {
        public bool IncludeJQuery { get; set; }

        public bool IncludeJQueryUI { get; set; }

        public bool IncludeJQueryPlugins { get; set; }

        public bool IncludePartScripts { get; set; }

        public bool IncludePartStyles { get; set; }

        /// <summary>
        /// Is used to instruct the control panel helper not to refresh navigation to the current page.
        /// </summary>
        public bool RefreshNavigation { get; set; }

        public ControlPanelSettings()
        {
            IncludeJQuery = true;
            IncludeJQueryUI = true;
            IncludeJQueryPlugins = true;
            IncludePartScripts = true;
            IncludePartStyles = true;
            RefreshNavigation = true;
        }
    }
}