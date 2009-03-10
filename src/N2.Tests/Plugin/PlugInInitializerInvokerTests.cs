using System;
using NUnit.Framework;
using N2.Plugin;
using N2.Engine;
using Rhino.Mocks;
using System.Reflection;
using N2.Tests.Plugin;

namespace N2.Tests.PlugIn
{
	[TestFixture]
	public class PlugInInitializerInvokerTests : ItemTestsBase
	{
        IEngine engine;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

            engine = mocks.Stub<IEngine>();
		}

		[Test]
		public void AssemblyDefinedPlugin_IsInvoked()
		{
			ITypeFinder typeFinder = mocks.StrictMock<ITypeFinder>();
			Expect.Call(typeFinder.GetAssemblies())
				.Return(new Assembly[] { typeof(PlugIn1).Assembly });
			Expect.Call(typeFinder.Find(typeof(IPluginInitializer)))
				.Return(new Type[] { typeof(PlugIn1) });

			mocks.ReplayAll();

			PluginBootstrapper invoker = new PluginBootstrapper(typeFinder);
            PlugIn1.WasInitialized = false;
			invoker.InitializePlugins(engine, invoker.GetPluginDefinitions());
            Assert.That(PlugIn1.WasInitialized, Is.True);
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

			mocks.ReplayAll();

			PluginBootstrapper invoker = new PluginBootstrapper(typeFinder);
            PlugIn2.WasInitialized = false;
			invoker.InitializePlugins(engine, invoker.GetPluginDefinitions());
            Assert.That(PlugIn2.WasInitialized, Is.True);
			mocks.VerifyAll();
		}

        [Test]
        public void Initializers_AreExecuted_AfterException()
        {
            ITypeFinder typeFinder = mocks.StrictMock<ITypeFinder>();
            Expect.Call(typeFinder.GetAssemblies())
                .Return(new Assembly[] { });
            Expect.Call(typeFinder.Find(typeof(IPluginInitializer)))
                .Return(new Type[] { typeof(ThrowingPlugin1), typeof(PlugIn2) });

            mocks.ReplayAll();

            PluginBootstrapper invoker = new PluginBootstrapper(typeFinder);
            PlugIn2.WasInitialized = false;

            ThrowingPlugin1.Throw = true;
            PluginInitializationException ex = ExceptionAssert.Throws<PluginInitializationException>(delegate
            {
                invoker.InitializePlugins(engine, invoker.GetPluginDefinitions());
            });
            ThrowingPlugin1.Throw = false;
            Assert.That(PlugIn2.WasInitialized, Is.True);
        }

        [Test]
        public void InnerException_IsInnerException_OfInitializationException()
        {
            ITypeFinder typeFinder = mocks.StrictMock<ITypeFinder>();
            Expect.Call(typeFinder.GetAssemblies())
                .Return(new Assembly[] { });
            Expect.Call(typeFinder.Find(typeof(IPluginInitializer)))
                .Return(new Type[] { typeof(ThrowingPlugin1), typeof(PlugIn2) });

            mocks.ReplayAll();

            PluginBootstrapper invoker = new PluginBootstrapper(typeFinder);
            PlugIn2.WasInitialized = false;

            ThrowingPlugin1.Throw = true;
            PluginInitializationException ex = ExceptionAssert.Throws<PluginInitializationException>(delegate
            {
                invoker.InitializePlugins(engine, invoker.GetPluginDefinitions());
            });
            ThrowingPlugin1.Throw = false;
            Assert.That(ex.InnerException, Is.TypeOf(typeof(SomeException)));
            Assert.That(ex.Message.Contains("ThrowingPlugin1 isn't happy."));
        }

        [Test]
        public void InnerExceptions_AreAdded_ToInitializationException()
        {
            ITypeFinder typeFinder = mocks.StrictMock<ITypeFinder>();
            Expect.Call(typeFinder.GetAssemblies())
                .Return(new Assembly[] { });
            Expect.Call(typeFinder.Find(typeof(IPluginInitializer)))
                .Return(new Type[] { typeof(ThrowingPlugin1), typeof(PlugIn2), typeof(ThrowingPlugin2)});

            mocks.ReplayAll();

            PluginBootstrapper invoker = new PluginBootstrapper(typeFinder);
            PlugIn2.WasInitialized = false;

            ThrowingPlugin1.Throw = true;
            ThrowingPlugin2.Throw = true;
            PluginInitializationException ex = ExceptionAssert.Throws<PluginInitializationException>(delegate
            {
                invoker.InitializePlugins(engine, invoker.GetPluginDefinitions());
            });
            ThrowingPlugin1.Throw = false; 
            ThrowingPlugin2.Throw = false; 
            Assert.That(ex.InnerExceptions.Length, Is.EqualTo(2));
            Assert.That(ex.Message.Contains("ThrowingPlugin1 isn't happy."));
            Assert.That(ex.Message.Contains("ThrowingPlugin2 is really mad."));
        }
	}
}
