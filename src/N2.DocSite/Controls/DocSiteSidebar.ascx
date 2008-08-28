<%@ Control Language="C#" AutoEventWireup="true" Codebehind="DocSiteSidebar.ascx.cs" Inherits="N2.DocSite.Controls.DocSiteSidebar" %>
<%@ Register Src="~/Controls/DocSiteContents.ascx" TagName="DocSiteContents" TagPrefix="DocSite" %>
<%@ Register Src="~/Controls/DocSiteIndex.ascx" TagName="DocSiteIndex" TagPrefix="DocSite" %>

<div id="sidebar_button_container">
	<div class="sidebar_button" id="sidebar_contents_button" title="Table of Contents"
	onclick="selectDocsiteSidebarButton(document.getElementById('sidebar_contents_button'), document.getElementById('docsite_toc'));
	<%= "document.getElementById('" + selectedButtonHiddenField.ClientID + "').value = 'sidebar_contents_button'" %>;">
		Contents
	</div>
	<div class="sidebar_button" id="sidebar_index_button" title="Help Index"
	onclick="selectDocsiteSidebarButton(document.getElementById('sidebar_index_button'), document.getElementById('docsite_index'));
	<%= "document.getElementById('" + selectedButtonHiddenField.ClientID + "').value = 'sidebar_index_button'" %>;">
		Index
	</div>
</div>
<div id="sidebar_contents">
	<div class="sidebar_content_hidden" id="docsite_toc">
		<DocSite:DocSiteContents id="contentsControl" runat="server" OnSelectedTopicChanged="contentsControl_SelectedTopicChanged" />
	</div>
	<div class="sidebar_content_hidden" id="docsite_index">
		<DocSite:DocSiteIndex id="indexControl" runat="server" OnSelectedHelpFileChanged="indexControl_SelectedHelpFileChanged" />
	</div>
</div>

<asp:HiddenField runat="server" ID="selectedButtonHiddenField" />
