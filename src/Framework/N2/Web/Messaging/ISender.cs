using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Messaging
{
	/// <summary>
	/// Sends messages to web sites.using a messaging system.
	/// </summary>
	public interface ISender
	{
		/// <summary>The name of the sender.</summary>
		string Name { get; }

		/// <summary>The targest that will receive messages.</summary>
		IEnumerable<MessageTarget> Targets { get; }

		/// <summary>Broadcasts the message to all targetes.</summary>
		/// <param name="messageType">The type of message to send. This is a key that connects the message to receivers on the other side.</param>
		/// <param name="message">The message data. This is serialized to json.</param>
		void Broadcast(string messageType, string message);

		/// <summary>Sends the message the named target only.</summary>
		/// <param name="targetName">The name of the target to send to.</param>
		/// <param name="messageType">The type of message to send. This is a key that connects the message to receivers on the other side.</param>
		/// <param name="message">The message data. This is serialized to json.</param>
		/// <remarks>This send method ignores only from machine named config settings.</remarks>
		void Send(string targetName, string messageType, string message);
	}
}
