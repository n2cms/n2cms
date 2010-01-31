using N2.Web;
using System;
using System.Diagnostics;

namespace N2.Engine
{
	/// <summary>
	/// Convenience base class for content adapters. One instance 
	/// of the inheriting class is created per request upon usage.
	/// </summary>
	public abstract class AbstractContentAdapter : IComparable<AbstractContentAdapter>
	{
		public AbstractContentAdapter()
		{
			AdaptedType = typeof(ContentItem);
		}

		#region IContentAdapter Members

		/// <summary>The path associated with this controller instance.</summary>
		[Obsolete("Use provided parameters or resolve path using engine.", true)]
		public PathData Path 
		{
			get { return Engine.Resolve<IWebContext>().CurrentPath; }
		}

		/// <summary>The content engine requesting external control.</summary>
		[Obsolete("Create constructor or public properties to get adapter dependencies.")]
		public IEngine Engine { get; set; }

		/// <summary>The type adapted by this adapter instance.</summary>
		public Type AdaptedType { get; set; }

		#endregion


		#region IComparable<AbstractContentAdapter> Members

		int IComparable<AbstractContentAdapter>.CompareTo(AbstractContentAdapter other)
		{
			return 2 * Utility.InheritanceDepth(other.AdaptedType).CompareTo(Utility.InheritanceDepth(AdaptedType)) // order by depth of adapted type first
				+ Utility.InheritanceDepth(other.GetType()).CompareTo(Utility.InheritanceDepth(GetType())); // then by depth of adapter
		}

		#endregion
	}
}
