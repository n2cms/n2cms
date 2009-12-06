
namespace N2.Edit
{
    public class NullBinder<T> : IBinder<T>
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
