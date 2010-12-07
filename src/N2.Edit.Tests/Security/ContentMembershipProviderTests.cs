using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Security;
using NUnit.Framework;

namespace N2.Tests.Security
{
	public class ContentMembershipProviderTests : ItemPersistenceMockingBase
	{
		ContentMembershipProvider provider;
			
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			//provider = new ContentMembershipProvider(new ItemBridge());
			
		}
	}
}
