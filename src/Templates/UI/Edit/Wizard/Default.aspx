<%@ Page MasterPageFile="~/Edit/Framed.master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Wizard.Default" Title="Wizard" %>
<%@ Import namespace="N2.Edit.Wizard.Items"%>
<asp:Content ContentPlaceHolderID="Head" ID="ch" runat="server">
   <link rel="stylesheet" href="../Css/new.css" type="text/css" />
</asp:Content>
<asp:Content ID="ContentToolbar" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:HyperLink ID="hlCancel" runat="server" CssClass="cancel command" meta:resourceKey="hlCancel">cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
    <n2:TabPanel runat="server" ToolTip="Select type">
        <asp:Repeater ID="rptLocations" runat="server">
            <ItemTemplate>
			    <div class="type cf">
				    <asp:HyperLink ID="hlNew" NavigateUrl='<%# GetEditUrl((MagicLocation)Container.DataItem) %>' ToolTip='<%# Eval("ToolTip") %>' runat="server">
					    <asp:Image ID="imgIco" ImageUrl='<%# Eval("IconUrl") %>' CssClass="icon" runat="server" />
                        <%# Eval("Title") %>
				    </asp:HyperLink>
				    <%# Eval("Description") %>
			    </div>
            </ItemTemplate>
        </asp:Repeater>
    </n2:TabPanel>
    <n2:TabPanel runat="server" ToolTip="Add location">
        <asp:MultiView ID="mvAdd" runat="server" ActiveViewIndex="0">
            <asp:View runat="server">
                <div class="cf">
                    <asp:Label runat="server" Text="Title" AssociatedControlID="txtTitle" />
                    <asp:TextBox ID="txtTitle" runat="server" />
                </div>
                <div class="cf">
                    <asp:Label runat="server" Text="Type" AssociatedControlID="ddlTypes" />
                    <asp:DropDownList ID="ddlTypes" runat="server" DataTextField="Title" DataValueField="Discriminator" />
                </div>
                <div class="cf">
                    <asp:Button ID="btnAdd" runat="server" Text="Add" OnCommand="btnAdd_Command" />
                </div>
            </asp:View>
            <asp:View runat="server">
                <asp:Label runat="server" Text="Added" />
            </asp:View>
        </asp:MultiView>
    </n2:TabPanel>
</asp:Content>