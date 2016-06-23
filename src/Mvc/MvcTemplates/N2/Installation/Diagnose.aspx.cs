#region License
/* Copyright (C) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using N2.Definitions;
using N2.Edit.Installation;
using N2.Engine;
using N2.Web;
using N2.Management.Installation;
using N2.Persistence;

namespace N2.Edit.Install
{
    public partial class Diagnose : Page
    {
        protected IHost host;
        protected string[] recentChanges = new string[0];
        protected IEngine Engine;

        protected override void OnInit(EventArgs e)
        {
            Engine = N2.Context.Current;
            InstallationUtility.CheckInstallationAllowed(Context);
            Status = Engine.Resolve<InstallationManager>().GetStatus();
            N2.Resources.Register.JQuery(this);
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            host = N2.Context.Current.Resolve<IHost>();
			Header.DataBind();
            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ShowLastError();

            try
            {
                CheckDatabase();
            }
            catch (Exception ex)
            {
                lblDbConnection.Text = formatException(ex);
            }

            try
            {
                lblRootNode.Text = CurrentInstallationManager.CheckRootItem();
            }
            catch (Exception ex)
            {
                lblRootNode.Text = formatException(ex);
            }

            try
            {
                lblStartNode.Text = CurrentInstallationManager.CheckStartPage();
            }
            catch (Exception ex)
            {
                lblStartNode.Text = formatException(ex);
            }

            try
            {
                //recentChanges = N2.Find.Items.All.MaxResults(5).OrderBy.Updated.Desc.Select().Select(i => i.SavedBy + ": #" + i.ID + " " + i.Title + " (" + i.Updated + ")").ToArray();
				recentChanges = Engine.Resolve<IContentItemRepository>().Find(ParameterCollection.Empty.OrderBy("Updated DESC").Take(5)).Select(i => i.SavedBy + ": #" + i.ID + " " + i.Title + " (" + i.Updated + ")").ToArray();
					//N2.Find.Items.All.MaxResults(5).OrderBy.Updated.Desc.Select().Select(i => i.SavedBy + ": #" + i.ID + " " + i.Title + " (" + i.Updated + ")").ToArray();
            }
            catch (Exception ex)
            {
                lblChanges.Text = formatException(ex);
            }

            try
            {
                rptDefinitions.DataSource = N2.Context.Definitions.GetDefinitions().SelectMany(d => N2.Context.Current.Resolve<ITemplateAggregator>().GetTemplates(d.ItemType).Select(t => t.Definition));
                rptDefinitions.DataBind();
            }
            catch (Exception ex)
            {
                lblDefinitions.Text = formatException(ex);
            }

            try
            {
                rptAssembly.DataSource = N2.Context.Current.Resolve<ITypeFinder>().GetAssemblies();
                rptAssembly.DataBind();
            }
            catch (Exception ex)
            {
                lblAssemblies.Text = formatException(ex);
            }

			ScheduledActions = Engine.Resolve<N2.Plugin.Scheduling.Scheduler>().Actions;
        }

        private void ShowLastError()
        {
            try
            {
                Exception lastError = Server.GetLastError();
                Server.ClearError();
                if (lastError != null)
                    lblError.Text = formatException(lastError);
                else
                    lblError.Text = "none";
            }
            catch (Exception ex)
            {
                lblError.Text = formatException(ex);
            }
        }

        private InstallationManager CurrentInstallationManager
        {
            get { return N2.Context.Current.Resolve<InstallationManager>(); }
        }

        private void CheckDatabase()
        {
            lblDbConnection.Text = CurrentInstallationManager.CheckDatabase();
        }

        private string formatException(Exception ex)
        {
            return
                string.Format("<textarea>{0}\n\n{1}</textarea>", ex.Message, ex.StackTrace);
        }

        /// <summary>
        /// Finds the trust level of the running application (http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)
        /// </summary>
        /// <returns>The current trust level.</returns>
        protected static AspNetHostingPermissionLevel GetTrustLevel()
        {
            foreach (AspNetHostingPermissionLevel trustLevel in new[] { AspNetHostingPermissionLevel.Unrestricted, AspNetHostingPermissionLevel.High, AspNetHostingPermissionLevel.Medium, AspNetHostingPermissionLevel.Low, AspNetHostingPermissionLevel.Minimal })
            {
                try
                {
                    new AspNetHostingPermission(trustLevel).Demand();
                }
                catch (System.Security.SecurityException)
                {
                    continue;
                }

                return trustLevel;
            }

            return AspNetHostingPermissionLevel.None;
        }

        protected IList<ItemDefinition> AllowedChildren(object dataItem)
        {
            ItemDefinition d = dataItem as ItemDefinition;

            return d.GetAllowedChildren(N2.Context.Current.Definitions, Activator.CreateInstance(d.ItemType) as ContentItem).ToList();
        }

        protected string GetDefinitions(Assembly a)
        {
            return string.Join(", ", N2.Context.Definitions.GetDefinitions().Where(d => d.ItemType.Assembly == a).Select(d => d.Title).ToArray());
        }

        protected DatabaseStatus Status { get; set; }

		public IList<Plugin.Scheduling.ScheduledAction> ScheduledActions { get; set; }
	}
}
