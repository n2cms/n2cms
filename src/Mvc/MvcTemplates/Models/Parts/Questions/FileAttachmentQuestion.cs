using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Details;

namespace N2.Templates.Mvc.Models.Parts.Questions
{
    [PartDefinition("File attachment")]
    public class FileAttachmentQuestion : Question
    {

        [EditableNumber("Max file size (kB)", 100)]
        public virtual int MaxFileSize { get; set; }

        [EditableText("Allowed file extensions", 100, HelpText = "Separate multiple extensions with comma (,)")]
        public virtual string AllowedFileExtensions { get; set; }
        public override System.Web.Mvc.MvcHtmlString CreateHtmlElement()
        {
            TagBuilder tb = new TagBuilder("input");
            tb.Attributes["type"] = "file";
            tb.Attributes["name"] = ElementID;
            return MvcHtmlString.Create(tb.ToString(TagRenderMode.SelfClosing));
        }

        public override void AppendAnswer(Details.AnswerContext context, string postedValue)
        {
            var file = context.HttpContext.Request.Files[ElementID];
            if (file == null || file.ContentLength == 0 || string.IsNullOrEmpty(file.FileName))
            {
                return;
            }

            if (MaxFileSize != 0 && file.ContentLength > MaxFileSize * 1024)
            {
                context.ValidationErrors.Add(ElementID, QuestionText + " is too big");
                return;
            }

            string fileName = System.IO.Path.GetFileName(file.FileName);
            if (!AllowedFileExtensions.Split(',').Any(ext => fileName.EndsWith(ext.Trim(), StringComparison.InvariantCultureIgnoreCase)))
            {
                context.ValidationErrors.Add(ElementID, QuestionText + " is not an allowed file type");
                return;
            }

            context.Attachments.Add(new System.Net.Mail.Attachment(file.InputStream, fileName));
        }
    }
}
