using System;
using N2.Details;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Web.UI.WebControls;
using N2.Web.Wiki;

namespace N2.Addons.Wiki
{
    public class WikiTextAttribute : EditableFreeTextAreaAttribute, IDisplayable
    {
        public WikiTextAttribute(string title, int sortOrder)
            : base(title, sortOrder)
        {
        }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            base.UpdateEditor(item, editor);

            FreeTextArea fta = editor as FreeTextArea;
            IArticle article = item as IArticle;
            fta.EnableFreeTextArea = article.WikiRoot.EnableFreeText;
        }

        Control IDisplayable.AddTo(ContentItem item, string detailName, Control container)
        {
            if (!(item is IArticle)) throw new ArgumentException("The supplied item " + item.Path + "#" + item.ID + " of type '" + item.GetContentType().FullName + "' doesn't implement IArticle.", "item");

            WikiParser parser = Engine.Resolve<WikiParser>();
            WikiRenderer renderer = Engine.Resolve<WikiRenderer>();

            PlaceHolder ph = new PlaceHolder();
            container.Controls.Add(ph);

            renderer.AddTo(parser.Parse((string)item[detailName]), ph, item as IArticle);
            
            return ph;
        }
    }
}
