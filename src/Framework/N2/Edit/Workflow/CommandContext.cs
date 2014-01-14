using System;
using System.Collections.Generic;
using System.Security.Principal;
using N2.Definitions;

namespace N2.Edit.Workflow
{
    /// <summary>
    /// Expose information about a saving or publishing process.
    /// </summary>
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
            Original = content.VersionOf.Value ?? content.Clone(false);
            Interface = userInterface;
            User = user;
            Binder = binder;
            Validator = validator;
            ValidationErrors = new List<ValidationError>();
            Errors = new List<Exception>();
            Parameters = new Dictionary<string, object>();
        }

        /// <summary>The unchanged data before command processed.</summary>
        public ContentItem Original { get; set; }

        /// <summary>The content being processed.</summary>
        public ContentItem Content { get; set; }

        /// <summary>The definition of the saved item.</summary>
        public ItemDefinition Definition { get; set; }

        /// <summary>The interface doing the changes.</summary>
        public string Interface { get; set; }

        /// <summary>The user doing the changes.</summary>
        public IPrincipal User { get; set; }

        /// <summary>Context parameters passed on throughout the process.</summary>
        public IDictionary<string, object> Parameters { get; set; }

        /// <summary>The validator is used by a command to validate content.</summary>
        public IValidator<CommandContext> Validator { get; set; }

        /// <summary>The binder pass data to and from the content item.</summary>
        public IContentForm<CommandContext> Binder { get; set; }

        /// <summary>Validations errors detected during the process.</summary>
        public ICollection<ValidationError> ValidationErrors { get; set; }

        /// <summary>Exceptions occurred during the process.</summary>
        public ICollection<Exception> Errors { get; set; }

        public override string ToString()
        {
            return "CommandContext {Data=" + Content + ", Interface=" + Interface + "}";
        }

        /// <summary>Gets or sets a value from the parameters dictionary.</summary>
        /// <param name="key">The ke of the value to get.</param>
        /// <returns>The value or null if no value exists.</returns>
        public object this[string key]
        {
            get { return Parameters.ContainsKey(key) ? Parameters[key] : null; }
            set { Parameters[key] = value; }
        }

        public string RedirectUrl { get; set; }
    }

}
