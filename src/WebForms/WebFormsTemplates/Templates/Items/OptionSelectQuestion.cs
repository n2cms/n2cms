using System.Collections.Generic;
using N2.Templates.Details;

namespace N2.Templates.Items
{
    public abstract class OptionSelectQuestion : Question
    {
        [EditableOptions(Title="Options", SortOrder=20)]
        public virtual IList<Option> Options
        {
            get
            {
                List<Option> options = new List<Option>();
                foreach (Option o in Children.WhereAccessible())
                    options.Add(o);
                return options;
            }
        }
    }
}
