
namespace N2.Edit.Workflow
{
    public interface IBinder<T>
    {
        bool UpdateObject(T value);

        void UpdateInterface(T value);
    }
}
