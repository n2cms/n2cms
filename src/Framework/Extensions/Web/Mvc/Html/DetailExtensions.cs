using System;
using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
    /// <summary>
    /// Helper class for loosley-typed evaluation of Detail properties on the current item
    /// </summary>
    public static class DetailExtensions
    {
        public static string Detail<TItem>(this HtmlHelper<TItem> helper, string detailName)
            where TItem : ContentItem
        {
            return Convert.ToString(helper.ViewData.Model[detailName]);
        }
    }
}
