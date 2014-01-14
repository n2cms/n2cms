namespace N2.Definitions
{
    /// <summary>
    /// Sort orders used by <see cref="ISortableRefiner"/>.
    /// </summary>
    public static class RefineOrder
    {
        public const int First = -10000;
        public const int Before = -1000;
        public const int Middle = 0;
        public const int After = 1000;
        public const int Last = 10000;
    }
}
