using System.Collections.Generic;
using System.Web.UI;
using N2.Edit.Workflow;

namespace N2.Edit.Web
{
    /// <summary>Validates using the web forms validators on the page</summary>
    /// <typeparam name="T">The type of model object (ignored)</typeparam>
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
                foreach (IValidator validator in page.Validators)
                    errors.Add(new ValidationError(Utility.GetProperty(validator, "ID") as string ?? "error", validator.ErrorMessage));

            return errors;
        }

        #endregion
    }
}
