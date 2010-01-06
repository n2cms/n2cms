using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Engine
{
	/// <summary>
	/// An event argument passing a value.
	/// </summary>
	/// <typeparam name="T">The type of value to pass.</typeparam>
	public class ValueEventArgs<T> : EventArgs
	{
		public T Value { get; set; }
	}
}
