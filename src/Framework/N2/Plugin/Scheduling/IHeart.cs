using System;

namespace N2.Plugin.Scheduling
{
    /// <summary>
    /// Interface of a timer wrapper that beats at a certain interval.
    /// </summary>
    public interface IHeart
    {
        event EventHandler Beat;
    }
}
