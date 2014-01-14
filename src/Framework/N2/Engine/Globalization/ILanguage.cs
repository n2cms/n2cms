using N2.Persistence.Search;
namespace N2.Engine.Globalization
{
    /// <summary>
    /// Represents a language available to the translation manager. In the default
    /// implementation this interface is implemented by the start node of a 
    /// language branch.
    /// </summary>
    public interface ILanguage
    {
        /// <summary>The friendly name of the language.</summary>
        string LanguageTitle { get; }

        /// <summary>The identifier of the language.</summary>
        string LanguageCode { get; }
    }
}
