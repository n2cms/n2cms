using System;
namespace N2.Engine.Globalization
{
    /// <summary>
    /// Used to cache information about a language available in the system.
    /// </summary>
    public class LanguageInfo : ILanguage
    {
        /// <summary>The identifier of item that is the root of this language.</summary>
        public int ID { get; set; }

        /// <summary>The language code of this language.</summary>
        public string LanguageCode { get; set; }

        /// <summary>The friendly name of the language.</summary>
        public string LanguageTitle { get; set; }

        public Func<ContentItem> Translation { get; set; }

        public static implicit operator ContentItem(LanguageInfo info)
        {
            return info.Translation();
        }
    }
}
