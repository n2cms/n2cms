using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Castle.Core;
using System.Runtime.CompilerServices;
using N2.Web;

namespace N2.Plugin.Scheduling
{
    /// <summary>
    /// Maintains a list of scheduler actions and checks wether it's time to 
    /// execute them.
    /// </summary>
    public class Scheduler : IStartable, IAutoStart
    {
        IList<ScheduledAction> actions;
        IHeart heart;
        Web.IWebContext context;
        IErrorHandler errorHandler;

        public Engine.Function<WaitCallback, bool> QueueUserWorkItem = ThreadPool.QueueUserWorkItem;

        public Scheduler(IPluginFinder plugins, IHeart heart, Web.IWebContext context, IErrorHandler errorHandler)
        {
            actions = new List<ScheduledAction>(InstantiateActions(plugins));
            this.heart = heart;
            this.context = context;
            this.errorHandler = errorHandler;
        }

        public IList<ScheduledAction> Actions
        {
            get { return actions; }
        }

        protected TimeSpan CalculateInterval(int interval, TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Seconds:
                    return new TimeSpan(0, 0, interval);
                case TimeUnit.Minutes:
                    return new TimeSpan(0, interval, 0);
                case TimeUnit.Hours:
                    return new TimeSpan(interval, 0, 0);
                default:
                    throw new NotImplementedException("Unknown time unit: " + unit);
            }
        }

        private IEnumerable<ScheduledAction> InstantiateActions(IPluginFinder plugins)
        {
            foreach (ScheduleExecutionAttribute attr in plugins.GetPlugins<ScheduleExecutionAttribute>())
            {
                ScheduledAction action = Activator.CreateInstance(attr.Decorates) as ScheduledAction;
                action.Interval = CalculateInterval(attr.Interval, attr.Unit);
                action.Repeat = attr.Repeat;
                yield return action;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        void heart_Beat(object sender, EventArgs e)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                ScheduledAction action = actions[i];
                if (!action.IsExecuting && action.LastExecuted.Add(action.Interval) < Utility.CurrentTime())
                {
                    action.IsExecuting = true;
                    QueueUserWorkItem(delegate  
                    {
                        try
                        {
                            action.Execute();
                        }
                        catch (Exception ex)
                        {
                            errorHandler.Notify(ex);
                        }
                        finally
                        {
                        }
                        action.LastExecuted = Utility.CurrentTime();
                        action.IsExecuting = false;
                    });

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
