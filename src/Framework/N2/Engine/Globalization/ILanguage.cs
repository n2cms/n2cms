using N2.Persistence.Search;
namespace N2.Engine.Globalization
{
	/// <summary>
	/// Represents a language available to the translation manager. In the default
	/// implementation this interface is implemented by the start node of a 
	/// language branch.
	/// </summary>
	[SearchableType]
	public interface ILanguage
	{
		/// <summary>The path of the image representing this language. It's used to represent the language in the editor interface.</summary>
		string FlagUrl { get; }

		/// <summary>The friendly name of the language.</summary>
		string LanguageTitle { get; }

		/// <summary>The identifier of the language.</summary>
		string LanguageCode { get; }
	}
}
