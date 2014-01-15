using N2.Details;
using N2.Integrity;
using System.Web.UI;

namespace N2.Templates.Items
{
    [WithEditableTitle("Question", 10, Focus = false)]
    [RestrictParents(typeof(ISurvey))]
    [AllowedZones("Questions", "")]
    public abstract class Question : AbstractItem
    {
        [EditableCheckBox("Required", 120)]
        public virtual bool Required { get; set; }
    }
}
