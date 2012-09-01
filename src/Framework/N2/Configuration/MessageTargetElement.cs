using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
	public class MessageTargetElement : NamedElement
	{
		/// <summary>When not empty this target is called on the machine with the configured name.</summary>
		[ConfigurationProperty("exceptFromMachineNamed")]
		public string ExceptFromMachineNamed
		{
			get { return (string)base["exceptFromMachineNamed"]; }
			set { base["exceptFromMachineNamed"] = value; }
		}

		/// <summary>
		/// The destination URL of the message. This typically has the format http://othermachine/N2/Resources/MessageSink.ashx
		/// </summary>
		[ConfigurationProperty("address")]
		public string Address
		{
			get { return (string)base["address"]; }
			set { base["address"] = value; }
		}
	}
}
