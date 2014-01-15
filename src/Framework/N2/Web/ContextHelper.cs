using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine.Globalization;
using N2.Engine;

namespace N2.Web
{
    public class ContextHelper
    {
        public ContextHelper(Func<IEngine> engineGetter, Func<PathData> pathGetter)
        {
            this.EngineGetter = engineGetter;
            this.PathGetter = pathGetter;
        }

        public Func<IEngine> EngineGetter { get; set; }
        public Func<PathData> PathGetter { get; set; }

        public IEngine Engine 
        { 
            get { return EngineGetter(); }
            set { EngineGetter = () => value; }
        }

        public PathData Path
        {
            get { return PathGetter() ?? PathData.Empty; }
        }

        public ContentItem Page
        {
            get { return Path.CurrentPage; }
        }

        public ContentItem Item
        {
            get { return Path.CurrentItem; }
        }

        public ContentItem Part
        {
            get { return Path.CurrentItem != null && !Path.CurrentItem.IsPage ? Path.CurrentItem : null; }
        }

        public ILanguage Language
        {
            get { return EngineGetter().Resolve<ILanguageGateway>().GetLanguage(Page); }
        }

        public string LanguageCode
        {
            get
            {
                var lang = Language;
                if (lang == null)
                    return null;
                return lang.LanguageCode;
            }
        }

        public bool IsPage
        {
            get { return Item != null && Item.IsPage; }
        }

        public bool IsEmpty
        {
            get { return Item != null; }
        }
    }
}
