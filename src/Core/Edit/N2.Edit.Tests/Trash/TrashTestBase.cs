using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using Rhino.Mocks;

namespace N2.Trashcan.Tests
{
	public class TrashTestBase
	{
		protected MockRepository mocks;
		protected ThrowableItem root;
		protected ThrowableItem item;
		protected TrashContainerItem trash;

		[SetUp]
		public virtual void SetUp()
		{
			mocks = new MockRepository();

			root = CreateItem<ThrowableItem>(1, "root", null);
			item = CreateItem<ThrowableItem>(2, "item", root);
			trash = CreateItem<TrashContainerItem>(3, "Trash", root);
		}

		[TearDown]
		public void TearDown()
		{
			mocks.ReplayAll();
			mocks.VerifyAll();
		}

		protected T CreateItem<T>(int id, string name, ContentItem parent) where T : ContentItem, new()
		{
			T i = new T();
			i.Name = name;
			i.ID = id;
			i.AddTo(parent);
			return i;
		}
	}
}
