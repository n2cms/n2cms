using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace N2.Web.Targeting
{
    public abstract class DetectorBase : IComparable<DetectorBase>
    {
        public virtual string Name
        {
            get { return GetType().Name; }
        }

        public abstract bool IsTarget(TargetingContext context);

        int IComparable<DetectorBase>.CompareTo(DetectorBase other)
        {
            return Name.CompareTo(other.Name);
        }

        public virtual DetectorDescription Description
        {
            get { return new DetectorDescription { Title = Regex.Replace(Name, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ").TrimEnd(), IconClass = "fa fa-screenshot" }; }
        }

        public class DetectorDescription
        {
            public string Title { get; set; }
            public string IconClass { get; set; }
        }
    }
}
