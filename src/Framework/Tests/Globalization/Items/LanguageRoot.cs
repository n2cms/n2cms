using N2.Details;
using N2.Engine.Globalization;
using N2.Web.UI;

namespace N2.Tests.Globalization.Items
{
    [FieldSetContainer("globalization", "Globalization", 200)]
    public class LanguageRoot : ContentItem, ILanguage
    {
        [EditableText("LanguageTitle", 110, ContainerName = "globalization")]
        public virtual string LanguageTitle
        {
            get { return (string)(GetDetail("LanguageTitle") ?? string.Empty); }
            set { SetDetail("LanguageTitle", value, string.Empty); }
        }

        [EditableText("LanguageCode", 120, ContainerName = "globalization")]
        public virtual string LanguageCode
        {
            get { return (string)(GetDetail("LanguageCode") ?? string.Empty); }
            set { SetDetail("LanguageCode", value, string.Empty); }
        }
    }
}
