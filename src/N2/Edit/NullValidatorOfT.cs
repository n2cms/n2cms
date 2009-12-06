using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit
{
    public class NullValidator<T> : IValidator<T>
    {
        #region IValidator<T> Members

        public ICollection<ValidationError> Validate(T contentItem)
        {
            return new List<ValidationError>();
        }

        #endregion
    }
}
