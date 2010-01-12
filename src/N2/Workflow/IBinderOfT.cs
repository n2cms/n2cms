
namespace N2.Workflow
{
    public interface IBinder<T>
    {
        bool UpdateObject(T value);

        void UpdateInterface(T value);
    }
}
