using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using N2.Collections;
using N2.Persistence;
using System.Web.UI.WebControls;

namespace N2.Web.UI.WebControls
{
	/// <summary>
	/// Adds sub-items with a certain zone name to a page.
	/// </summary>
    [PersistChildren(false)]
    [ParseChildren(true)]
    public class Zone : ItemAwareControl
	{
		private IEnumerable<ItemFilter> filters;
		private bool isDataBound = false;
		private IList<ContentItem> items = null;
        private ITemplate separatorTemplate;
        private ITemplate headerTemplate;
        private ITemplate footerTemplate;

		/// <summary>Gets or sets the zone from which to featch items.</summary>
		public string ZoneName
		{
			get { return (string) ViewState["ZoneName"] ?? ""; }
			set { ViewState["ZoneName"] = value; }
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
			get
			{
				if (items == null)
				{
					items = GetItems();
				}
				return items;
			}
			set
			{
				items = value;
				isDataBound = false;
			}
		}
	    
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


		protected override void OnInit(EventArgs e)
		{
			EnsureChildControls();

			base.OnInit(e);
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
				args.Items = CurrentItem.GetChildren(ZoneName);

			OnSelected(args);
			OnFiltering(args);
			return args.Items;
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

			string templateUrl = GetTemplateUrl(item);

			Control templateItem = ItemUtility.AddUserControl(container, item, templateUrl);

			if (AddedItemTemplate != null)
				AddedItemTemplate.Invoke(this, new ControlEventArgs(templateItem));
		}

		/// <summary>This event is invoked before when retrieving the user control template used to display a certain item. This is a place where the template can be changed.</summary>
		public event EventHandler<TemplateUrlEventArgs> GettingItemTemplate;

		/// <summary>Gets the user control path which will display a certain item. This method can return an alternative templates if the <see cref="GettingItemTemplate"/> is bound.</summary>
		/// <param name="item">The item which template we want to get.</param>
		/// <returns>An url to the template that should display the item.</returns>
		public virtual string GetTemplateUrl(ContentItem item)
		{
			TemplateUrlEventArgs args = new TemplateUrlEventArgs(item);
			if (GettingItemTemplate != null)
				GettingItemTemplate.Invoke(this, args);

			return args.TemplateUrl;
		}

		public event EventHandler<ItemEventArgs> AddingChild;
		public event EventHandler<ControlEventArgs> AddedItemTemplate;
		public event EventHandler<ItemListEventArgs> Selecting;
		public event EventHandler<ItemListEventArgs> Selected;
        public event EventHandler<ItemListEventArgs> Filtering;

	}
}