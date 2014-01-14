namespace N2.Web
{
    public interface IRequestContextAccessor
    {
        object Get(object key);
        void Set(object key, object instance);
    }
}
