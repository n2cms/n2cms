using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Engine;
using N2.Plugin;
using N2.Definitions;
using N2.Details;

namespace N2.Templates.Services
{
     [Service]
    public class QuestionRequiredDefinitionAppender: IAutoStart
    {

        private readonly IDefinitionManager definitions;
        private string checkBoxText = "Required";
        private int _sortOrder = 30;
        public static readonly string RequiredDetailName = "Required";

        public QuestionRequiredDefinitionAppender(IDefinitionManager definitions)
        {
            this.definitions = definitions;
        }

        public int SortOrder
        {
            get { return _sortOrder; }
        }

        public string CheckBoxText
        {
            get { return checkBoxText; }
            set { checkBoxText = value; }
        }


        void IAutoStart.Start()
        {
            foreach (ItemDefinition definition in definitions.GetDefinitions())
            {

                if (typeof(N2.Templates.Items.Question).IsAssignableFrom(definition.ItemType))
                {
                    EditableCheckBoxAttribute ecb = new EditableCheckBoxAttribute(CheckBoxText, 10);
                    ecb.Name = RequiredDetailName;
                    ecb.SortOrder = SortOrder;

                    definition.Add(ecb);
                }
            }
        }

        void IAutoStart.Stop()
        {
          //not needed
        }
    }
}