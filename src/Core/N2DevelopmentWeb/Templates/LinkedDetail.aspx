<%@ Page Language="C#" MasterPageFile="~/DefaultMasterPage.Master" AutoEventWireup="true" CodeBehind="LinkedDetail.aspx.cs" Inherits="N2DevelopmentWeb.Templates.LinkedDetail" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Do some serious stuff" /><br />
	<asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="It's time to stop laughing" /><br />

Item 1 referenced by this item:
	<ul><li><asp:HyperLink runat="server" NavigateUrl="<%$ CurrentItem: ReferencedItem.Url %>" Text="<%$ CurrentItem: ReferencedItem.Url %>" /></li></ul>

Find all inbound links:
	<ul><asp:Repeater runat="server" DataSource="<%# N2.Context.Persister.RelationFinder.ListReferences(CurrentPage) %>">
		<ItemTemplate>
			<li><asp:HyperLink runat="server" NavigateUrl='<%# Eval("EnclosingItem.Url") %>' Text='<%# Eval("EnclosingItem.Url") %>' /></li>
		</ItemTemplate>
	</asp:Repeater></ul>

All details:
	<ul>
		<asp:Repeater ID="Repeater3" runat="server" DataSource='<%# CurrentItem.Details.Values %>'>
			<ItemTemplate>
				<li>
					<%# Eval("Name") %>: <%# Eval("Value") %>
				</li>
			</ItemTemplate>
		</asp:Repeater>
	</ul>

DetailCollections:
	<ul>
		<asp:Repeater ID="Repeater4" runat="server" DataSource='<%# CurrentItem.DetailCollections.Values %>'>
			<ItemTemplate>
				<li>
					<%# Eval("Name") %>: <%# Eval("Count") %>
				</li>
			</ItemTemplate>
		</asp:Repeater>
	</ul>

<hr />

CurrentItem["relatedList"]:
	<ul>
		<asp:Repeater ID="Repeater2" runat="server" DataSource='<%# CurrentItem["relatedList"] %>'>
			<ItemTemplate>
				<li>
					<a href="<%# Eval("Url") %>"><%# Eval("Url")%></a>
				</li>
			</ItemTemplate>
		</asp:Repeater>
	</ul>

<hr />

ListReferences ReferencedItem2:
	<ul><asp:Repeater ID="Repeater1" runat="server" DataSource='<%# N2.Context.Persister.RelationFinder.SetDetailName("ReferencedItem2").ListReferences(CurrentPage) %>'>
		<ItemTemplate>
			<li><asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# Eval("EnclosingItem.Url") %>' Text='<%# Eval("EnclosingItem.Url") %>' ToolTip='<%# Eval("ID") %>' /></li>
		</ItemTemplate>
	</asp:Repeater></ul>
ListReferences relatedList2:
	<ul><asp:Repeater ID="Repeater5" runat="server" DataSource='<%# N2.Context.Persister.RelationFinder.SetDetailName("relatedList2").ListReferences(CurrentPage) %>'>
		<ItemTemplate>
			<li><asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# Eval("EnclosingItem.Url") %>' Text='<%# Eval("EnclosingItem.Url") %>' ToolTip='<%# Eval("ID") %>' /></li>
		</ItemTemplate>
	</asp:Repeater></ul>
ListReferences relatedList2:
	<ul><asp:Repeater ID="Repeater6" runat="server" DataSource='<%# N2.Context.Persister.RelationFinder.SetDetailName("relatedList2").SetMaxResults(4).SetFirstResult(1).SetSortExpression("Published DESC").ListReferences(CurrentPage) %>'>
		<ItemTemplate>
			<li><asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# Eval("EnclosingItem.Url") %>' Text='<%# Eval("EnclosingItem.Url") %>' ToolTip='<%# Eval("ID") %>' /></li>
		</ItemTemplate>
	</asp:Repeater></ul>

</asp:Content>
