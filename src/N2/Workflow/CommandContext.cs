using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using N2.Edit;

namespace N2.Workflow
{
    public class CommandContext
    {
        public CommandContext(ContentItem data, string userInterface, IPrincipal user)
			: this(data, userInterface, user, new NullBinder<CommandContext>(), new NullValidator<CommandContext>())
        {
        }

		public CommandContext(ContentItem data, string userInterface, IPrincipal user, IBinder<CommandContext> binder, IValidator<CommandContext> validator)
        {
            Content = data;
            Interface = userInterface;
            User = user;
            Binder = binder;
            Validator = validator;
            ValidationErrors = new List<ValidationError>();
            Errors = new List<Exception>();
			Parameters = new Dictionary<string, object>();
        }

        public ContentItem Content { get; set; }
        public string Interface { get; set; }
        public string RedirectTo { get; set; }
        public IPrincipal User { get; set; }

		public IDictionary<string, object> Parameters { get; set; }
		public IValidator<CommandContext> Validator { get; set; }
		public IBinder<CommandContext> Binder { get; set; }

        public ICollection<ValidationError> ValidationErrors { get; set; }
        public ICollection<Exception> Errors { get; set; }

        public override string ToString()
        {
            return "CommandContext {Data=" + Content + ", Interface=" + Interface + ", RedirectTo=" + RedirectTo + "}";
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
