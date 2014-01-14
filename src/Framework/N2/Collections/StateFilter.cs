namespace N2.Collections
{
    /// <summary>
    /// Filters items not having a given state.
    /// </summary>
    public class StateFilter : ItemFilter
    {
        private ContentState requiredState;

        public StateFilter(ContentState requiredState)
        {
            this.requiredState = requiredState;
        }

        public override bool Match(ContentItem item)
        {
            return (item.State & requiredState) == requiredState;
        }

        public override string ToString()
        {
            return "State=" + requiredState;
        }
    }
}
