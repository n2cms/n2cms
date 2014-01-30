using System;

namespace N2.Engine
{
    /// <summary>
    /// Defines a class that does work.
    /// </summary>
    public interface IWorker
    {
        /// <summary>Gets the number of executing work actions.</summary>
        int ExecutingWorkItems { get; }

        /// <summary>Starts the execution of the specified work.</summary>
        /// <param name="action">The method to execute.</param>
        void DoWork(Action action);
    }
}
