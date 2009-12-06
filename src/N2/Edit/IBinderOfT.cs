using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit
{
    public interface IBinder<T>
    {
        bool UpdateObject(T value);

        void UpdateInterface(T value);
    }
}
