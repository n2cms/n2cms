using System.Configuration;

namespace N2.Configuration
{
	/// <summary>
	/// A collection of pattern replacements for the name editor.
	/// </summary>
	public class PatternValueCollection : ConfigurationElementCollection
	{
		public PatternValueCollection()
		{
			BaseAdd(new PatterValueElement("[Â‰·‡‚„@]", "a", true));
			BaseAdd(new PatterValueElement("[≈ƒ¡¿¬√]", "a", true));
			BaseAdd(new PatterValueElement("[Ê]", "ae", true));
			BaseAdd(new PatterValueElement("[∆]", "AE", true));
			BaseAdd(new PatterValueElement("[ˆ¯Ùı]", "o", true));
			BaseAdd(new PatterValueElement("[÷ÿ‘’]", "O", true));
			BaseAdd(new PatterValueElement("[^ a-zA-Z0-9_-]", "", true));
		}
		protected override ConfigurationElement CreateNewElement()
		{
			return new PatterValueElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((PatterValueElement) element).Pattern;
		}
	}
}