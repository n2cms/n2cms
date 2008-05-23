using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using N2.Plugin;
using N2.Engine;
using Rhino.Mocks;
using System.Reflection;

namespace N2.Tests.PlugIn
{
	[TestFixture]
	public class PlugInInitializerInvokerTests : ItemTestsBase
	{
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
		}

		[Test]
		public void AssemblyDefinedPlugin_IsInvoked()
		{
			ITypeFinder typeFinder = mocks.StrictMock<ITypeFinder>();
			Expect.Call(typeFinder.GetAssemblies())
				.Return(new Assembly[] { typeof(PlugIn1).Assembly });
			Expect.Call(typeFinder.Find(typeof(IPluginInitializer)))
				.Return(new Type[] { typeof(PlugIn1) });

			IEngine engine = mocks.StrictMock<IEngine>();
			Expect.Call(engine.Persister).Return(null);

			mocks.ReplayAll();

			PluginBootstrapper invoker = new PluginBootstrapper(typeFinder);
			invoker.InitializePlugins(engine, invoker.GetPluginDefinitions());
			
			mocks.VerifyAll();
		}

		[Test]
		public void AutoInitializePlugin_IsInvoked()
		{
			ITypeFinder typeFinder = mocks.StrictMock<ITypeFinder>();
			Expect.Call(typeFinder.GetAssemblies())
				.Return(new Assembly[] {});
			Expect.Call(typeFinder.Find(typeof(IPluginInitializer)))
				.Return(new Type[] { typeof(PlugIn2) });

			IEngine engine = mocks.StrictMock<IEngine>();
			Expect.Call(engine.Definitions).Return(null);

			mocks.ReplayAll();

			PluginBootstrapper invoker = new PluginBootstrapper(typeFinder);
			invoker.InitializePlugins(engine, invoker.GetPluginDefinitions());

			mocks.VerifyAll();
		}
	}
}
