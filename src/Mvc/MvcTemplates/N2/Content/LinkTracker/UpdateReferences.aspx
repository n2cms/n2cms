<%@ Page Language="C#" MasterPageFile="../Framed.master" AutoEventWireup="true" CodeBehind="UpdateReferences.aspx.cs" Inherits="N2.Edit.LinkTracker.UpdateReferences" meta:resourcekey="PageResource1" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<%@ Register Src="../AffectedItems.ascx" TagName="AffectedItems" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="Toolbar" runat="server">
    <asp:LinkButton ID="btnUpdate" meta:resourceKey="btnUpdate" runat="server" OnCommand="OnUpdateCommand" CssClass="command"><img src='../../Resources/Icons/link_edit.png' /> Update links</asp:LinkButton>
	<edit:CancelLink ID="hlCancel" runat="server" meta:resourceKey="hlCancel">Close</edit:CancelLink>
</asp:Content>
<asp:Content ID="ContentContent" ContentPlaceHolderID="Content" runat="server">
	<div class="tabPanel">

		<asp:MultiView ID="mvPhase" runat="server" ActiveViewIndex="0">
			<asp:View runat="server">
				<p>
					<asp:CheckBox runat="server" Text="Add permanent redirect at previous URL" ID="chkPermanentRedirect" meta:resourceKey="chkPermanentRedirect" />
				</p>
				<fieldset runat="server" id="fsReferences">
					<legend><asp:Literal Text="These items linking to the moved page will be updateds" runat="server" meta:resourceKey="ltUpdatedLinks"></asp:Literal></legend>
		
					<div style="margin:5px;">
					<asp:Repeater runat="server" ID="rptReferencingItems">
						<HeaderTemplate><div style="max-height:200px; overflow:auto;"></HeaderTemplate>
						<ItemTemplate>
							<div>
								<a href='<%# Eval("Url") %>'><asp:Image ImageUrl='<%# ResolveUrl(Eval("IconUrl")) %>' runat="server" /><%# Eval("Title") %></a>
							</div>
						</ItemTemplate>
						<FooterTemplate></div></FooterTemplate>
					</asp:Repeater>
					</div>
				</fieldset>
		
				<fieldset runat="server" id="fsChildren" style="margin-top:10px;">
					<legend><asp:CheckBox Text="Also update links leading to the following children" Checked="true" runat="server" ID="chkChildren" CssClass="DimChildren" meta:resourceKey="chkChildren"/></legend>
		
					<uc1:AffectedItems id="targetsToUpdate" runat="server" />
				</fieldset>
				
				<script type="text/javascript">
					jQuery(function ($) {
						$(".DimChildren input").click(function () {
							var $affected = $(this).closest("fieldset").children(":not(legend)");
							if (this.checked) {
								$affected.removeAttr("disabled").removeClass("disabled");
							} else {
								$affected.attr("disabled", "disabled").addClass("disabled");
							}
						});
					});
				</script>
			</asp:View>
			<asp:View runat="server">
				<fieldset>
					<legend>Updatring references to:</legend>
					<asp:Repeater runat="server" ID="rptDescendants">
						<ItemTemplate>
							<div class="ItemToUpdate" data-id="<%# Eval("ID") %>" data-path="<%# Eval("Path") %>" data-title="<%# Eval("Title") %>">
								<a href='<%# Eval("Url") %>'><asp:Image ImageUrl='<%# ResolveUrl(Eval("IconUrl")) %>' runat="server" /><%# Eval("Title") %></a>
							</div>
						</ItemTemplate>
					</asp:Repeater>
				</fieldset>
				
				<script type="text/javascript">
					jQuery(function ($) {
						var $descendants = $(".ItemToUpdate");
						function call() {
							if ($descendants.length == 0) {
								<%= GetRefreshScript(Selection.SelectedItem, N2.Edit.ToolbarArea.Both, true) %>
								return;
							}
							var $current = $descendants.slice(0, 1);
							$descendants = $descendants.slice(1);
							$.ajax({
								url: "UpdateReferencesTo.ashx",
								data: { selected: $current.attr("data-path") },
								success: function (data) {
									$current.slideUp();
									call();
								}
							});
						}
						call();
					});
				</script>
			</asp:View>
		</asp:MultiView>
	</div>
</asp:Content>