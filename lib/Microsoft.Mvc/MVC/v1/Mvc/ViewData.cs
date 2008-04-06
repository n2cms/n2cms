namespace System.Web.Mvc {
    using System.Collections;
    using System.Web.UI;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class ViewData {
        private object _data;

        public ViewData(object data) {
            _data = data;
        }

        public object this[string dataItem] {
            get {
                IDictionary dictionary = _data as IDictionary;
                if (dictionary != null) {
                    return dictionary[dataItem];
                }
                else {
                    return DataBinder.Eval(_data, dataItem);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "There is no more specific exception that we can catch.")]
        public bool ContainsDataItem(string dataItem) {
            IDictionary dictionary = _data as IDictionary;
            bool exists = true;
            if (dictionary != null) {
                exists = dictionary.Contains(dataItem);
            }
            else {
                try {
                    DataBinder.Eval(_data, dataItem);
                }
                catch (Exception) {
                    exists = false;
                }
            }
            return exists;
        }
    }
}
