using N2.Edit.Workflow;

namespace N2.Edit
{
    /// <summary>
    /// Doesn't bind anything but reports the object was updated.
    /// </summary>
    /// <typeparam name="T">The type of model object (ignored)</typeparam>
    public class NullBinder<T> : IContentForm<T>
    {
        #region IBinder<ContentItem> Members

        public bool UpdateObject(T value)
        {
            return true;
        }

        public void UpdateInterface(T value)
        {
        }

        #endregion
    }
}
