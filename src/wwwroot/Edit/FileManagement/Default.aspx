<%@ Page MasterPageFile="../Framed.master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.FileManagement.Default" Title="File Manager" meta:resourceKey="fileManagementPage" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Edit" %>

<asp:Content ID="ch" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="Css/filemanager.css" type="text/css" />    
    <script src="../Js/UrlSelection.js" type="text/javascript" ></script>
	<script src="Js/FileSelection.js" type="text/javascript" ></script>
    <script src="Js/jquery.MultiFile.js" type="text/javascript" ></script>
</asp:Content>
<asp:Content ID="ct" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:HyperLink ID="hlItems" runat="server" CssClass="command switch" NavigateUrl="../ItemSelection/Default.aspx" meta:resourceKey="hlItems">items</asp:HyperLink>
    <asp:LinkButton ID="btnDelete" runat="server" CssClass="command fileTool folderTool" OnCommand="OnDeleteCommand" meta:resourceKey="btnDelete">delete</asp:LinkButton>
    <asp:HyperLink ID="hlNewFolder" runat="server" NavigateUrl="javascript:n2nav.onNewFolderClick();" class="command folderTool" meta:resourceKey="hlNewFolder">new folder</asp:HyperLink>
    <asp:HyperLink ID="hlUpload" runat="server" NavigateUrl="javascript:n2nav.onUploadClick();" class="command folderTool" meta:resourceKey="hlUpload">upload file</asp:HyperLink>
    <asp:HyperLink ID="hlCancel" runat="server" CssClass="cancel command" NavigateUrl="javascript:window.close();" meta:resourceKey="hlCancel">cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
    <input type="hidden" id="selectedUrl" runat="server" />
    <asp:CustomValidator ID="cvDeleteRoot" runat="server" ErrorMessage="Cannot delete upload folder." OnServerValidate="OnDeleteValidation" meta:resourceKey="cvDeleteRoot" Display="Dynamic" CssClass="validator" />
    <asp:CustomValidator ID="cvDeleteProblem" runat="server" ErrorMessage="Error deleting file/folder." meta:resourceKey="cvDeleteProblem" Display="Dynamic" CssClass="validator" />
    <asp:CustomValidator ID="cvDeleteException" runat="server" Display="Dynamic" CssClass="validator" />
    
    <div id="actions" class="actions">
        <div id="newFolder" class="actionPanel newFolderPanel">
            <asp:TextBox ID="txtFolder" runat="server" />
            <asp:Button ID="btnCreateFolder" runat="server" Text="OK" OnClick="OnCreateFolderClick" meta:resourceKey="btnCreateFolder"/>
            <input type="button" onclick="n2nav.onCancel();" value="Cancel" />
            <asp:RegularExpressionValidator ID="revFolder" ControlToValidate="txtFolder" ValidationExpression="^[0-9a-zA-Z_\-]{1,}$" ErrorMessage="Only alphanumeric characters supported." runat="server"  meta:resourceKey="revFolder"/>
        </div>
        <div id="upload" class="actionPanel uploadPanel">
			<asp:FileUpload ID="fileUpload" runat="server" CssClass="multi" />
			<asp:Button ID="btnUpload" runat="server" OnClick="OnUploadClick" Text="Upload" />
			<input type="button" onclick="n2nav.onCancel();" value="Cancel" />
        </div>
    </div>
    <div id="tree" class="tree">
        <asp:SiteMapDataSource ID="smds" runat="server" ShowStartingNode="false" />
        <ul>
            <li>
                <edit:TreeView ID="fileView" runat="server" DataSourceID="smds" OnTreeNodeDataBound="fileView_TreeNodeDataBound" ExpandDepth="10" />
            </li>
        </ul>
    </div>
    <script type="text/javascript">
        $(document).ready(function(){
			toolbarSelect("filemanager");
			
		    n2nav.selectionInputId = '#<%= selectedUrl.ClientID %>';
		    n2nav.parentInputId = '<%= OpenerInputId %>';
		    n2nav.setupLinks('#tree');
			$("#tree").SimpleTree();
        });
    </script>
</asp:Content>