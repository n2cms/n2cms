using System;
using System.ComponentModel;
using N2.Templates.Mvc.Models.Parts;
using N2.Web.Mvc;
using N2.Web.UI;
using System.ComponentModel.DataAnnotations;

namespace N2.Templates.Mvc.Models
{
    public class CommentInputModel : IItemContainer<CommentInput>, IDataErrorInfo
    {
        public CommentInputModel()
        {
        }

        public CommentInputModel(CommentInput currentItem)
        {
            Update(currentItem);
        }

        public string Text { get; set; }

        public string Url { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        [RegularExpression("^$", ErrorMessage = "Clear this must be empty")]
        public string Trap { get; set; }

        #region IDataErrorInfo Members

        public string this[string columnName]
        {
            get { return Validate(columnName); }
        }

        public string Error
        {
            get { return String.Empty; }
        }

        #endregion

        #region IItemContainer<CommentInput> Members

        public CommentInput CurrentItem { get; private set; }

        ContentItem IItemContainer.CurrentItem
        {
            get { return CurrentItem; }
        }

        #endregion

        public CommentInputModel Update(CommentInput currentItem)
        {
            CurrentItem = currentItem;

            return this;
        }

        private string Validate(string propertyName)
        {
            switch (propertyName.ToLower())
            {
                case "title":
                    if (String.IsNullOrEmpty(Title))
                        return "Title cannot be empty";
                    break;
                case "name":
                    if (String.IsNullOrEmpty(Name))
                        return "Name cannot be empty";
                    break;
                case "text":
                    if (String.IsNullOrEmpty(Text))
                        return "Text cannot be empty";
                    break;
                case "email":
                    if (String.IsNullOrEmpty(Email))
                        return "Email cannot be empty";
                    break;
            }
            return String.Empty;
        }
    }
}
