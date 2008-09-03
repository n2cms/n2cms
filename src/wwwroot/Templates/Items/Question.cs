using N2.Details;
using N2.Integrity;

namespace N2.Templates.Items
{
    [WithEditableTitle("Question", 10)]
    [RestrictParents(typeof(ISurvey))]
    [AllowedZones("Questions", "")]
    public abstract class Question : AbstractItem
    {
    }
}