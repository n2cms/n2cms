using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Plugin.Scheduling;
using N2.Plugin;
using Rhino.Mocks;
using N2.Engine;
using Rhino.Mocks.Interfaces;
using System.Timers;
using NUnit.Framework.SyntaxHelpers;
using System.Reflection;
using System.Threading;
using N2.Web;

namespace N2.Tests.Plugin.Scheduling
{
    [TestFixture]
    public class SchedulingTests : ItemTestsBase
    {
        Scheduler scheduler;
        IEventRaiser raiser;
        IErrorHandler errorHandler;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            ITypeFinder types = mocks.Stub<ITypeFinder>();
            Expect.Call(types.GetAssemblies()).Return(new Assembly[] { GetType().Assembly }).Repeat.Any();
            mocks.Replay(types);

            IHeart heart = mocks.Stub<IHeart>();
            heart.Beat += null;
            raiser = LastCall.IgnoreArguments().GetEventRaiser();
            mocks.Replay(heart);

            errorHandler = mocks.DynamicMock<IErrorHandler>();
            mocks.Replay(errorHandler);

            IPluginFinder plugins = new PluginFinder(types);

            scheduler = new Scheduler(plugins, heart, errorHandler);
            scheduler.QueueUserWorkItem = delegate(WaitCallback function)
            {
                function(null);
                return true;
            };
            scheduler.Start();
        }

        [Test]
        public void CanRunOnce()
        {
            OnceAction once = SelectThe<OnceAction>();
            RepeatAction repeat = SelectThe<RepeatAction>();

            raiser.Raise(null, new EventArgs());

            Assert.That(once.executions, Is.EqualTo(1));
            Assert.That(repeat.executions, Is.EqualTo(1));
        }

        [Test]
        public void OnceAction_IsRemoved_AfterRunningOnce()
        {
            OnceAction once = SelectThe<OnceAction>();
            RepeatAction repeat = SelectThe<RepeatAction>();

            raiser.Raise(null, new EventArgs());

            Assert.That(scheduler.Actions.Contains(once), Is.False);
        }

        [Test]
        public void RepeatAction_IsNotExecuted_BeforeTimeElapsed()
        {
            RepeatAction repeat = SelectThe<RepeatAction>();

            Function<DateTime> prev = N2.Utility.CurrentTime;
            try
            {
                raiser.Raise(null, new EventArgs());
                N2.Utility.CurrentTime = delegate { return DateTime.Now.AddSeconds(50); };
                raiser.Raise(null, new EventArgs());
            }
            finally
            {
                N2.Utility.CurrentTime = prev;
            }
            Assert.That(repeat.executions, Is.EqualTo(1));
        }

        [Test]
        public void RepeatAction_IsExecuted_AfterTimeElapsed()
        {
            RepeatAction repeat = SelectThe<RepeatAction>();

            Function<DateTime> prev = N2.Utility.CurrentTime;
            try
            {
                raiser.Raise(null, new EventArgs());
                N2.Utility.CurrentTime = delegate { return DateTime.Now.AddSeconds(70); };
                raiser.Raise(null, new EventArgs());

                Assert.That(repeat.executions, Is.EqualTo(2));
            }
            finally
            {
                N2.Utility.CurrentTime = prev;
            }
        }

        private class ThrowingAction : ScheduledAction
        {
            public Exception ExceptionToThrow { get; set; }
            public override void Execute()
            {
                throw ExceptionToThrow;
            }
        }

        [Test]
        public void WillCatchErrors_AndContinueExecution_OfOtherActions()
        {
            var ex = new NullReferenceException("Bad bad");
            scheduler.Actions.Insert(0, new ThrowingAction { ExceptionToThrow = ex });
            OnceAction once = SelectThe<OnceAction>();
            RepeatAction repeat = SelectThe<RepeatAction>();

            mocks.BackToRecord(errorHandler);
            errorHandler.Handle(ex);
            mocks.ReplayAll();

            raiser.Raise(null, new EventArgs());

            Assert.That(once.executions, Is.EqualTo(1));
            Assert.That(repeat.executions, Is.EqualTo(1));
        }

        private T SelectThe<T>() where T: ScheduledAction
        {
            return (from a in scheduler.Actions where a.GetType() == typeof(T) select a).Single() as T;
        }
    }
}
