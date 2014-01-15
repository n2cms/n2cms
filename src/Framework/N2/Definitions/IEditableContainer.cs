using System;

namespace N2.Definitions
{
    /// <summary>
    /// Attributes implementing this interface can serve as containers for 
    /// editors in edit mode.
    /// </summary>
    public interface IEditableContainer : IContainable, IComparable<IEditableContainer>
    {
    }
}
