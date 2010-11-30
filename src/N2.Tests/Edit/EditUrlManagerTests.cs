using System;
using N2.Configuration;
using N2.Edit;
using N2.Tests.Edit.Items;
using NUnit.Framework;

namespace N2.Tests.Edit
{
	public abstract class EditUrlManagerTests : TypeFindingBase
	{
		protected EditUrlManager editUrlManager;

		protected override Type[] GetTypes()
		{
			return new[]
			       	{
			       		typeof (ComplexContainersItem),
			       		typeof (ItemWithRequiredProperty),
			       		typeof (ItemWithModification),
			       		typeof (NotVersionableItem),
			       		typeof (LegacyNotVersionableItem),
			       		typeof (ItemWithSecuredContainer)
			       	};
		}

		[SetUp]
		public virtual void SetUp()
		{
			editUrlManager = new EditUrlManager(new EditSection());
		}
	}
}