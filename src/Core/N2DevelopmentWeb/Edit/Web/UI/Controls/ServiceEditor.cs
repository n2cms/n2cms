using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using N2.Edit.Settings;
using System.Collections.Generic;
using N2.Definitions;

namespace N2.Edit.Web.UI.Controls
{
	public class ServiceEditor : WebControl
	{
		#region Fields
		private Engine.IEngine engine;
		private IDictionary<string, Control> editors;		
		#endregion

		#region Constructor
		public ServiceEditor()
		{
			this.CssClass = "serviceEditor";
			this.Engine = N2.Context.Current;
		} 
		#endregion

		#region Properties
		public virtual Engine.IEngine Engine
		{
			get { return engine; }
			set { engine = value; }
		}

		/// <summary>Gets a dictionary of editor controls added this control.</summary>
		public IDictionary<string, Control> Editors
		{
			get { return editors; }
		}

		public virtual Settings.ISettingsProvider Settings
		{
			get { return this.Engine.Resolve<Settings.ISettingsProvider>(); }
		}
		
		#endregion

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.EnsureChildControls();
		}

		protected override void CreateChildControls()
		{
			AddEditors();
			if (!Page.IsPostBack)
			{
				InitEditors();
			}
			base.CreateChildControls();
		}

		protected virtual void AddEditors()
		{
			editors = new Dictionary<string, Control>();
			IEditableContainer rootContainer = this.Settings.RootContainer;
			AddEditorsRecursive(this, rootContainer);
			//AddValidatorsToPageRecursive(this);
		}

		protected virtual void InitEditors()
		{
			foreach (N2.Edit.Settings.IServiceEditable editable in Settings.Settings)
			{
				editable.UpdateEditor(Engine, Editors[GetDictionaryKey(editable)]);
			}
		}


		protected virtual Control CreateContainer(Control container)
		{
			HtmlGenericControl detailContainer = new HtmlGenericControl("div");
			detailContainer.Attributes["class"] = "editDetail";
			container.Controls.Add(detailContainer);
			return detailContainer;
		}

		/// <summary>Adds editors and containers to the supplied container.</summary>
		/// <param name="container">The control on which editors and containres will be added.</param>
		/// <param name="contained">The definition that will add a control in the container.</param>
		protected virtual void AddEditorsRecursive(Control container, Definitions.IContainable contained)
		{
			Control added = contained.AddTo(CreateContainer(container));

			IServiceEditable editable = contained as IServiceEditable;
			if (editable != null)
				this.Editors[GetDictionaryKey(editable)] = added;


			Definitions.IEditableContainer subContainer = contained as Definitions.IEditableContainer;
			if (subContainer != null)
				foreach (Definitions.IContainable subContained in subContainer.GetContained(this.Page.User))
					AddEditorsRecursive(added, subContained);
		}

		///// <summary>Adds validators to the current page's validator collection.</summary>
		///// <param name="c">The container control whose validators are added.</param>
		//protected virtual void AddValidatorsToPageRecursive(Control validatorContainer)
		//{
		//    if (validatorContainer is IValidator)
		//        this.Page.Validators.Add((IValidator)validatorContainer);
		//    foreach (Control childControl in validatorContainer.Controls)
		//        AddValidatorsToPageRecursive(childControl);
		//}

		public void Save()
		{
			EnsureChildControls();

			foreach (IServiceEditable editable in Settings.Settings)
			{
				editable.UpdateService(Engine, Editors[GetDictionaryKey(editable)]);
			}
		}

		private static string GetDictionaryKey(N2.Edit.Settings.IServiceEditable editable)
		{
			return editable.Name + "@" + editable.ServiceName;
		}
	}
}
