using System.Collections.Generic;

namespace N2.Workflow
{
    public interface IValidator<T>
    {
        ICollection<ValidationError> Validate(T contentItem);
    }
}
