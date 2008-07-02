using System;
using System.Collections.Generic;
using System.Text;
using N2.Details;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Templates.Wiki.Fragmenters;

namespace N2.Templates.Wiki
{
    public class WikiTextAttribute : EditableFreeTextAreaAttribute, IDisplayable
    {
        public WikiTextAttribute(string title, int sortOrder)
            : base(title, sortOrder)
        {
        }

        Control IDisplayable.AddTo(ContentItem item, string detailName, Control container)
        {
            if (!(item is IArticle)) throw new ArgumentException("The supplied item " + item.Name + "#" + item.ID + " doesn't implement IArticle.", "item");

            WikiParser parser = Engine.Resolve<WikiParser>();
            WikiRenderer renderer = Engine.Resolve<WikiRenderer>();

            PlaceHolder ph = new PlaceHolder();
            container.Controls.Add(ph);

            renderer.AddTo(parser.Parse((string)item[detailName]), container, (item as IArticle).WikiRoot, item);
            
            return ph;
        }
    }
}
