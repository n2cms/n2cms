using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace N2.Templates.Wiki.Fragmenters
{
    /// <summary>
    /// Base class implementing some common functionality for fragmenters.
    /// </summary>
    public abstract class AbstractFragmenter : IFragmenter
    {
        protected virtual string Name
        {
            get { return GetType().Name.Replace("Fragmenter", ""); }
        }
        protected Regex Expression { get; set; }

        public virtual IEnumerable<Fragment> GetFragments(string text)
        {
            MatchCollection matches = Expression.Matches(text);
            foreach (Match m in matches)
            {
                if (!m.Success && m.Length > 0)
                    continue;

                yield return CreateFragment(m);
            }
        }

        protected virtual Fragment CreateFragment(Match m)
        {
            Fragment f = new Fragment
            {
                Name = this.Name,
                Value = m.Value,
                StartIndex = m.Index,
                Length = m.Length
            };
            return f;
        }

        protected Regex CreateExpression(string pattern)
        {
            return new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
        }
    }
}
