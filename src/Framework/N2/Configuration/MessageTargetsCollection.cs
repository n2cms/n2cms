using System.Configuration;

namespace N2.Configuration
{
	[ConfigurationCollection(typeof(MessageTargetElement))]
	public class MessageTargetsCollection : LazyRemovableCollection<MessageTargetElement>
	{
		/// <summary>When not empty this target is called on the machine with the configured name.</summary>
		[ConfigurationProperty("onlyFromMachineNamed")]
		public string OnlyFromMachineNamed
		{
			get { return (string)base["onlyFromMachineNamed"]; }
			set { base["onlyFromMachineNamed"] = value; }
		}

		/// <summary>By default forms authentication is used for encrypting a message header. This requires a shared machine key. Setting a shared secret will use this for encrypting.</summary>
		[ConfigurationProperty("sharedSecret")]
		public string SharedSecret
		{
			get { return (string)base["sharedSecret"]; }
			set { base["sharedSecret"] = value; }
		}
	}
}
