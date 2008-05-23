using System;
using System.Collections.Generic;
using System.Text;
using N2.Engine;
using Rhino.Mocks;
using System.Reflection;

namespace N2.Tests
{
	public abstract class TypeFindingBase : ItemTestsBase
	{
		protected ITypeFinder typeFinder;

		protected abstract Type[] GetTypes();
		public override void SetUp()
		{
			base.SetUp();

			typeFinder = mocks.StrictMock<ITypeFinder>();
			Expect.On(typeFinder).Call(typeFinder.Find(typeof(ContentItem))).Return(GetTypes()).Repeat.Any();
			Expect.On(typeFinder).Call(typeFinder.GetAssemblies()).Return(new Assembly[] { typeof(TypeFindingBase).Assembly }).Repeat.Any();
			mocks.Replay(typeFinder);
		}
	}
}
