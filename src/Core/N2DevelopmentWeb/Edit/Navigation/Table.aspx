<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Table.aspx.cs" Inherits="N2.Edit.Navigation.Table" meta:resourceKey="tablePage" %>
<%@ Register TagPrefix="nav" TagName="ContextMenu" Src="ContextMenu.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head id="Head1" runat="server">
        <title>Navigation</title>
        <link rel="stylesheet" href="../Css/All.css" type="text/css" />
        <link rel="stylesheet" href="../Css/Framed.css" type="text/css" />
        <script src="../Js/plugins.ashx" type="text/javascript" ></script>
    </head>
<body class="navigation table">
    <form id="form1" runat="server">
        <div id="nav" class="list">
            <asp:SiteMapDataSource ID="smds" runat="server" />
            <asp:SiteMapPath ID="smp" runat="server" CssClass="path" SkipLinkText="">
                <NodeTemplate>
                    <a class="enabled" onclick="<%# Eval("CurrentItem.Path", "window.top.n2.setupToolbar('{0}');") %>"
						rel='<%# Eval("CurrentItem.Path") %>'
                        href='<%# "Table.aspx?selected=" + Server.UrlEncode((string)Eval("CurrentItem.RewrittenUrl")) %>'>
                        <asp:Image ImageUrl='<%# Eval("CurrentItem.IconUrl") %>' runat="server" />
                        <%# Container.SiteMapNode.Title %>
                    </a>
                </NodeTemplate>
                <CurrentNodeStyle CssClass="selected" />
            </asp:SiteMapPath>

            <n2:ItemDataSource ID="idsItems" runat="server" />
            <asp:DataGrid ID="dgrItems" DataSourceID="idsItems" runat="server" DataKeyField="ID"
                AutoGenerateColumns="false" OnItemCommand="OnDataGridItemCommand" AllowPaging="true" PageSize="100"
                CssClass="gv" AlternatingItemStyle-CssClass="alt" UseAccessibleHeader="true">
                <Columns>
                    <asp:TemplateColumn>
                        <ItemTemplate>
                            <a onclick="<%# Eval("Path", "window.top.n2.setupToolbar('{0}');") %>"
								rel='<%# Eval("Path") %>'
                                href='<%# "Table.aspx?selected=" + Server.UrlEncode((string)Eval("RewrittenUrl")) %>' 
                                style='<%# ((int)Eval("Children.Count")==0) ? "display:none" : "" %>'
                                title='<%# (int)Eval("Children.Count") %>'>
                                <img src="../Img/Ico/bullet_toggle_plus.gif" />
                            </a>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-CssClass="sort">
                        <ItemTemplate>
                            <div title="<%# Eval("SortOrder") %>">
                                <asp:LinkButton ID="btnSortUp" runat="server" CommandName="SortUp" CommandArgument='<%# Eval("ID") %>' CssClass="up">
                                    <img src="../Img/Ico/bullet_arrow_up.gif" />
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnSortDown" runat="server" CommandName="SortDown" CommandArgument='<%# Eval("ID") %>' CssClass="down">
                                    <img src="../Img/Ico/bullet_arrow_down.gif" />
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Title" meta:resourceKey="colTitle">
                        <ItemTemplate>
                            <asp:HyperLink ID="hlShow" runat="server" Target="preview" CssClass="title"
                                rel='<%# Eval("Path") %>'
                                NavigateUrl='<%# Eval("RewrittenUrl") %>'>
                                <asp:Image ImageUrl='<%# Eval("IconUrl") %>' runat="server" />
                                <%# Eval("Title")%>
                            </asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Name" meta:resourceKey="colName">
                        <ItemTemplate>
                            <%# Eval("Name") %>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Publ." meta:resourceKey="colPublished">
                        <ItemTemplate>
                            <%# Eval("Published", "{0:yyy-MM-dd}") %>-<%# Eval("Expires", "{0:yyy-MM-dd}") %>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Zone" meta:resourceKey="colZone">
                        <ItemTemplate>
                            <%# Eval("ZoneName") %>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </div>
        <nav:ContextMenu id="cm" runat="server" />
    </form>
</body>
</html>
