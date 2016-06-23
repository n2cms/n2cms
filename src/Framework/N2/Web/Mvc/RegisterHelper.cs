using System;
using System.Web.Mvc;
using N2.Definitions;
using N2.Definitions.Runtime;
using N2.Web.Mvc.Html;
using N2.Details;
using N2.Engine;

namespace N2.Web.Mvc
{
    public class RegisterHelper : IContentRegistration
    {
        #region Static
        public static RegisteringDisplayRendererFactory RendererFactory { get; set; }

        static RegisterHelper()
        {
            RendererFactory = new RegisteringDisplayRendererFactory();
        }
        #endregion

        public HtmlHelper Html { get; set; }
        
        string ContainerName { get; set; }

		Logger<RegisterHelper> logger;

        public RegisterHelper(HtmlHelper html)
        {
            this.Html = html;
        }


        public Builder<T> RegisterDisplayable<T>(string name) where T : class, IDisplayable, new()
        {
            var re = RegistrationExtensions.GetRegistrationExpression(Html);
            if (re != null)
            {
                var displayable = re.Definition.Displayables[name] as T ?? new T { Name = name };
				logger.DebugFormat("RegisterDisplayable<{0}> {1} with name {2}", typeof(T).Name, displayable, name);
				re.Add(displayable);
            }

            return new Builder<T>(name, re);
        }
        public Builder<T> RegisterDisplayable<T>(T displayable) where T : IDisplayable
        {
            var re = RegistrationExtensions.GetRegistrationExpression(Html);
            if (re != null)
			{
				logger.DebugFormat("RegisterDisplayable<{0}> {1} with name {2}", typeof(T).Name, displayable, displayable.Name);
				re.Add(displayable);
			}
            return new Builder<T>(displayable.Name, re);
        }

        #region IContentRegistration Members

        public EditableBuilder<T> RegisterEditable<T>(string name, string title) where T : class, IEditable, new()
        {
            var re = RegistrationExtensions.GetRegistrationExpression(Html);
            if (re != null)
            {
                var editable = re.Definition.Editables[name] as T ?? new T();
				logger.DebugFormat("RegisterEditable<{0}> {1} with name {2}", typeof(T).Name, editable, editable.Name);
                re.Add(editable, name, title);
            }

            return RendererFactory.Create<T>(Rendering.RenderingContext.Create(Html, name, isEditable: Html.GetControlPanelState().IsFlagSet(UI.WebControls.ControlPanelState.DragDrop)), re);
        }

        public EditableBuilder<T> RegisterEditable<T>(T editable) where T : IEditable
        {
            var re = RegistrationExtensions.GetRegistrationExpression(Html);
            if (re != null)
            {
				logger.DebugFormat("RegisterEditable<{0}> {1} with name {2}", typeof(T).Name, editable, editable.Name);
				re.Add(editable, editable.Name, editable.Title);
            }

            return RendererFactory.Create<T>(Rendering.RenderingContext.Create(Html, editable.Name), re);
        }


        public Builder<T> Register<T>(T editable) where T : IUniquelyNamed
        {
            var re = RegistrationExtensions.GetRegistrationExpression(Html);
            if (re != null)
            {
				logger.DebugFormat("Register<{0}> {1} with name {2}", typeof(T).Name, editable, editable.Name);
				re.Add(editable);
            }

            return new Builder<T>(editable.Name, re);
        }

        public void RegisterModifier(Details.IContentTransformer modifier)
        {
            var re = RegistrationExtensions.GetRegistrationExpression(Html);
            if (re != null)
			{
				logger.DebugFormat("RegisterModifier {0}", modifier);
				re.Definition.ContentTransformers.Add(modifier);
			}
        }

        public Builder<T> RegisterRefiner<T>(T refiner) where T : ISortableRefiner
        {
            var re = RegistrationExtensions.GetRegistrationExpression(Html);
			if (re != null)
			{
				logger.DebugFormat("RegisterRefiner<{0}> {1}", typeof(T).Name, refiner);
				return re.RegisterRefiner<T>(refiner);
			}
            return new Builder<T>(null);
        }

        #endregion

        public IDisposable BeginContainer(string containerName)
        {
            this.ContainerName = containerName;
			logger.DebugFormat("BeginContainer {0}", containerName);
			return new ResetOnDispose { PreviousContainerName = ContainerName, Helper = this };
        }

        public void EndContainer()
        {
			logger.DebugFormat("EndContainer {0}", ContainerName);
			this.ContainerName = null;
        }

        #region class ResetOnDispose<T>
        class ResetOnDispose : IDisposable
        {
            public string PreviousContainerName { get; set; }
            public RegisterHelper Helper { get; set; }

            #region IDisposable Members

            public void Dispose()
            {
                Helper.ContainerName = PreviousContainerName;
            }

            #endregion
        }
        #endregion

        #region class RegisteringDisplayRendererFactory
        public class RegisteringDisplayRendererFactory
        {
            public virtual RegisteringDisplayRenderer<T> Create<T>(Rendering.RenderingContext context, ContentRegistration re) where T : IEditable
            {
                return new RegisteringDisplayRenderer<T>(context, re);
            }
        }
        #endregion
    }
}
