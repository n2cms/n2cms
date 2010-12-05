using System;
using System.ComponentModel;
using N2.Templates.Mvc.Areas.Blog.Models.Pages;
using N2.Web.Mvc;
using N2.Web.UI;

namespace N2.Templates.Mvc.Areas.Blog.Models
{
    public class BlogCommentInputModel : IItemContainer<BlogPost>, IDataErrorInfo
    {
        public BlogCommentInputModel()
        {
        }

        public BlogCommentInputModel(BlogPost currentItem)
        {
            Update(currentItem);
        }

        public string Text { get; set; }

        public string Url { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

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

        #region IItemContainer<BlogPost> Members

        public BlogPost CurrentItem { get; private set; }

        ContentItem IItemContainer.CurrentItem
        {
            get { return CurrentItem; }
        }

        #endregion

        public BlogCommentInputModel Update(BlogPost currentItem)
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