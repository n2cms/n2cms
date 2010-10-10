using System;
using System.Collections.Generic;
using System.Text;
using N2.Details;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Addons.Wiki.Fragmenters;
using N2.Web.UI.WebControls;
using N2.Templates.Configuration;
using System.Web.Configuration;

namespace N2.Addons.Wiki
{
    public class WikiTextAttribute : EditableFreeTextAreaAttribute, IDisplayable
    {
        public WikiTextAttribute(string title, int sortOrder)
            : base(title, sortOrder)
        {
        }

        bool? freeText = null;

        protected override TextBox CreateEditor()
        {
            if (freeText == null)
            {
                TemplatesSection config = WebConfigurationManager.GetSection("n2/templates") as TemplatesSection;
                if (config != null)
                {
                    freeText = config.Wiki.FreeTextMode;
                }
                else
                {
                    freeText = false;
                }
            }
            FreeTextArea fta = base.CreateEditor() as FreeTextArea;
            fta.EnableFreeTextArea = freeText.Value;
            return fta;
        }

        Control IDisplayable.AddTo(ContentItem item, string detailName, Control container)
        {
            if (!(item is IArticle)) throw new ArgumentException("The supplied item " + item.Path + "#" + item.ID + " of type '" + item.GetContentType().FullName + "' doesn't implement IArticle.", "item");

            WikiParser parser = Engine.Resolve<WikiParser>();
            WikiRenderer renderer = Engine.Resolve<WikiRenderer>();

            PlaceHolder ph = new PlaceHolder();
            container.Controls.Add(ph);

            renderer.AddTo(parser.Parse((string)item[detailName]), ph, item);
            
            return ph;
        }
    }
}
