using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Collections;
using N2.Engine;
using N2.Web.Parts;
using N2.Edit;

namespace N2.Web.UI.WebControls
{
    /// <summary>
    /// Adds sub-items with a certain zone name to a page.
    /// </summary>
    [PersistChildren(false)]
    [ParseChildren(true)]
    public class Zone : ItemAwareControl
    {
        public const string PageKey = "n2.zones";
            
        IEnumerable<ItemFilter> filters;
        bool isDataBound = false;
        IList<ContentItem> items = null;
        ITemplate separatorTemplate;
        ITemplate headerTemplate;
        ITemplate footerTemplate;
        PartsAdapter partsAdapter;

        /// <summary>The content adapter related to the current page item.</summary>
        protected virtual PartsAdapter PartsAdapter
        {
            get { return partsAdapter ?? (partsAdapter = Engine.Resolve<IContentAdapterProvider>().ResolveAdapter<PartsAdapter>(CurrentItem)); }
        }

        /// <summary>Gets or sets the zone from which to featch items.</summary>
        public string ZoneName
        {
            get { return (string)ViewState["ZoneName"] ?? ""; }
            set 
            { 
                ViewState["ZoneName"] = value;
                OnZoneNameChanged();
            }
        }

        /// <summary>Gets or sets an enumeration of filters applied to the items.</summary>
        public IEnumerable<ItemFilter> Filters
        {
            get { return filters; }
            set { filters = value; }
        }

        /// <summary>Gets or sets a list of items to display.</summary>
        public virtual IList<ContentItem> DataSource
        {
            get { return items ?? (items = GetItems()); }
            set
            {
                items = value;
                isDataBound = false;
            }
        }

        public override ContentItem CurrentItem
        {
            get
            {
                return base.CurrentItem;
            }
            set
            {
                base.CurrentItem = value;

                if (ChildControlsCreated)
                {
                    ChildControlsCreated = false;
                    EnsureChildControls();
                }
            }
        }
        
        // templates

        /// <summary>Inserted between added child items.</summary>
        [DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(SimpleTemplateContainer))]
        public virtual ITemplate SeparatorTemplate
        {
            get { return this.separatorTemplate; }
            set { this.separatorTemplate = value; }
        }

        /// <summary>Inserted before the zone control if a control was added.</summary>
        [DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(SimpleTemplateContainer))]
        public virtual ITemplate HeaderTemplate
        {
            get { return this.headerTemplate; }
            set { this.headerTemplate = value; }
        }

        /// <summary>Added after the zone control if a control was added.</summary>
        [DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(SimpleTemplateContainer))]
        public virtual ITemplate FooterTemplate
        {
            get { return this.footerTemplate; }
            set { this.footerTemplate = value; }
        }

        // control overrides

        protected override void OnInit(EventArgs e)
        {
            RegisterZone();
            EnsureChildControls();

            base.OnInit(e);
        }

        protected virtual void OnZoneNameChanged()
        {
            if(ChildControlsCreated)
            {
                ChildControlsCreated = false;
                DataSource = null;
                EnsureChildControls();
            }
        }

        protected virtual void RegisterZone()
        {
            IList<Zone> zones = Page.Items[PageKey] as IList<Zone>;
            if (zones == null)
                Page.Items[PageKey] = zones = new List<Zone>();
            zones.Add(this);
        }

        protected override void OnDataBinding(EventArgs e)
        {
            if (!isDataBound)
                EnsureChildControls();

            base.OnDataBinding(e);
        }

        private ItemList GetItems()
        {
            ItemListEventArgs args = new ItemListEventArgs(null);
            OnSelecting(args);

            if (CurrentItem != null && args.Items == null)
                args.Items = new ItemList(PartsAdapter.GetParts(CurrentItem, ZoneName, GetInterface()));

            OnSelected(args);
            OnFiltering(args);
            return args.Items;
        }

        protected virtual string GetInterface()
        {
            return Interfaces.Viewing;
        }

        protected virtual void OnSelecting(ItemListEventArgs args)
        {
            if (Selecting != null)
                Selecting.Invoke(this, args);
        }

        protected virtual void OnSelected(ItemListEventArgs args)
        {
            if (Selected != null)
                Selected.Invoke(this, args);
        }

        private void OnFiltering(ItemListEventArgs args)
        {
            if (Filtering != null)
                Filtering.Invoke(this, args);

            if (Filters != null)
                foreach (ItemFilter filter in Filters)
                    filter.Filter(args.Items);
        }

        protected override void CreateChildControls()
        {
            CreateItems(this);
            base.CreateChildControls();
        }

        protected virtual void CreateItems(Control container)
        {
            if (DataSource != null)
            {
                container.Controls.Clear();
                bool firstPass = true;
                foreach (ContentItem item in DataSource)
                {
                    if (firstPass)
                    {
                        firstPass = false;
                        AppendTemplate(HeaderTemplate, container);
                    }
                    else if (SeparatorTemplate != null)
                        AppendTemplate(SeparatorTemplate, container);

                    AddChildItem(container, item);
                }
                if(!firstPass)
                    AppendTemplate(FooterTemplate, container);

                isDataBound = true;
                ChildControlsCreated = true;
            }
        }

        private void AppendTemplate(ITemplate template, Control container)
        {
            if (template != null)
            {
                PlaceHolder ph = new PlaceHolder();
                container.Controls.Add(ph);
                template.InstantiateIn(ph);
            }
        }

        protected virtual void AddChildItem(Control container, ContentItem item)
        {
            if (AddingChild != null)
                AddingChild.Invoke(this, new ItemEventArgs(item));

            Control addedControl;
            if (GettingItemTemplate != null)
            {
                TemplateUrlEventArgs args = new TemplateUrlEventArgs(item);
                GettingItemTemplate(this, args);
                if (string.IsNullOrEmpty(args.TemplateUrl))
                    addedControl = PartsAdapter.AddChildPart(item, container);
                else
                    addedControl = PartsAdapter.AddUserControl(args.TemplateUrl, container, args.Item);
            }
            else
                addedControl = PartsAdapter.AddChildPart(item, container);

            if (AddedItemTemplate != null)
                AddedItemTemplate.Invoke(this, new ControlEventArgs(addedControl));
        }

        [Obsolete("The event is obsolete and may be removed in the future. Please note that using this event prevents the new default behaviour which is relying on the PartsAdapter.")]
        public event EventHandler<TemplateUrlEventArgs> GettingItemTemplate;
        public event EventHandler<ControlEventArgs> AddedItemTemplate;
        public event EventHandler<ItemEventArgs> AddingChild;
        public event EventHandler<ItemListEventArgs> Selecting;
        public event EventHandler<ItemListEventArgs> Selected;
        public event EventHandler<ItemListEventArgs> Filtering;

    }
}
