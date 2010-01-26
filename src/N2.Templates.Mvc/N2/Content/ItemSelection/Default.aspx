<%@ Page Language="C#" MasterPageFile="../Framed.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.ItemSelection.Default" Title="Item selector" meta:resourceKey="itemSelectionPage" ResponseEncoding="UTF-8" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<asp:Content ID="ch" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="../../Resources/Css/Framed.css" type="text/css" />
    <script src="../../Resources/Js/UrlSelection.js" type="text/javascript" ></script>
    <script src="Js/ItemSelection.js" type="text/javascript" ></script>
</asp:Content>
<asp:Content ID="ct" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:HyperLink ID="hlFiles" runat="server" CssClass="command switch" NavigateUrl="../../Files/Selector.aspx" meta:resourceKey="hlFiles">files</asp:HyperLink>
    <asp:HyperLink ID="hlCancel" runat="server" CssClass="cancel command" NavigateUrl="javascript:window.close();" meta:resourceKey="hlCancel">cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
    <input type="hidden" id="selectedUrl" runat="server" />
    <div id="tree" class="tree">
        <edit:Tree ID="siteTreeView" runat="server" Target="link" />
    </div>
    <script type="text/javascript">
        $(document).ready(function(){
	        n2nav.parentInputId = '<%= OpenerInputId %>';
            n2nav.setupLinks('#tree');
			$("#tree").SimpleTree({
//				success: function(el){
//					n2nav.refreshLinks(el);
//				}
			});
		});
    </script>
</asp:Content>
