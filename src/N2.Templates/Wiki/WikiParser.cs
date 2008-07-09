using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using N2.Templates.Wiki.Fragmenters;

namespace N2.Templates.Wiki
{
    public class WikiParser
    {
        IList<IFragmenter> fragmenters = new List<IFragmenter>();

        public WikiParser()
        {
            fragmenters.Add(new CommentFragmenter());
            fragmenters.Add(new UserInfoFragmenter());
            fragmenters.Add(new InternalLinkFragmenter());
            fragmenters.Add(new ExternalLinkFragmenter());
            fragmenters.Add(new HeadingFragmenter());
            fragmenters.Add(new TemplateFragmenter());
            fragmenters.Add(new TextFragmenter());
        }

        public void Add(AbstractFragmenter fragment)
        {
            fragmenters.Add(fragment);
        }

        public IEnumerable<Fragment> Parse(string text)
        {
            var list = new List<Fragment>();
            Fragment(text, 0, list);
            return list;
        }

        private void Fragment(string text, int fragmenterIndex, IList<Fragment> fragments)
        {
            if (fragmenterIndex >= fragmenters.Count || string.IsNullOrEmpty(text))
                return;

            IFragmenter fragmenter = fragmenters[fragmenterIndex];
            int index = 0;
            foreach (Fragment f in fragmenter.GetFragments(text))
            {
                if (f.StartIndex > index)
                {
                    Fragment(text.Substring(index, f.StartIndex - index), fragmenterIndex + 1, fragments);
                }
                fragments.Add(f);
                index = f.StartIndex + f.Length;
            }
            if (index < text.Length)
                Fragment(text.Substring(index), fragmenterIndex + 1, fragments);
        }

        //protected virtual Fragmenter CreateFragment(string value)
        //{
        //    string key = string.Empty;
        //    int colonIndex = value.IndexOf(':');
        //    if (colonIndex > 0)
        //    {
        //        key = value.Substring(0, colonIndex);
        //    }
        //    if (map.ContainsKey(key))
        //        return map[key].Construct();
        //    else
        //        return new TextFragment(value);
        //}
    }
}
