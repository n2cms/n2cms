using System;
using System.Collections.Generic;

namespace N2.Addons.Wiki
{
    /// <summary>
    /// Classes implementing this interface are responsible for splitting text
    /// input into fragments. These fragments can later be rendered as html by
    /// the wiki renderer.
    /// </summary>
    public interface IFragmenter
    {
        /// <summary>Analyses the text and returns fragments relevat for this fragmenter.</summary>
        /// <param name="text">The wiki text to analyse.</param>
        /// <returns>An enumeration of fragments.</returns>
        IEnumerable<Fragment> GetFragments(string text);

        /// <summary>Adds a fragment retrieved through GetFragments to the current list of fragments. This method can be used by the fragmenter for special treatment of odd fragments (such as nested lists).</summary>
        /// <param name="fragment">The fragment to add.</param>
        /// <param name="fragments">The current list of fragments.</param>
        void Add(Fragment fragment, IList<Fragment> fragments);
    }
}
