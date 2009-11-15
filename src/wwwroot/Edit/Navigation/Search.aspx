<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="N2.Edit.Navigation.Search" meta:resourceKey="searchPage" %>
<%@ Register TagPrefix="nav" TagName="ContextMenu" Src="ContextMenu.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head id="Head1" runat="server">
        <title>Search</title>
        <asp:PlaceHolder runat="server">
		<link rel="stylesheet" href="<%=MapCssUrl("all.css")%>" type="text/css" />
		<link rel="stylesheet" href="<%=MapCssUrl("framed.css")%>" type="text/css" />
		<link rel="stylesheet" href="<%=MapCssUrl("navigation.css")%>" type="text/css" />
		</asp:PlaceHolder>
        <script src="../Js/jquery.ui.ashx" type="text/javascript" ></script>
    </head>
<body class="edit navigation search">
    <form id="form1" runat="server">
        <asp:Panel runat="server" CssClass="list">
            <n2:ItemDataSource ID="idsItems" runat="server" />
            <div id="nav" class="nav">
                <asp:DataGrid ID="dgrItems" DataSourceID="idsItems" DataMember="Query" runat="server" DataKeyField="ID" AutoGenerateColumns="false" CssClass="gv" AlternatingItemStyle-CssClass="alt" UseAccessibleHeader="true" ShowHeader="false">
                    <Columns>
                        <asp:TemplateColumn HeaderText="Title" meta:resourceKey="colTitle" >
                            <ItemTemplate>
                                <asp:HyperLink ID="hlShow" runat="server" Target="preview" runat="server" 
                                    NavigateUrl='<%# ((N2.INode)Container.DataItem).PreviewUrl %>'
                                    Title='<%# Eval("Published", "{0:yyy-MM-dd}") + " - " + Eval("Expires", "{0:yyy-MM-dd}") %>'
                                    rel='<%# Eval("Path") %>'>
                                    <asp:Image ImageUrl='<%# Eval("IconUrl") %>' runat="server" />
                                    <%# Eval("Title")%>
                                </asp:HyperLink>
                                <%# Eval("ZoneName", " ({0})") %>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </div>
            <nav:ContextMenu id="cm" runat="server" />
            <script type="text/javascript">
            	jQuery(document).ready(function() {
            		if (window.n2ctx) window.n2ctx.toolbarSelect('search');
            	});
            </script>
        </asp:Panel>
    </form>
</body>
</html>
