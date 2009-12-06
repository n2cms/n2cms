using System;
using System.Collections.Generic;
using System.Security.Principal;
using N2.Edit;
using N2.Security;
using System.Web;

namespace N2.Workflow
{
    public class CommandContext
    {
        public CommandContext(ContentItem data, string userInterface, IPrincipal user)
            : this(data, userInterface, user, new NullBinder<ContentItem>(), new NullValidator<ContentItem>())
        {
        }

        public CommandContext(ContentItem data, string userInterface, IPrincipal user, IBinder<ContentItem> binder, IValidator<ContentItem> validator)
        {
            Data = data;
            Interface = userInterface;
            User = user;
            Binder = binder;
            Validator = validator;
            ValidationErrors = new List<ValidationError>();
            Errors = new List<Exception>();
        }

        public ContentItem Data { get; set; }
        public string Interface { get; set; }
        public string RedirectTo { get; set; }
        public IPrincipal User { get; set; }
        public Permission Intent { get; set; }

        public IValidator<ContentItem> Validator { get; set; }
        public IBinder<ContentItem> Binder { get; set; }

        public ICollection<ValidationError> ValidationErrors { get; set; }
        public ICollection<Exception> Errors { get; set; }

        public override string ToString()
        {
            return "CommandContext {Data=" + Data + ", Interface=" + Interface + ", Intent=" + Intent + "}";
        }

        public bool ApplyRedirection(HttpResponse response)
        {
            if(!string.IsNullOrEmpty(RedirectTo))
            {
                response.Redirect(RedirectTo);
                return true;
            }
            return false;
        }
    }

}
