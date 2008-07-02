using System;
using System.Collections.Generic;

namespace N2.Templates.Wiki
{
    /// <summary>
    /// Classes implementing this interface are responsible for splitting text
    /// input into fragments. These fragments can later be rendered as html by
    /// the wiki renderer.
    /// </summary>
    public interface IFragmenter
    {
        IEnumerable<Fragment> GetFragments(string text);
    }
}
