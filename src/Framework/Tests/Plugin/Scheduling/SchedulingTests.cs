using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using N2.Configuration;
using N2.Engine;
using N2.Plugin;
using N2.Plugin.Scheduling;
using N2.Web;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using Shouldly;

namespace N2.Tests.Plugin.Scheduling
{
    [TestFixture]
    public class SchedulingTests : ItemTestsBase
    {
        Scheduler scheduler;
        IEventRaiser raiser;
        IErrorNotifier errorHandler;
		private IWebContext ctx;
		private Fakes.FakeEngine engine;
		private AsyncWorker worker;
		private EngineSection config;
		private ScheduledAction[] actions;
		private IHeart heart;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

			config = TestSupport.SetupEngineSection();
			actions = new ScheduledAction[] { new OnceAction(), new RepeatAction() };

			heart = mocks.Stub<IHeart>();
			heart.Beat += null;
			raiser = LastCall.IgnoreArguments().GetEventRaiser();
			mocks.Replay(heart);

			errorHandler = mocks.DynamicMock<IErrorNotifier>();
            mocks.Replay(errorHandler);

            ctx = mocks.DynamicMock<IWebContext>();
            mocks.Replay(ctx);

			engine = new Fakes.FakeEngine();
			engine.Container.AddComponentInstance("", typeof(IErrorNotifier), MockRepository.GenerateStub<IErrorNotifier>());

			worker = new AsyncWorker();
			worker.QueueUserWorkItem = delegate(WaitCallback function)
			{
				function(null);
				return true;
			};

			scheduler = new Scheduler(engine, heart, worker, ctx, errorHandler, actions, config);
            scheduler.Start();
        }

        [Test]
        public void CanRunOnce()
        {
            OnceAction once = SelectThe<OnceAction>();
            RepeatAction repeat = SelectThe<RepeatAction>();

            raiser.Raise(null, new EventArgs());

            Assert.That(once.Executions, Is.EqualTo(1));
            Assert.That(repeat.Executions, Is.EqualTo(1));
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

            Func<DateTime> prev = N2.Utility.CurrentTime;
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
            Assert.That(repeat.Executions, Is.EqualTo(1));
        }

        [Test]
        public void RepeatAction_IsExecuted_AfterTimeElapsed()
        {
            RepeatAction repeat = SelectThe<RepeatAction>();

            Func<DateTime> prev = N2.Utility.CurrentTime;
            try
            {
                raiser.Raise(null, new EventArgs());
                N2.Utility.CurrentTime = delegate { return DateTime.Now.AddSeconds(70); };
                raiser.Raise(null, new EventArgs());

                Assert.That(repeat.Executions, Is.EqualTo(2));
            }
            finally
            {
                N2.Utility.CurrentTime = prev;
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
            //errorHandler.Notify(ex);  // wayne: custom error handler called instead of the error 
            mocks.ReplayAll();

            raiser.Raise(null, new EventArgs());

            Assert.That(once.Executions, Is.EqualTo(1));
            Assert.That(repeat.Executions, Is.EqualTo(1));
        }

		[Test]
		public void Action_WithIClosableInterface_AreDisposed()
		{
			var action = new ClosableAction();
			scheduler.Actions.Insert(0, action);
			raiser.Raise(null, new EventArgs());
			Assert.That(action.wasExecuted);
			Assert.That(action.wasClosed);
		}

		[Test, Ignore]
		public void ScheduledAction_WillBeInjected_WithDependencies()
		{
			throw new NotImplementedException("TODO test");
		}

		[Test]
		public void DisabledScheduler_DoesntExecuteActions()
		{
			config = new EngineSection { Scheduler = new SchedulerElement { Enabled = false } };
			scheduler = new Scheduler(engine, heart, worker, ctx, errorHandler, actions, config);

			scheduler.Actions.Count.ShouldBe(0);
		}

		[Test]
		public void RemovedAction_IsNotExecuted()
		{
			config.Scheduler.Remove(new ScheduledActionElement { Name = "RepeatAction" });
			scheduler = new Scheduler(engine, heart, worker, ctx, errorHandler, actions, config);

			scheduler.Actions.Single().ShouldBeTypeOf<OnceAction>();
		}

		[Test]
		public void Interval_AndRepeat_CanBeReconfigured()
		{
			config.Scheduler.Add(new ScheduledActionElement { Name = "OnceAction", Interval = TimeSpan.FromHours(6), Repeat = true });
			scheduler = new Scheduler(engine, heart, worker, ctx, errorHandler, actions, config);

			var once = SelectThe<OnceAction>();
			once.Repeat.ShouldBe(Repeat.Indefinitely);
			once.Interval.ShouldBe(TimeSpan.FromHours(6));
		}

		[Test]
		public void Scheduler_OnMachineWithName_ExecutesActions()
		{
			config = new EngineSection { Scheduler = new SchedulerElement { ExecuteOnMachineNamed = Environment.MachineName } };
			scheduler = new Scheduler(engine, heart, worker, ctx, errorHandler, actions, config);

			scheduler.Actions.Count.ShouldBe(2);
		}

		[Test]
		public void Scheduler_OnMachineWithOtherName_DoesntExecuteActions()
		{
			config = new EngineSection { Scheduler = new SchedulerElement { ExecuteOnMachineNamed = "SomeOtherMachine" } };
			scheduler = new Scheduler(engine, heart, worker, ctx, errorHandler, actions, config);

			scheduler.Actions.Count.ShouldBe(0);
		}

		[Test]
		public void Action_OnMachineWithName_IsExecuted()
		{
			config.Scheduler.Add(new ScheduledActionElement { Name = "OnceAction", ExecuteOnMachineNamed = Environment.MachineName });
			scheduler = new Scheduler(engine, heart, worker, ctx, errorHandler, actions, config);

			scheduler.Actions.Count.ShouldBe(2);
		}

		[Test]
		public void Action_OnMachineWithOtherName_DoesntExecute()
		{
			config.Scheduler.Add(new ScheduledActionElement { Name = "OnceAction", ExecuteOnMachineNamed = "SomeOtherMachine" });
			scheduler = new Scheduler(engine, heart, worker, ctx, errorHandler, actions, config);

			scheduler.Actions.Count.ShouldBe(1);
		}

        private T SelectThe<T>() where T: ScheduledAction
        {
			return scheduler.Actions.OfType<T>().Single();
        }

        private class ClosableAction : ScheduledAction, N2.Web.IClosable
        {
            public bool wasExecuted = false;
            public bool wasClosed = false;

            public override void Execute()
            {
                wasExecuted = true;
            }

            #region IDisposable Members

            public void Dispose()
            {
                wasClosed = true;
            }

            #endregion
        }

        private class ThrowingAction : ScheduledAction
        {
            public Exception ExceptionToThrow { get; set; }
            public override void Execute()
            {
                throw ExceptionToThrow;
            }
        }
    }
}
