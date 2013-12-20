using System;
using System.Linq;

namespace N2.Templates.Mvc.Models.Parts.Questions
{
    [PartDefinition("Single Select (radio buttons)")]
    public class SingleSelect : OptionSelectQuestion
    {
        public override string ElementID
        {
            get { return "ss_" + ID; }
        }

        public override string GetAnswerText(string value)
        {
            var selectedOption = Options.FirstOrDefault(opt => opt.ID.ToString() == value);

            if (selectedOption == null)
                return String.Empty;

            return selectedOption.Title;
        }

        protected override string InputType
        {
            get { return "radio"; }
        }
    }
}
