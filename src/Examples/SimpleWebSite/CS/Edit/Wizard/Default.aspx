<%@ Page MasterPageFile="../Framed.master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Wizard.Default" Title="Wizard" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>
<%@ Import namespace="N2.Edit.Wizard.Items"%>
<asp:Content ID="ContentToolbar" ContentPlaceHolderID="Toolbar" runat="server">
	<asp:HyperLink ID="hlCancel" runat="server" CssClass="cancel command" meta:resourceKey="hlCancel">Cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
    <n2:tabpanel ID="tpType" runat="server" ToolTip="Select type" CssClass="tabPanel" meta:resourcekey="tpTypeResource1" RegisterTabCss="False">
        <asp:Repeater ID="rptLocations" runat="server">
            <ItemTemplate>
			    <div class="type cf">
				    <asp:HyperLink ID="hlNew" NavigateUrl='<%# GetEditUrl((MagicLocation)Container.DataItem) %>' ToolTip='<%# Eval("ToolTip") %>' runat="server">
						<asp:Image ID="imgIco" ImageUrl='<%# Eval("IconUrl") %>' CssClass="icon" runat="server" meta:resourcekey="imgIcoResource1" />
                        <%# Eval("Title") %>
                    </asp:HyperLink>
				    <%# Eval("Description") %>
			    </div>
            </ItemTemplate>
        </asp:Repeater>
    </n2:tabpanel>
    <n2:tabpanel ID="tpAdd" runat="server" ToolTip="Add location" CssClass="tabPanel" meta:resourcekey="tpAddResource1" RegisterTabCss="False">
        <asp:MultiView ID="mvAdd" runat="server" ActiveViewIndex="0">
            <asp:View runat="server">
                <div class="cf">
                    <asp:Label runat="server" Text="Title" AssociatedControlID="txtTitle" 
						meta:resourcekey="LabelResource1" />
                    <asp:TextBox ID="txtTitle" runat="server" 
						meta:resourcekey="txtTitleResource1" />
                </div>
                <div class="cf">
                    <asp:Label runat="server" Text="Type" AssociatedControlID="ddlTypes" 
						meta:resourcekey="LabelResource2" />
                    <asp:DropDownList ID="ddlTypes" runat="server" DataTextField="Title" 
						DataValueField="Discriminator" meta:resourcekey="ddlTypesResource1" />
                </div>
                <div class="cf">
                    <asp:Button ID="btnAdd" runat="server" Text="Add" OnCommand="btnAdd_Command" 
						meta:resourcekey="btnAddResource1" />
                </div>
            </asp:View>
            <asp:View runat="server">
                <asp:Label runat="server" Text="Added" meta:resourcekey="LabelResource3" />
            </asp:View>
        </asp:MultiView>
    </n2:tabpanel>
</asp:Content>