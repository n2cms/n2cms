using System;

namespace N2.Engine
{
    public interface IDecorator<T>
    {
        T Component { get; }
    }
}
