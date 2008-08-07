using System;
using System.Collections.Generic;
using System.Text;
using N2.Engine;
using Rhino.Mocks;
using System.Reflection;
using System.Security.Principal;

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

        protected IPrincipal CreateUser(string name, params string[] roles)
        {
            return base.CreatePrincipal(name, roles);
            //IPrincipal user = mocks.StrictMock<IPrincipal>();
            //IIdentity identity = mocks.StrictMock<IIdentity>();

            //Expect.On(user).Call(user.Identity).Return(identity).Repeat.AtLeastOnce();
            //Expect.On(user)
            //    .Call(user.IsInRole(null))
            //    .IgnoreArguments()
            //    .Repeat.Any()
            //    .Do(new IsInRole(delegate(string role)
            //    {
            //        return Array.IndexOf<string>(roles, role) >= 0;
            //    }));
            //mocks.Replay(user);

            //Expect.On(identity).Call(identity.Name).Return(name).Repeat.AtLeastOnce();
            //mocks.Replay(identity);
            //return user;
        }

        private delegate bool IsInRole(string role);
	}
}
