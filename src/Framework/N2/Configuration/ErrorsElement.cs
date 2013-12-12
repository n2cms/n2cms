using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// Configuration related to errors handling and reporting.
    /// </summary>
    public class ErrorsElement : ConfigurationElement
    {
        [ConfigurationProperty("action", DefaultValue = ErrorAction.None)]
        public ErrorAction Action
        {
            get { return (ErrorAction)base["action"]; }
            set { base["action"] = value; }
        }

        /// <summary>A negative value is treated as unlimited number of errors.</summary>
        [ConfigurationProperty("handleWrongClassException", DefaultValue = true)]
        public bool HandleWrongClassException
        {
            get { return (bool)base["handleWrongClassException"]; }
            set { base["handleWrongClassException"] = value; }
        }

        /// <summary>A negative value is treated as unlimited number of errors.</summary>
        [ConfigurationProperty("maxErrorReportsPerHour", DefaultValue = 60)]
        public int MaxErrorReportsPerHour
        {
            get { return (int)base["maxErrorReportsPerHour"]; }
            set { base["maxErrorReportsPerHour"] = value; }
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

        [ConfigurationProperty("sqlExceptionHandling", DefaultValue = ExceptionResolutionMode.RefreshGet)]
        public ExceptionResolutionMode SqlExceptionHandling
        {
            get { return (ExceptionResolutionMode)base["sqlExceptionHandling"]; }
            set { base["sqlExceptionHandling"] = value; }
        }
    }
}
