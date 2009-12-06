using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Edit
{
    public class PageValidator<T> : IValidator<T>
    {
        Page page;
        string validationGroup;

        public PageValidator(Page page)
        {
            this.page = page;
        }

        public PageValidator(Page page, string validationGroup)
            : this(page)
        {
            this.validationGroup = validationGroup;
        }

        #region IValidator<T> Members

        public ICollection<ValidationError> Validate(T contentItem)
        {
            if (string.IsNullOrEmpty(validationGroup))
                page.Validate();
            else
                page.Validate(validationGroup);

            var errors = new List<ValidationError>();
            if (!page.IsValid)
                foreach (BaseValidator validator in page.Validators)
                    errors.Add(new ValidationError(validator.ID, validator.ErrorMessage));

            return errors;
        }

        #endregion
    }
}
