<%@ Page Title="" Language="C#" MasterPageFile="~/N2/Content/Framed.master" AutoEventWireup="true" CodeBehind="BulkEditing.aspx.cs" Inherits="N2.Management.Content.Export.BulkEditing" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Toolbar" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Outside" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">

<div class="tabPanel">
	<asp:MultiView ID="mvWizard" runat="server" ActiveViewIndex="0">
		<asp:View ID="vSelection" runat="server">
			<div class="filtering">
			<div>
				<label>Type</label>
				<asp:DropDownList ID="ddlTypes" DataTextField="Title" DataValueField="Discriminator" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddlTypes_OnSelectedIndexChanged" />
			</div>
			<div>
				<label><input type="radio" name="selection" value="all" <%= Request["selection"] != "specific" ? "checked='checked'" : "" %> /> All</label>
			<div>
			</div>
				<label><input type="radio" name="selection" value="specific" <%= Request["selection"] == "specific" ? "checked='checked'" : "" %>/> Select specific items</label>
			</div>
			<blockquote>
				<asp:CheckBoxList CssClass="descendants" DataTextField="Title" DataValueField="ID" ID="chkDescendants" runat="server" RepeatDirection="Horizontal" RepeatColumns="5" />
			</blockquote>
			<blockquote>
				<a href="#all" onclick="$('.descendants input').attr('checked', 'checked'); return false;">Select all</a>
				<a href="#none" onclick="$('.descendants input').removeAttr('checked'); return false;">Deselect all</a>
			</blockquote>
			<asp:Button runat="server" CommandArgument="0" OnCommand="OnNext" Text="Next" />
			</div>
		</asp:View>
		<asp:View ID="vEditors" runat="server">
			<asp:Repeater ID="rptEditors" runat="server" EnableViewState="false">
				<ItemTemplate>
					<div><label title="<%# string.Join(",", GetSelectedEditors()) %>"><input class="editorSelection" name="Editors" value='<%# Eval("Name") %>' type="checkbox" <%# Eval("Checked") %> /><%# Eval("Title") %></label></div>
				</ItemTemplate>
			</asp:Repeater>

			<asp:Button ID="Button1" runat="server" CommandArgument="1" OnCommand="OnNext" Text="Next" />
		</asp:View>
		<asp:View ID="vEditing" runat="server">
			<asp:ValidationSummary ID="vsEdit" runat="server" CssClass="validator info" HeaderText="The item couldn't be saved. Please look at the following:" meta:resourceKey="vsEdit"/>
			<asp:CustomValidator ID="cvException" runat="server" Display="None" />

			<n2:ItemEditor ID="ie" runat="server" />

			<asp:Button ID="btnSave" runat="server" OnCommand="OnSave" Text="Save" />
		</asp:View>
		<asp:View ID="vConfirmation" runat="server">
			<asp:Repeater ID="rptAffectedItems" runat="server">
				<HeaderTemplate><h2>Affected items</h2><ul></HeaderTemplate>
				<ItemTemplate>
					<li><a href="<%# Eval("Url") %>"><img src='<%# Eval("IconUrl") %>' /><%# Eval("Title") %></a></li>
				</ItemTemplate>
				<FooterTemplate></ul></FooterTemplate>
			</asp:Repeater>
		</asp:View>
	</asp:MultiView>

</div>
	<script type="text/javascript">
		$(document).ready(function () {
			$("input:radio[name='selection']").change(function () {
				var $d = $("blockquote");
				if (this.value == "specific") {
					$d.fadeIn();
				} else {
					$d.hide();
					$d.find("input").attr("checked", "checked");
				}
			}).filter(":checked").trigger("change");
		});
	</script>
	<style>
		.filtering blockquote a { margin-right: 5px; }
	</style>
</asp:Content>
