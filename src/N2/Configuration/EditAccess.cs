using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using System.Collections.Specialized;

namespace N2.Configuration
{
    public class EditAccess : ConfigurationElement
    {
        [ConfigurationProperty("users"), TypeConverter(typeof(CommaDelimitedStringCollectionConverter))]
        public StringCollection Users
        {
            get { return (CommaDelimitedStringCollection)base["users"]; }
        }

        [ConfigurationProperty("roles"), TypeConverter(typeof(CommaDelimitedStringCollectionConverter))]
        public StringCollection Roles 
        {
            get { return (CommaDelimitedStringCollection)base["roles"]; }
        }
    }
}
