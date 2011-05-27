<%@ Page MasterPageFile="../Framed.master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Trash.Default" %>
<asp:Content ID="ContentToolbar" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:LinkButton ID="btnClear" runat="server" CssClass="command" meta:resourceKey="btnClear" OnClientClick="return confirm('really empty trash?');" OnClick="btnClear_Click">empty trash</asp:LinkButton>
    <asp:HyperLink ID="hlCancel" runat="server" CssClass="cancel command" meta:resourceKey="hlCancel">cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="ContentContent" ContentPlaceHolderID="Content" runat="server">
	<n2:ItemDataSource id="idsTrash" runat="server" />
	<h1>Trash</h1>
	<asp:CustomValidator ID="cvRestore" CssClass="validator" ErrorMessage="An item with the same name already exists at the previous location." runat="server" Display="Dynamic" />
	<asp:GridView ID="gvTrash" DataKeyNames="ID" runat="server" DataSourceID="idsTrash" AutoGenerateColumns="false" OnRowCommand="gvTrash_RowCommand" EmptyDataText="No items in trash" CssClass="gv" AlternatingRowStyle-CssClass="alt">
		<Columns>
			<asp:TemplateField HeaderText="Title" meta:resourceKey="colTitle">
				<ItemTemplate>
					<asp:HyperLink ID="hlDeletedItem" runat="server" NavigateUrl='<%# Eval("Url") %>'>
						<asp:Image runat="server" ImageUrl='<%# Eval("IconUrl") %>' />
						<%# Eval("Title") %>
					</asp:HyperLink>
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Deleted" meta:resourceKey="colDeleted">
				<ItemTemplate>
					<%# ((N2.ContentItem)Container.DataItem)["DeletedDate"] %>				
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Previous location" meta:resourceKey="colPrevious">
				<ItemTemplate>
					<asp:HyperLink ID="hlPreviousLocation" runat="server" NavigateUrl='<%# DataBinder.Eval(((N2.ContentItem)Container.DataItem)["FormerParent"], "Url") %>'>
						<asp:Image runat="server" ImageUrl='<%# DataBinder.Eval(((N2.ContentItem)Container.DataItem)["FormerParent"], "IconUrl") %>' />
						<%# DataBinder.Eval(((N2.ContentItem)Container.DataItem)["FormerParent"], "Title") %>
					</asp:HyperLink>
				</ItemTemplate>
			</asp:TemplateField>
			<asp:ButtonField Text="Restore" CommandName="Restore" meta:resourceKey="colRestore" />
			<asp:ButtonField Text="Delete" CommandName="Delete" meta:resourceKey="colDelete" />
		</Columns>
	</asp:GridView>
</asp:Content>