using N2.Details;
using N2.Integrity;
using N2.Templates.Mvc.Models.Pages;

namespace N2.Templates.Mvc.Models.Parts
{
    [PartDefinition("Tracking script",
        IconUrl = "~/Content/Img/google.png",
        SortOrder = 2000)]
    [RestrictParents(typeof (LanguageRoot))]
    [AllowedZones(Zones.SiteRight)]
    [RestrictCardinality]
    public class Tracking : PartBase
    {
        [EditableCheckBox("Enabled", 100)]
        public virtual bool Enabled
        {
            get { return (bool) (GetDetail("Enabled") ?? true); }
            set { SetDetail("Enabled", value, true); }
        }

        [EditableCheckBox("Track authenticated editors", 100)]
        public virtual bool TrackEditors
        {
            get { return (bool) (GetDetail("TrackEditors") ?? false); }
            set { SetDetail("TrackEditors", value, false); }
        }

        [EditableText("UACCT code", 100)]
        public virtual string UACCT
        {
            get { return (string) (GetDetail("UACCT") ?? string.Empty); }
            set { SetDetail("UACCT", value, string.Empty); }
        }
    }
}
