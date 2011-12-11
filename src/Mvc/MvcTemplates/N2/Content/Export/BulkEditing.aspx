<%@ Page Title="" Language="C#" MasterPageFile="~/N2/Content/Framed.master" AutoEventWireup="true" CodeBehind="BulkEditing.aspx.cs" Inherits="N2.Management.Content.Export.BulkEditing" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Toolbar" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Outside" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">
	<fieldset>
		<legend>Item filtering</legend>

		<label>
			All children of type
			<input type="radio" name="option" value="All children" />
		</label>

		<label>
			All descendant items of type
			<input type="radio" name="option" value="All children" />
		</label>
		<asp:DropDownList ID="ddlTypes" DataTextField="Title" DataValueField="Discriminator" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddlTypes_OnSelectedIndexChanged" />
	</fieldset>

	<fieldset>
		<legend>Item selection</legend>

		<asp:RadioButtonList ID="rblDescendants" runat="server" />
	</fieldset>

	<n2:ItemEditor ID="ie" runat="server" />
</asp:Content>
