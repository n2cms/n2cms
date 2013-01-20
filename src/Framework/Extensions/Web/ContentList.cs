/*************************************************************************************************

News List: Generic Model
Licensed to users of N2CMS under the terms of The MIT License (MIT)

Copyright (c) 2013 Benjamin Herila <mailto:ben@herila.net>

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, 
sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or 
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING 
BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*************************************************************************************************/


using System;
using System.Diagnostics;
using N2.Collections;
using N2.Details;
using N2.Integrity;
using System.Collections.Generic;

namespace N2.Web
{
	public enum HeadingLevel
	{
		H1 = 1,
		H2 = 2,
		H3 = 3,
		H4 = 4
	}

	public enum NewsDisplayMode
	{
		TitleLinkOnly = 0,
		TitleAndAbstract = 1,
		TitleAndText = 2,
		HtmlItemTemplate = 3
	}

	public enum SortMode
	{
		Descending = 0,
		Ascending = 1
	}

    [PartDefinition("ContentContainer Link", IconUrl = "{IconsUrl}/link.png")]
    [RestrictParents(typeof(ContentList))]
    public class ContentListContainerLink : ContentItem, Definitions.IPart
    {

		/// <summary>
		/// Link to a container of news or blog items.
		/// </summary>
        [EditableLink("News container", 100, SelectableTypes = new[] { typeof(ContentItem) })]
		public virtual ContentItem Container
        {
			get { return (ContentItem)GetDetail("Container"); }
            set
            {
	            if (!value.IsPage)
					throw new ArgumentException("value must be a page (IsPage == true)");
				SetDetail("Container", value);
            }
        }

    }

    [PartDefinition("Content List",
        Description = "A list of pages that can be displayed in a column.",
        SortOrder = 160,
        IconUrl = "{IconsUrl}/newspaper_go.png")]
    [WithEditableTitle("Title", 10, Required = false)]
    [AvailableZone("Sources", "Sources")]
    [RestrictChildren(typeof(ContentListContainerLink))]
    public class ContentList : ContentItem
    {
        public override string TemplateKey
        {
            get { return "NewsList"; }
            set { base.TemplateKey = "NewsList"; }
        }

        [EditableEnum("Title heading level", 90, typeof(HeadingLevel))]
        public virtual int TitleLevel
        {
            get { return (int)(GetDetail("TitleLevel") ?? 3); }
            set { SetDetail("TitleLevel", value, 3); }
        }

        [EditableChildren("News container", "Sources", 100)]
        public virtual IList<ContentListContainerLink> Containers
        {
            get
            {
                try
                {
                    if (GetChildren() == null)
                        return new List<ContentListContainerLink>();
                    //else
                        return GetChildren().Cast<ContentListContainerLink>();
                }
                catch (Exception x)
                {
                    Exceptions.Add(x.ToString());
                    return new List<ContentListContainerLink>();
                }
            }
        }

        [EditableNumber("Max news to display", 120)]
        public virtual int MaxNews
        {
            get { return (int)(GetDetail("MaxNews") ?? 3); }
            set { SetDetail("MaxNews", value, 3); }
        }

        public virtual void Filter(ItemList items)
        {
	        Debug.Assert(items != null, "items != null");
	        PageFilter.FilterPages(items);
            CountFilter.Filter(items, 0, MaxNews);
        }

        [EditableEnum(
            Title = "Display mode",
            SortOrder = 150,
            EnumType = typeof(NewsDisplayMode))
        ]
        public virtual NewsDisplayMode DisplayMode
        {
            get { return (NewsDisplayMode)(GetDetail("DisplayMode") ?? NewsDisplayMode.TitleAndAbstract); }
            set { SetDetail("DisplayMode", (int)value, (int)NewsDisplayMode.TitleAndAbstract); }
        }

        [EditableEnum(
            Title = "Sort mode",
            SortOrder = 200,
            EnumType = typeof(SortMode))
        ]
        public virtual SortMode SortByDate
        {
            get { return (SortMode)(GetDetail("SortByDate") ?? SortMode.Descending); }
            set { SetDetail("SortByDate", (int)value, (int)SortMode.Descending); }
        }

        [EditableCheckBox("Group by month", 250)]
        public virtual bool GroupByMonth
        {
            get { return (bool)(GetDetail("GroupByMonth") ?? true); }
            set { SetDetail("GroupByMonth", value, true); }
        }

        [EditableCheckBox("Show Past Items", 500, CheckBoxText = "Show Past Items")]
        public virtual bool ShowPastEvents
        {
            get { return (bool)(GetDetail("ShowPastEvents") ?? true); }
            set { SetDetail("ShowPastEvents", value, true); }
        }

        [EditableCheckBox("Show Future Items", 501, CheckBoxText = "Show Future Items")]
        public virtual bool ShowFutureEvents
        {
            get { return (bool)(GetDetail("ShowFutureEvents") ?? false); }
            set { SetDetail("ShowFutureEvents", value, false); }
        }

		//TODO: Make this property visible only if the NewsDisplayMode is set to HtmlItemTemplate
		[EditableText(
			Rows = 10, 
			Columns = 50, 
			TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine, 
			Title = "Custom Item Template (HTML)", 
			HelpText = "Enclose properties of the news/blog items in $$, e.g. $$Text$$ to insert the \"Text\" property at that location.")
		]
		public string HtmlItemTemplate
		{
			get { return GetDetail("HtmlItemTemplate", ""); }
			set { SetDetail("HtmlItemTemplate", value, ""); }
		}

        public List<string> Exceptions = new List<string>();

    }
}