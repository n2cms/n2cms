<%@ Page MasterPageFile="Framed.Master" Language="C#" AutoEventWireup="true" CodeBehind="New.aspx.cs" Inherits="N2.Edit.New" Title="Create new item" meta:resourceKey="DefaultResource" %>
<%@ Import namespace="N2.Definitions"%>
<asp:Content ContentPlaceHolderID="Toolbar" ID="ct" runat="server">
    <asp:HyperLink ID="hlCancel" runat="server" CssClass="cancel command" meta:resourceKey="hlCancel">cancel</asp:HyperLink>
</asp:Content>
<asp:Content ContentPlaceHolderID="Content" ID="cc" runat="server">
	<asp:CustomValidator ID="cvPermission" CssClass="validator info" ErrorMessage="Not authorized" Display="Dynamic" runat="server" />
    <n2:TabPanel runat="server" ToolTip="Select type" meta:resourceKey="tpType">
		<div class="cf">
		<asp:Repeater ID="rptTypes" runat="server">
			<ItemTemplate>
				<div class="type cf i<%# Container.ItemIndex %> a<%# Container.ItemIndex % 2 %>">
					<asp:HyperLink ID="hlNew" NavigateUrl='<%# GetEditUrl((ItemDefinition)Container.DataItem) %>' ToolTip='<%# Eval("ToolTip") %>' runat="server">
						<asp:Image ID="imgIco" ImageUrl='<%# Eval("IconUrl") %>' CssClass="icon" runat="server" ToolTip='<%# Eval("NumberOfItems") %>' />
						<span class="title"><%# GetDefinitionString((ItemDefinition)Container.DataItem, "Title") ?? Eval("Title") %></span>
						<span class="description"><%# GetDefinitionString((ItemDefinition)Container.DataItem, "Description") ?? Eval("Description")%></span>
					</asp:HyperLink>
				</div>
			</ItemTemplate>
		</asp:Repeater>
		</div>
    </n2:TabPanel>
    
    <n2:TabPanel runat="server" ToolTip="Position" meta:resourceKey="tpPosition" >
		<asp:Label ID="lblPosition" runat="server" meta:resourceKey="lblZone" Text="Create new item in zone" />
        <asp:RadioButtonList ID="rblPosition" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblPosition_OnSelectedIndexChanged" CssClass="position">
            <asp:ListItem Value="0" meta:resourceKey="fsPosition_before">
                Before the selected item (at the same depth)
                <blockquote><ul>
                    <li>other items</li>
                    <li><em>new item</em></li>
                    <li><strong>selected item</strong><ul>
                        <li>other item</li>
                    </ul></li>
                </ul></blockquote>
            </asp:ListItem>
            <asp:ListItem Value="1" Selected="true" meta:resourceKey="fsPosition_below">
                Below the selected item (one level deeper)
                <blockquote><ul>
                    <li><strong>selected item</strong><ul>
                        <li>other item</li>
                        <li><em>new item</em></li>
                    </ul></li>
                    <li>other item</li>
                </ul></blockquote>
            </asp:ListItem>
        </asp:RadioButtonList>
    </n2:TabPanel>
    
    <n2:TabPanel runat="server" ToolTip="Zone" meta:resourceKey="tpZone" >
		<asp:Label ID="lblZone" runat="server" meta:resourceKey="lblZone" Text="Create new item in zone" />
		<asp:RadioButtonList ID="rblZone" DataTextField="Title" DataValueField="ZoneName" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblZone_OnSelectedIndexChanged">
            <asp:ListItem Value="" Selected="true" meta:resourceKey="rblZone_default">Default</asp:ListItem>
        </asp:RadioButtonList>
    </n2:TabPanel>
    
    <script type="text/javascript">
    	var key = { up: 38, right: 39, down: 40 };
    	jQuery(document).keyup(function(e) {
    		if (e.keyCode == key.up || e.keyCode == key.down) {
    			$selectables = $(".type a");
    			var index = $selectables.index($(":focus"));
    			index += e.keyCode == key.up ? -1 : 1;
    			$selectables.eq(index).focus();
    		}
    	});
    </script>
</asp:Content>