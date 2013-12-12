using System;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using N2.Definitions;
using N2.Engine;

namespace N2.Details
{
    [DebuggerDisplay("{name, nq} [{TypeName, nq}]")]
    public abstract class AbstractDisplayableAttribute : Attribute, IDisplayable, IComparable<IUniquelyNamed>
    {
        private string cssClass = null;
        private string name;
        int? hashCode;
        IEngine engine;
        
        public string CssClass
        {
            get { return cssClass; }
            set { cssClass = value; }
        }

        public IEngine Engine
        {
            get { return engine ?? (engine = Context.Current); }
            set { engine = value; }
        }

        #region IDisplayable Members
        /// <summary>Gets or sets the name of the detail (property) on the content item's object.</summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual Control AddTo(ContentItem item, string detailName, Control container)
        {
            using (var sw = new StringWriter())
            {
                Write(item, detailName, sw);
                string html = sw.ToString();
                if (string.IsNullOrEmpty(html))
                    return null;

                var lc = new LiteralControl(html);
                container.Controls.Add(lc);
                return lc;
            }
        }
        #endregion

        #region IWritingDisplayable Members

        public virtual void Write(ContentItem item, string detailName, System.IO.TextWriter writer)
        {
            writer.Write("[Override AddTo or Write]");
        }

        #endregion

        #region Equals & GetHashCode
        /// <summary>Checks another object for equality.</summary>
        /// <param name="obj">The other object to check.</param>
        /// <returns>True if the items are of the same type and have the same name.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as AbstractDisplayableAttribute;
            if (other == null)
                return false;
            return name == other.Name;
        }

        /// <summary>Gets a hash code based on the attribute's name.</summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return hashCode ?? (hashCode = (name == null) ? base.GetHashCode() : (GetType().FullName.GetHashCode() + name.GetHashCode())).Value;
        }

        private string TypeName
        {
            get { return GetType().Name; }
        }
        #endregion

        #region IComparable<IUniquelyNamed> Members

        int IComparable<IUniquelyNamed>.CompareTo(IUniquelyNamed other)
        {
            var containable = other as IContainable;
            if (containable != null)
                return 1;

            if (other is IDisplayable)
                return 0;
            if (other == null)
                return 1;

            return Name.CompareTo(other.Name);
        }

        #endregion
    }
}
