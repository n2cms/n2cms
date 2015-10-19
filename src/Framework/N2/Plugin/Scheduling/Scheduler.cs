using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using N2.Engine;
using N2.Web;
using System.Threading;
using System.Globalization;

namespace N2.Plugin.Scheduling
{
    /// <summary>
    /// Maintains a list of scheduler actions and checks whether it's time to 
    /// execute them.
    /// </summary>
    [Service]
    public class Scheduler : IAutoStart
    {
        private readonly Engine.Logger<Scheduler> logger;

	    readonly IList<ScheduledAction> actions = new List<ScheduledAction>();
	    readonly IHeart heart;
        readonly IWorker worker;
	    readonly IWebContext context;
	    readonly IErrorNotifier errorHandler;
	    readonly IEngine engine;
        private bool enabled;
        private bool asyncActions;
        private bool runWhileDebuggerAttached;

        public Scheduler(IEngine engine, IHeart heart, IWorker worker, IWebContext context, IErrorNotifier errorHandler, ScheduledAction[] registeredActions, Configuration.EngineSection config)
        {
            this.engine = engine;
            this.heart = heart;
            this.worker = worker;
            this.context = context;
            this.errorHandler = errorHandler;

            this.enabled = config.Scheduler.Enabled;
            this.asyncActions = config.Scheduler.AsyncActions;
            this.runWhileDebuggerAttached = config.Scheduler.RunWhileDebuggerAttached;
            if (!string.IsNullOrEmpty(config.Scheduler.ExecuteOnMachineNamed))
                if (config.Scheduler.ExecuteOnMachineNamed != Environment.MachineName)
                    this.enabled = false;

            if (enabled)
            {
                actions = new List<ScheduledAction>(InstantiateActions(registeredActions, config.Scheduler));
            }
        }

        public IList<ScheduledAction> Actions
        {
            get { return actions; }
        }

        private IEnumerable<ScheduledAction> InstantiateActions(IEnumerable<ScheduledAction> registeredActions, Configuration.SchedulerElement schedulerElement)
        {
            var isClear = schedulerElement.IsCleared;
            var removedNames = new HashSet<string>(schedulerElement.RemovedElements.Select(re => re.Name));
            var added = schedulerElement.AddedElements.ToDictionary(ae => ae.Name);
            foreach (var action in registeredActions)
            {
                string name = action.GetType().Name;
                if (removedNames.Contains(name) && !added.ContainsKey(name))
                    continue;
                if (isClear && !added.ContainsKey(name))
                    continue;
                if (added.ContainsKey(name))
                {
                    var actionConfig = added[name];
                    if (!string.IsNullOrEmpty(actionConfig.ExecuteOnMachineNamed) && actionConfig.ExecuteOnMachineNamed != Environment.MachineName)
                        continue;
                }

                if (added.ContainsKey(name))
                {
                    var actionConfig = added[name];
                    if (actionConfig.Interval.HasValue)
                        action.Interval = actionConfig.Interval.Value;
                    if (actionConfig.Repeat.HasValue)
                        action.Repeat = actionConfig.Repeat.Value ? Repeat.Indefinitely : Repeat.Once;
                }

                yield return action;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        void heart_Beat(object sender, EventArgs e)
        {
            ExecuteActions();
        }

        /// <summary>Executes the scheduled actions that are scheduled for executions.</summary>
        public void ExecuteActions()
        {
            if (!enabled)
                return;

            if (Debugger.IsAttached && !runWhileDebuggerAttached)
                return;

            for (int i = 0; i < actions.Count; i++)
            {
                ScheduledAction action = actions[i];
                if (action.ShouldExecute())
                {
                    Action work = delegate
                    {
                        try
                        {
                            var config = ((System.Web.Configuration.GlobalizationSection)System.Configuration.ConfigurationManager.GetSection("system.web/globalization"));
                            if (!string.IsNullOrEmpty(config.Culture))
                                Thread.CurrentThread.CurrentCulture = new CultureInfo(config.Culture);
                            if (!string.IsNullOrEmpty(config.UICulture))
                                Thread.CurrentThread.CurrentUICulture = new CultureInfo(config.UICulture);
                        }
                        catch (Exception ex)
                        {
                            logger.Warn(ex);
                        }

                        try
                        {
                            logger.Debug("Executing " + action.GetType().Name);
                            action.Engine = engine;
                            action.Execute();
                            action.ErrorCount = 0;
                        }
                        catch (Exception ex)
                        {
                            action.ErrorCount++;
							action.LastError = ex;
							try
							{
								logger.Error("Exception executing scheduled action: ", ex);
								action.OnError(ex);     // wayne: call custom action error handler
							}
							catch (Exception ex2)
							{
								logger.Error("Exception handling error: ", ex2);
							}
                        }
                        finally
                        {
                            try
                            {
                                IClosable closable = action as IClosable;
                                if (closable != null)
                                    closable.Dispose();
                            }
                            catch (Exception ex)
                            {
                                errorHandler.Notify(ex);
                            }
                        }
                        action.LastExecuted = Utility.CurrentTime();
                        action.IsExecuting = false;

                        try
                        {
                            context.Close();
                        }
                        catch (Exception ex)
                        {
                            errorHandler.Notify(ex);
                        }
                    };

                    action.IsExecuting = true;
                    if (asyncActions)
                        worker.DoWork(work);
                    else
                        work();

                    if (action.Repeat == Repeat.Once)
                    {
                        actions.RemoveAt(i);
                        --i;
                    }
                }
            }
        }

        #region IStartable Members

        public void Start()
        {
            heart.Beat += new EventHandler(heart_Beat);
        }

        public void Stop()
        {
            heart.Beat -= new EventHandler(heart_Beat);
        }

        #endregion
    }
}
