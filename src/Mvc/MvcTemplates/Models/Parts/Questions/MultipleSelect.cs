using System;
using System.Linq;

namespace N2.Templates.Mvc.Models.Parts.Questions
{
    [PartDefinition("Multiple Select (check boxes)")]
    public class MultipleSelect : OptionSelectQuestion
    {
        public override string ElementID
        {
            get { return "ms_" + ID; }
        }

        public override string GetAnswerText(string value)
        {
            var values = (value ?? String.Empty).Split(',');

            var selectedOptions = Options.Where(opt => values.Contains(opt.ID.ToString())).Select(opt => opt.Title).ToArray();

            if (selectedOptions.Length == 0)
                return String.Empty;

            return String.Join(", ", selectedOptions);
        }

        protected override string InputType
        {
            get { return "checkbox"; }
        }
    }
}
