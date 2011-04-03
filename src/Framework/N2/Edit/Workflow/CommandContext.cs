using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using N2.Edit;
using N2.Definitions;

namespace N2.Edit.Workflow
{
    public class CommandContext
    {
        public CommandContext(ItemDefinition definition, ContentItem content, string userInterface, IPrincipal user)
			: this(definition, content, userInterface, user, new NullBinder<CommandContext>(), new NullValidator<CommandContext>())
        {
        }

		public CommandContext(ItemDefinition definition, ContentItem content, string userInterface, IPrincipal user, IContentForm<CommandContext> binder, IValidator<CommandContext> validator)
        {
			Definition = definition;
            Content = content;
            Interface = userInterface;
            User = user;
            Binder = binder;
            Validator = validator;
            ValidationErrors = new List<ValidationError>();
            Errors = new List<Exception>();
			Parameters = new Dictionary<string, object>();
        }

		public ContentItem Content { get; set; }
		public ItemDefinition Definition { get; set; }
        public string Interface { get; set; }
        public IPrincipal User { get; set; }

		public IDictionary<string, object> Parameters { get; set; }
		public IValidator<CommandContext> Validator { get; set; }
		public IContentForm<CommandContext> Binder { get; set; }

        public ICollection<ValidationError> ValidationErrors { get; set; }
        public ICollection<Exception> Errors { get; set; }

        public override string ToString()
        {
            return "CommandContext {Data=" + Content + ", Interface=" + Interface + "}";
        }

		public object this[string key]
		{
			get { return Parameters.ContainsKey(key) ? Parameters[key] : null; }
			set { Parameters[key] = value; }
		}
	}

}
