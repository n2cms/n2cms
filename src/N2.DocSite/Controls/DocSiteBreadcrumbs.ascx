<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocSiteBreadcrumbs.ascx.cs" Inherits="N2.DocSite.Controls.DocSiteBreadcrumbs" %>

<div id="docsite_breadcrumbs_content">
  <asp:UpdatePanel runat="server" ID="breadCrumbsUpdatePanel" ChildrenAsTriggers="true">
	  <ContentTemplate>
		  <asp:SiteMapPath runat="server" ID="breadCrumbs" SiteMapProvider="DocSiteContentsSiteMapProvider">
			  <CurrentNodeStyle CssClass="docsite_breadcrumb docsite_breadcrumbs_active" />
			  <NodeStyle CssClass="docsite_breadcrumb docsite_breadcrumbs_link" />
			  <RootNodeStyle CssClass="docsite_breadcrumb docsite_breadcrumbs_root" />
		  </asp:SiteMapPath>
	  </ContentTemplate>
  </asp:UpdatePanel>
</div>