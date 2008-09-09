using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Templates.Wiki.Fragmenters
{
    public abstract class ListFragmenter : RegexFragmenter
    {
        public ListFragmenter(string pattern)
            : base(pattern)
        {
        }

        public override void Add(Fragment fragment, IList<Fragment> fragments)
        {
            if (fragments.Count > 0)
            {
                Fragment prev = fragments[fragments.Count - 1];
                if (prev.Name == fragment.Name)
                {
                    if (prev.Value.Length < fragment.Value.Length)
                    {
                        Add(fragment, prev.ChildFragments);
                        return;
                    }
                    else if (prev.Value.Length > fragment.Value.Length)
                    {
                        for (int i = fragments.Count - 1; i >= 0; i--)
                        {
                            if (fragments[i].Name != fragment.Name)
                                break;
                            if (fragments[i].Value.Length < fragment.Value.Length)
                            {
                                base.Add(fragment, fragments[i].ChildFragments);
                            }
                        }
                    }
                }
            }

            base.Add(fragment, fragments);
        }
    }
}
