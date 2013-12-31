using System;
using System.Linq;
using System.ComponentModel;
using System.Reflection;
using System.Web.UI;
using N2.Details;

namespace N2.Web.UI.WebControls
{
    /// <summary>
    /// Control used to display content on a web forms page. The control will 
    /// look for an attribute implementing IDisplayable on the property 
    /// referenced by the PropertyName on the display control.
    /// </summary>
    /// <example>
    /// <!-- Displays the page title using the default displayable. -->
    /// &lt;n2:Display PropertyName="Title" runat="server" /&gt;
    /// </example>
    [PersistChildren(false)]
    [ParseChildren(true)]
    public class Display : Control, IItemContainer
    {
        bool isAdded = false;
        private IDisplayable displayable = null;
        private Control displayer;
        ITemplate headerTemplate;
        ITemplate footerTemplate;

        /// <summary>Gets the displayable attribute</summary>
        public IDisplayable Displayable
        {
            get { return displayable ?? (displayable = GetDisplayableAttribute(PropertyName, CurrentItem, SwallowExceptions, Optional)); }
        }

        /// <summary>Gets the control responsible of displaying the detail.</summary>
        public Control Displayer
        {
            get { return displayer; }
        }

        /// <summary>Use the displayer and the values from this path.</summary>
        public string Path
        {
            get { return (string) (ViewState["Path"] ?? string.Empty); }
            set 
            { 
                ViewState["Path"] = value; 
                currentItem = null;
                ReAddDisplayable();
            }
        }

        private void ReAddDisplayable()
        {
            if (isAdded)
            {
                ClearChildViewState();
                Controls.Clear();
                AddDisplayable();
            }
        }

        /// <summary>The name of the property on the content item whose value is displayed with the Display control.</summary>
        public string PropertyName
        {
            get { return (string) ViewState["PropertyName"] ?? ""; }
            set { ViewState["PropertyName"] = value; }
        }

        /// <summary>A format to use for the html output of this displayable. E.g. &lt;span class='something'&gt;{0}&lt;/span&gt;.</summary>
        public string Format
        {
            get { return (string)ViewState["Format"] ?? ""; }
            set { ViewState["Format"] = value; }
        }

        /// <summary>Prevent this control from throwing exceptions when no displayable with the given name on the page.</summary>
        public bool Optional
        {
            get { return (bool)(ViewState["Optional"] ?? true); }
            set { ViewState["Optional"] = value; }
        }

        /// <summary>Controls whether this property can be edited via the the UI when navigating using the drag-drop mode.</summary>
        public bool Editable
        {
            get { return (bool)(ViewState["Editable"] ?? true); }
            set { ViewState["Editable"] = value; }
        }

        /// <summary>Prevent this control from throwing exceptions when any irregularities occurs, e.g. there is no property with the given name on the page.</summary>
        public bool SwallowExceptions
        {
            get { return (bool)(ViewState["SwallowExceptions"] ?? false); }
            set { ViewState["SwallowExceptions"] = value; }
        }

        /// <summary>Inserted before the display control if a control was added.</summary>
        [System.ComponentModel.DefaultValueAttribute((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(SimpleTemplateContainer))]
        public virtual ITemplate HeaderTemplate
        {
            get { return this.headerTemplate; }
            set { this.headerTemplate = value; }
        }

        /// <summary>Added after the display control if a control was added.</summary>
        [System.ComponentModel.DefaultValueAttribute((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(SimpleTemplateContainer))]
        public virtual ITemplate FooterTemplate
        {
            get { return this.footerTemplate; }
            set { this.footerTemplate = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            AddDisplayable();
            isAdded = true;
            base.OnInit(e);
        }

        protected void AddDisplayable()
        {
            if (Site != null && Site.DesignMode)
                return;

            if (Displayable != null)
            {
                displayer = Displayable.AddTo(CurrentItem, PropertyName, this);

                if (displayer != null)
                {
                    if (HeaderTemplate != null)
                    {
                        Control header = new SimpleTemplateContainer();
                        Controls.AddAt(0, header);

                        HeaderTemplate.InstantiateIn(header);
                    }

                    if (FooterTemplate != null)
                    {
                        Control footer = new SimpleTemplateContainer();
                        Controls.Add(footer);

                        FooterTemplate.InstantiateIn(footer);
                    }
                }
            }
        }


        public static IDisplayable GetDisplayableAttribute(string propertyName, ContentItem item, bool swallowExceptions, bool optional)
        {
            if (item == null)
            {
                return ThrowUnless(swallowExceptions, new ArgumentNullException("item"));
            }

            var displayable = N2.Definitions.Static.DefinitionMap.Instance.GetOrCreateDefinition(item).Displayables.FirstOrDefault(d => d.Name == propertyName);
            if (displayable == null)
                return ThrowUnless(swallowExceptions || optional, new N2Exception("No displayable '{0}' found for the item #{1} of type '{2}'.", propertyName, item.ID, item.GetContentType()));
            
            return displayable;
        }

        private static IDisplayable ThrowUnless<T>(bool swallowExceptions, T ex)
            where T : Exception
        {
            if (swallowExceptions)
                return null;
            throw ex;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            using (WebExtensions.GetEditableWrapper(CurrentItem, Editable && ControlPanel.GetState(this).IsFlagSet(ControlPanelState.DragDrop), PropertyName, Displayable, writer))
            {
                string format = Format;
                if (!string.IsNullOrEmpty(format))
                {
                    int index = format.IndexOf("{0}");
                    if (index > 0)
                    {
                        writer.Write(format.Substring(0, index));
                        base.Render(writer);
                        writer.Write(format.Substring(index + 3));
                    }
                    else
                    {
                        writer.Write(format);
                    }
                }
                else
                {
                    base.Render(writer);
                }
            }
        }

        #region IItemContainer Members

        private ContentItem currentItem = null;

        public ContentItem CurrentItem
        {
            get
            {
                if (Site != null && Site.DesignMode)
                    return null;

                if (currentItem == null)
                {
                    currentItem = ItemUtility.FindCurrentItem(Parent) ?? N2.Context.CurrentPage;
                    if (!string.IsNullOrEmpty(Path))
                    {
                        currentItem = ItemUtility.WalkPath(currentItem, Path);
                    }
                }
                return currentItem;
            }
            set
            {
                currentItem = value;
                ReAddDisplayable();
            }
        }

        #endregion
    }
}
