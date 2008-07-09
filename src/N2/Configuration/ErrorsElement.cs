using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
    public class ErrorsElement : ConfigurationElement
    {
        [ConfigurationProperty("action", DefaultValue = ErrorAction.None)]
        public ErrorAction Action
        {
            get { return (ErrorAction)base["action"]; }
            set { base["action"] = value; }
        }

        [ConfigurationProperty("mailTo")]
        public string MailTo
        {
            get { return (string)base["mailTo"]; }
            set { base["mailTo"] = value; }
        }

        [ConfigurationProperty("mailFrom")]
        public string MailFrom
        {
            get { return (string)base["mailFrom"]; }
            set { base["mailFrom"] = value; }
        }
    }
}
