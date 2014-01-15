using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Addons.Wiki
{
    /// <summary>
    /// A recognized chunk of content that needs to be processed by the 
    /// renderer.
    /// </summary>
    public class Fragment
    {
        public Fragment()
        {
            ChildFragments = new List<Fragment>();
        }

        /// <summary>The type of fragment.</summary>
        public string Name { get; set; }
        /// <summary>The value of the fragment. The meaning of this string can vary depending on the the type of fragment.</summary>
        public string Value { get; set; }
        public string InnerContents { get; set; }
        public int StartIndex { get; set; }
        public int Length { get; set; }
        public Fragment Previous { get; set; }
        public Fragment Next { get; set; }
        public IList<Fragment> ChildFragments { get; set; }

        public override string ToString()
        {
            return base.ToString() + " [" + Name + "]";
        }
    }
}
