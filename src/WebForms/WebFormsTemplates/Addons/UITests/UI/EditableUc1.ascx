<%@ Control Language="C#" AutoEventWireup="true" %>
<script runat="server">
	public string PropertyNameOnUserControl 
	{ 
		get { return txtValue.Text; }
		set { txtValue.Text = value; }
	}
</script>

<asp:TextBox ID="txtValue" runat="server" />