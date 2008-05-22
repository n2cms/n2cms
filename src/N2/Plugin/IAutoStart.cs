using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Plugin
{
	/// <summary>
	/// Informas the medium trust engine that this component should be started 
	/// through the start method. This is normally performed by the castle 
	/// startable facility but this is a quick fix for medium trust environments.
	/// </summary>
	public interface IAutoStart
	{
		/// <summary>The method invoked by the medium trust engine.</summary>
		void Start();
	}
}
