<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="N2.Edit.Navigation.Search" meta:resourceKey="searchPage" %>
<%@ Register TagPrefix="nav" TagName="ContextMenu" Src="ContextMenu.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head id="Head1" runat="server">
        <title>Navigation</title>
        <link rel="stylesheet" href="../Css/All.css" type="text/css" />
        <link rel="stylesheet" href="../Css/Framed.css" type="text/css" />
        <script src="../Js/plugins.ashx" type="text/javascript" ></script>
        <script src="../Js/jquery.ui.ashx" type="text/javascript" ></script>
        <script type="text/javascript">
            $(document).ready(function(){
				toolbarSelect("search");
			});
		</script>
    </head>
<body class="navigation search">
    <form id="form1" runat="server">
        <asp:Panel runat="server" DefaultButton="btnSerach" CssClass="list">
            <asp:TextBox ID="txtQuery" runat="server" CssClass="tb" />
            <asp:ImageButton ID="btnSerach" runat="server" ImageUrl="../Img/Ico/find.gif" OnClick="btnSerach_Click" CssClass="btn" meta:resourceKey="btnSearch" />
            <asp:RequiredFieldValidator ID="rfvQuery" ControlToValidate="txtQuery" runat="server" ErrorMessage="*" meta:resourceKey="rfvQuery" Display="Dynamic" />
            <n2:ItemDataSource ID="idsItems" runat="server" />
            <div id="nav" class="nav">
                <asp:DataGrid ID="dgrItems" DataSourceID="idsItems" DataMember="Query" runat="server" DataKeyField="ID" AutoGenerateColumns="false" CssClass="gv" AlternatingItemStyle-CssClass="alt" UseAccessibleHeader="true">
                    <Columns>
                        <asp:TemplateColumn HeaderText="Title" meta:resourceKey="colTitle" >
                            <ItemTemplate>
                                <asp:HyperLink ID="hlShow" runat="server" Target="preview" rel='<%# Eval("Path") %>' NavigateUrl='<%# ((N2.INode)Container.DataItem).PreviewUrl %>'>
                                    <img src="<%# Eval("IconUrl") %>" />
                                    <%# Eval("Title")%>
                                </asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Publ." meta:resourceKey="colPublished" ItemStyle-CssClass="date">
                            <ItemTemplate>
                                <%# Eval("Published", "{0:yyy-MM-dd}") %>-<%# Eval("Expires", "{0:yyy-MM-dd}") %>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Zone" meta:resourceKey="colZone" >
                            <ItemTemplate>
                                <%# Eval("ZoneName") %>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </div>
            <nav:ContextMenu id="cm" runat="server" />
        </asp:Panel>
    </form>
</body>
</html>
