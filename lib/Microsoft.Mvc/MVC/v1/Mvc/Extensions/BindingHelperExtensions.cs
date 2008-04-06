namespace System.Web.Mvc {
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reflection;
    using System.Web;

    public static class BindingHelperExtensions {
        /// <summary>
        /// Reads in values from a NameValueCollection (like Request.Form, Cookies, Session, or QueryString) and sets the properties
        /// on this object. The names of the values must be Type.PropertyName - i.e. "Product.ProductName"
        /// </summary>
        /// <param name="values">The NameValueCollection to read from (i.e. Request.Form)</param>
        /// <param name="keys">The keys to use</param>
        public static void UpdateFrom(object value, NameValueCollection values, params string[] keys) {
            if (keys.Length > 0) {
                NameValueCollection newList = new NameValueCollection();
                foreach (string key in keys) {
                    if (ContainsKey(values, key)) {
                        newList.Add(key, values[key]);
                    }
                }
                UpdateFrom(value, newList);
            }
        }

        private static bool ContainsKey(NameValueCollection collection, string keyToFind) {
            foreach (string key in collection) {
                if (key.ToUpperInvariant().Equals(keyToFind.ToUpperInvariant())) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Reads in values from a NameValueCollection (like Request.Form, Cookies, Session, or QueryString) and sets the properties
        /// on this object. The names of the values must be Type.PropertyName - i.e. "Product.ProductName"
        /// </summary>
        /// <param name="values">Request.Form, QueryString, Parameter, etc</param>
        public static void UpdateFrom(object value, NameValueCollection values) {
            UpdateFrom(value, values, "");
        }

        /// <summary>
        /// Reads in values from a NameValueCollection (like Request.Form, Cookies, Session, or QueryString) and sets the properties
        /// on this object. The names of the values must be Type.PropertyName - i.e. "Product.ProductName"
        /// </summary>
        /// <param name="values">Request.Form, QueryString, Parameter, etc</param>
        /// <param name="objectPrefix">A prefix for the Keys in the Namevalue collection - "Product." in "Product.ProductName" for example</param>
        public static void UpdateFrom(object value, NameValueCollection values, string objectPrefix) {
            Type objType = value.GetType();
            string objName = objType.Name;

            // TODO: Use ComponentModel instead of Reflection to get/set the properties
            PropertyInfo[] fields = objType.GetProperties();

            PopulateTypeException ex = null;

            foreach (PropertyInfo property in fields) {
                //check the key
                //going to be forgiving here, allowing for full declaration
                //or just propname
                string httpKey = property.Name;

                if (!String.IsNullOrEmpty(objectPrefix))
                    httpKey = objectPrefix + httpKey;

                if (values[httpKey] == null) {
                    httpKey = objName + "." + property.Name;
                }

                if (values[httpKey] == null) {
                    httpKey = objName + "_" + property.Name;
                }


                if (values[httpKey] != null) {
                    TypeConverter conv = TypeDescriptor.GetConverter(property.PropertyType);
                    object thisValue = values[httpKey];

                    if (conv.CanConvertFrom(typeof(string))) {
                        try {
                            thisValue = conv.ConvertFrom(values[httpKey]);
                            property.SetValue(value, thisValue, null);

                        }
                        catch (FormatException e) {
                            string message = property.Name + " is not a valid " + property.PropertyType.Name + "; " + e.Message;
                            if (ex == null)
                                ex = new PopulateTypeException("Errors occurred during object binding - review the LoadExceptions property of this exception for more details");

                            ExceptionInfo info = new ExceptionInfo();
                            info.AttemptedValue = thisValue;
                            info.PropertyName = property.Name;
                            info.ErrorMessage = message;

                            ex.LoadExceptions.Add(info);
                        }
                    }
                    else {
                        // TODO: Why do we throw an exception here instead of setting "ex"?
                        throw new FormatException("No type converter available for type: " + property.PropertyType);
                    }
                }
            }
            // TODO: Why does this code only throw the last exception that happened? Typically the first exception is the most important one.
            if (ex != null)
                throw ex;
        }

        /// <summary>
        /// Gets a POST/GET/QueryString value passed in
        /// </summary>
        /// <param name="key">The form/querystring parameter name</param>
        /// <returns>System.String</returns>
        public static string ReadFromRequest(this Controller controller, string key) {
            return ReadFromRequest<string>(controller, key);
        }

        /// <summary>
        /// Gets a POST/GET/QueryString value passed in and Types it
        /// </summary>
        /// <param name="key">The form/querystring parameter name</param>
        /// <returns>System.String</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Implemented in ReadFromRequest()")]
        public static T ReadFromRequest<T>(this Controller controller, string key) {
            HttpContextBase context = controller.ControllerContext.HttpContext;
            object val = null;
            T result = default(T);

            if (context != null) {

                //check POST
                if (context.Request.Form[key] != null)
                    val = context.Request.Form[key];

                if (val == null) {
                    //then QueryString
                    if (context.Request.QueryString[key] != null)
                        val = context.Request.QueryString[key];
                }
            }

            if (val != null) {
                if (typeof(T) == typeof(Boolean)) {
                    // TODO: This set of values seems arbitrary. Who decided this list?
                    //check and see if "on" or "off" is the value - if so it's a checkbox
                    if (val.ToString() == "on") {
                        val = true;
                    }
                    else if (val.ToString() == "off") {
                        val = false;
                    }
                    else if (val.ToString() == "yes") {
                        val = true;
                    }
                    else if (val.ToString() == "no") {
                        val = false;
                    }
                }

                result = (T)val;
            }
            return result;
        }
    }
}
