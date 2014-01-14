namespace N2.Edit.Workflow
{
    /// <summary>
    /// Base class for commands used by <see cref="CommandDispatcher"/>.
    /// </summary>
    /// <typeparam name="T">The type of context to process.</typeparam>
    public abstract class CommandBase<T>
    {
        public virtual string Name { get { return GetType().Name; } }
        public string Title { get; set; }

        public abstract void Process(T state);
    }
}
