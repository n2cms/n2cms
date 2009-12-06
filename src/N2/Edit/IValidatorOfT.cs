using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit
{
    public interface IValidator<T>
    {
        ICollection<ValidationError> Validate(T contentItem);
    }
}
