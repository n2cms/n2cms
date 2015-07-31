using System.Configuration;

namespace N2.Configuration
{
	public class MessagingElement : ConfigurationElement
	{
		/// <summary>The name of the sender to report to targets..</summary>
		[ConfigurationProperty("senderName", DefaultValue = "Unknown")]
		public string SenderName
		{
			get { return (string)base["senderName"]; }
			set { base["senderName"] = value; }
		}

		/// <summary>Whether messages are dispatched to targets.</summary>
		[ConfigurationProperty("enabled", DefaultValue = true)]
		public bool Enabled
		{
			get { return (bool)base["enabled"]; }
			set { base["enabled"] = value; }
		}

		/// <summary>Whether messages are sent immediately or whether the send method returns immediately and messages are sent on a separate thread.</summary>
		[ConfigurationProperty("async", DefaultValue = true)]
		public bool Async
		{
			get { return (bool)base["async"]; }
			set { base["async"] = value; }
		}

		[ConfigurationProperty("targets")]
		public MessageTargetsCollection Targets
		{
			get { return (MessageTargetsCollection)base["targets"]; }
			set { base["targets"] = value; }
		}
	}
}
