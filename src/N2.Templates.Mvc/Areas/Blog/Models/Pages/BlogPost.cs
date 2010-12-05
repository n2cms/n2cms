using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using N2.Collections;
using N2.Details;
using N2.Edit.FileSystem;
using N2.Integrity;
using N2.Templates.Mvc.Models.Pages;
using N2.Templates.Mvc.Models.Parts;
using N2.Templates.Mvc.Services;
using N2.Web.Drawing;
using N2.Web.Mvc;
using N2.Web.UI;
using N2.Persistence.Serialization;
using N2.Definitions;

namespace N2.Templates.Mvc.Areas.Blog.Models.Pages
{
    [PageDefinition("Blog Post", Description = "A blog posting.", SortOrder = 155,
        IconUrl = "~/N2/Resources/icons/script_edit.png")]
    [RestrictParents(typeof(BlogPostContainer))]
    [TabContainer("PostSettings", "Post Settings", 5)]
    [FieldSetContainer("CommentSettings", "Comment Settings", 105, ContainerName = "PostSettings")]
	public class BlogPost : BlogBase, ISyndicatable
    {
        public BlogPost()
        {
            Visible = false;
            Tags = "uncategorized";
        }

        [EditableFileUpload("Title Image", 35, ContainerName = Tabs.Content)]
        public virtual string ImageUrl
        {
            get;
            set;
        }

        [EditableTextBox("Image Caption", 36, ContainerName = Tabs.Content)]
        public virtual string ImageCaption
        {
            get;
            set;
        }

        [EditableTextBox("Author <br><span style=\"font-size: 10px; font-style: italic; color: #666666;\">(Defaults to current user)</span>", 90, ContainerName = Tabs.Content)]
        public virtual string Author
        {
            get { return (string)(GetDetail("Author") ?? SavedBy); }
            set { SetDetail("Author", value, string.Empty); }
        }

        [EditableFreeTextArea("Text", 100, ContainerName = Tabs.Content)]
        public override string Text
        { get; set; }

        [EditableTextBox("Tags <br><span style=\"font-size: 10px; font-style: italic; color: #666666;\">(Comma Delimited)</span>", 110,
            ContainerName = Tabs.Content)]
        public virtual string Tags
        {
            get { return (string)(GetDetail("Tags") ?? string.Empty); }
            set
            {
                string val = !string.IsNullOrEmpty(value.Trim()) ? value : "uncategorized";
                SetDetail("Tags", val.ToLower());
            }
        }

        [EditableCheckBox("", 10,
            CheckBoxText = "Enable Comments",
            ContainerName = "CommentSettings",
            DefaultValue = true)]
        public virtual bool EnableComments
        {
            get;
            set;
        }

        [EditableCheckBox("", 15,
            CheckBoxText = "Show Comments",
            ContainerName = "CommentSettings",
            DefaultValue = true)]
        public virtual bool ShowComments
        {
            get;
            set;
        }

        [DisplayableLiteral()]
        public virtual string Introduction
        {
            get
            {
                return Text.Split(new string[] { "<!--more-->" }, 2, System.StringSplitOptions.None)[0];
            }
            set { SetDetail("Introduction", value, string.Empty); }
        }

        public virtual string GetResizedImageUrl(IFileSystem fs)
        {
            return GetReizedUrl(fs, "blog");
        }

        private string GetReizedUrl(IFileSystem fs, string imageSize)
        {
            string resizedUrl = ImagesUtility.GetResizedPath(ImageUrl, imageSize);
            if (fs.FileExists(resizedUrl))
                return resizedUrl;
            return ImageUrl;
        }

        public bool IsSummarized
        {
            get
            {
                return Text.Split(new string[] { "<!--more-->" }, 2, System.StringSplitOptions.None).Count() > 1;
            }
        }

        public IList<BlogComment> Comments
        {
            get { return GetChildren(new TypeFilter(typeof(BlogComment))).OfType<BlogComment>().ToList(); }
        }

        public int GetNextCommentID()
        {
            return Comments.Count > 0 ? Comments.Max(c => c.CommentID) + 1 : 1;
        }

        string ISyndicatable.Summary
        {
            get { return Introduction; }
        }


        public bool Syndicate
        {
            get;
            set;
        }
    }
}