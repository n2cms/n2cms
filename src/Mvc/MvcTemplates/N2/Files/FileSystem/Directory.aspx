<%@ Page Language="C#" MasterPageFile="../../Content/Framed.Master" AutoEventWireup="true" CodeBehind="Directory.aspx.cs" Inherits="N2.Edit.FileSystem.Directory1" %>
<%@ Register TagPrefix="edit" TagName="FileUpload" Src="FileUpload.ascx" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<%@ Import Namespace="N2.Web" %>

<asp:Content ContentPlaceHolderID="Head" runat="server">
	<%--Upgrading default version of bootstrap breaks the cms layout. Newer version added as needed.--%>
	<link href="//maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css" type="text/css" rel="stylesheet" />
</asp:Content>

<asp:Content ContentPlaceHolderID="Toolbar" runat="server">
	<edit:ButtonGroup runat="server" CssClass="btn btn-danger">
		<asp:LinkButton ID="btnDelete" runat="server" Text="Delete selected" CssClass="command primary-action" OnCommand="OnDeleteCommand" OnClientClick="return confirm('Delete selected files and folders?');" meta:resourceKey="btnDelete" />
        <asp:LinkButton ID="btnAdd" runat="server" Text="Add selected" CssClass="command primary-action" OnCommand="OnAddCommand" OnClientClick="return confirm('Add selected files?');" meta:resourceKey="btnAdd" Visible="false"/>
		<asp:HyperLink ID="hlEdit" runat="server" Text="Edit" CssClass="command edit" meta:resourceKey="hlEdit" />
        <asp:HyperLink ID="hlCancel" runat="server" Text="Close" CssClass="btn" meta:resourceKey="hlCancel" Visible="false"/>
        
	</edit:ButtonGroup>
</asp:Content>
<asp:Content ContentPlaceHolderID="Content" runat="server">
	<h1 style="float:left;margin-top:0;"><% foreach (N2.ContentItem node in ancestors) {
                var url = Url.Parse("Directory.aspx").AppendSelection(node);
        %>/<a href="<%= string.Format("{0}{1}{2}", url, string.IsNullOrEmpty(ParentQueryString) ? "" : url.ToString().Contains("?") ? "&" : "?", ParentQueryString) %>"><%= node.Title %></a><% } %></h1>
	<span class="input-group-btn" style="text-align:right;padding:0 10px 10px 0;">
        <button id="btn-view-grid" class="btn btn-default" type="button" title="Grid View"><span class="glyphicon glyphicon-th"></span></button>
        <button id="btn-view-list" class="btn btn-default" type="button" title="List View"><span class="glyphicon glyphicon-list"></span></button>
		<script>
			$(function () {
				$("#btn-view-grid").click(function (e) {
					e.preventDefault();
					$("#directory-container").removeClass('upload-folder');
				});
				$("#btn-view-list").click(function (e) {
					e.preventDefault();
					$("#directory-container").addClass('upload-folder');
				});
			});
		</script>
    </span>
	<div style="clear:both;"></div>
	<div class="tabPanel" data-flag="Unclosable">
        <edit:PermissionPanel id="ppPermitted" RequiredPermission="Write" runat="server" meta:resourceKey="ppPermitted">
			<edit:FileUpload runat="server" />
		</edit:PermissionPanel>
        <div id="directory-container" class="directory cf">
            <a href ="<%= GetEditUrl() %>">
            <div data-i="0" class="file create-new-folder">
                <span class="file-ic glyphicon glyphicon-folder-plus"></span>
                <label>Create a New Folder</label>
            </div>
            </a>
		    <asp:Repeater ID="rptDirectories" runat="server">
			    <ItemTemplate>
				    <div class="file">
					    <label>
                            <%if (!IsMultiUpload && IsAllowed) { %>
						        <input name="directory" value="<%# Eval("Path") %>" type="checkbox" />
                            <%} %>
						    <asp:Image ImageUrl='<%# Eval("IconUrl") %>' runat="server" />
					    </label>
						<edit:ItemLink DataSource="<%# Container.DataItem %>" InterfaceUrl="Directory.aspx" ParentQueryString='<%# ParentQueryString %>' runat="server" />
				    </div>
			    </ItemTemplate>
		    </asp:Repeater>
		
		    <asp:Repeater ID="rptFiles" runat="server">
			    <ItemTemplate>
				    <div class="file">
					    <label style='<%# ImageBackgroundStyle((string)Eval("LocalUrl")) %>'>
                            <%if (IsMultiUpload || IsAllowed) { %>
                                <input name="file" value="<%# Eval("LocalUrl") %>" type="checkbox" />
                            <% } %>
					    </label>
						<edit:ItemLink DataSource="<%# Container.DataItem %>" InterfaceUrl="File.aspx" runat="server" />
				    </div>
			    </ItemTemplate>
		    </asp:Repeater>
	    </div>

    </div>

<style>
    .create-new-folder {
    position:relative;
}
.create-new-folder label {
    position: absolute;
    bottom: 0;
    left: 0;
    right: 0;
    background: none !important;
    display: block;
    margin: 0;
    font-weight: bold;
    font-size: 10px;
    height:25px !important;
    text-align:center;
}
.file .file-ic {
    position: absolute;
    top: 25px;
    left: 0;
    right: 0;
    text-align: center;
    font-size: 25px;
}
.glyphicon-folder-plus {
    background: url('/N2/Resources/img/folder_plus.gif') no-repeat center center;
    height: 30px;
}

.glyphicon {
    position: relative;
    top: 1px;
    display: inline-block;
    font-family: 'Glyphicons Halflings';
    font-style: normal;
    font-weight: 400;
    line-height: 1;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
}

.file.create-new-folder {
    background: #a1d4fe;
}
</style>
</asp:Content>
