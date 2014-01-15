<%@ Page Language="C#" AutoEventWireup="true" Inherits="N2.Web.UI.ContentPage" %>
<%@ Register TagPrefix="n2" Namespace="N2.Web.UI.WebControls" Assembly="N2" %>

<script runat="server">

	protected override void OnInit(EventArgs e)
	{
		ItemDataSource1.Query = N2.Find.Items.Where.Name.Eq(CurrentPage.Name);
		base.OnInit(e);
	}
	
</script>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<n2:ItemDataSource ID="ItemDataSource1" runat="server" />
		<asp:Repeater ID="Repeater2" runat="server" DataSourceID="ItemDataSource1" DataMember="Query">
			<HeaderTemplate><ul></HeaderTemplate>
			<ItemTemplate><li><%# Eval("Title") %></li></ItemTemplate>
			<FooterTemplate></ul></FooterTemplate>
		</asp:Repeater>
    </div>
    </form>
</body>
</html>
