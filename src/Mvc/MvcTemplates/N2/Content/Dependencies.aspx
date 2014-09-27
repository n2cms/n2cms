<%@ Page
	Language		= "C#"
	MasterPageFile	= "Framed.Master" 
	Inherits		= "N2.Edit.Web.EditPage" %>
<%@ Register
	Src			= "ReferencingItems.ascx"
	TagName		= "ReferencingItems"
	TagPrefix	= "edit" %>
<%@ Register
	TagPrefix	= "edit"
	Namespace	= "N2.Edit.Web.UI.Controls"
	Assembly	= "N2.Management" %>
<asp:Content ID="ContentToolbar" ContentPlaceHolderID="Toolbar" runat="server">
		<asp:HyperLink ID="hlBack" runat="server" CssClass="btn plain command" meta:resourceKey="hlBack">Close</asp:HyperLink>
</asp:Content>
<asp:Content
		ID="ContentContent"
		ContentPlaceHolderID="Content"
		runat="server">
	<edit:FieldSet
			class="referencingItems"
			runat="server"
			Legend="Items referencing the items you're deleting"
			style="padding:8px;"
			meta:resourceKey="referencingItems">
		<edit:ReferencingItems
				id="referencingItems"
				runat="server" />
	</edit:FieldSet>
</asp:Content>

<script runat="server">
	protected override void OnLoad(EventArgs e)
	{
		if(!this.IsPostBack) {
			this.referencingItems.Item = this.SelectedItem;
			this.referencingItems.DataBind();
			hlBack.NavigateUrl = Request["returnUrl"] ?? SelectedNode.PreviewUrl;
		}
		
		base.OnLoad(e);
	}
</script>
