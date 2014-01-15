using System;
namespace N2.Persistence
{
    public interface IParameter
    {
        bool IsMatch(object item);
    }
}
