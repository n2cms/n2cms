using System;
using System.Web;
using System.Web.UI;
using N2.Collections;
using N2.Definitions;
using N2.Integrity;
using N2.Persistence;
using N2.Templates.Items;
using N2.Web.Parts;
using N2.Web.UI;

namespace N2.Templates.Items
{
    [PartDefinition("Poll",
        IconUrl = "~/Templates/UI/Img/chart_pie.png")]
    [AllowedZones(Zones.Left, Zones.Right, Zones.RecursiveRight, Zones.RecursiveLeft, Zones.SiteLeft, Zones.SiteRight)]
    [RestrictParents(typeof (AbstractContentPage))]
    [AllowedChildren(typeof(SingleSelect))]
    [FieldSetContainer("questionContainer", "Question", 100)]
    public class Poll : SidebarItem, IAddablePart
    {
        [Details.PollCreatorDetail(QuestionText = "Question", CreateNewText = "Create as new question", Title = "Alternatives", SortOrder = 100, ContainerName = "questionContainer")]
        public virtual SingleSelect Question
        {
            get
            {
                var children = Children.WhereAccessible();
                if (children.Count > 0)
                {
                    return children[children.Count - 1] as SingleSelect;
                }
                return null;
            }
        }

        public virtual Control AddTo(Control container)
        {
            string templateUrl = (DisplayResult(container.Page.Request))
                ? "~/Templates/UI/Parts/Result.ascx"
                : TemplateUrl;

            return N2.Web.UI.ItemUtility.AddUserControl(templateUrl, container, this);
        }

        public virtual void AddAnswer(IPersister persister, int selectedItem)
        {
            Option o = persister.Get<Option>(selectedItem);
            o.Answers++;
            persister.Save(o);
        }

        public virtual HttpCookie GetAnsweredCookie(int selectedItem)
        {
            HttpCookie c = new HttpCookie("p" + Question.ID, selectedItem.ToString());
            c.Expires = N2.Utility.CurrentTime().AddMonths(1);
            return c;
        }

        private bool DisplayResult(HttpRequest request)
        {
            return Question != null && request["p" + Question.ID] != null;
        }

        protected override string TemplateName
        {
            get { return "Poll"; }
        }
    }
}
