<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Calculator.ascx.cs" Inherits="App.UI.Calculator1" %>

<table class="calculator">
	<!-- Using the part's title and the page's title -->
	<thead><td colspan="4"><%= CurrentItem.Title %> on <%= CurrentPage.Title %></td></thead>
	<tbody><tr>
		<td colspan="4">
			<asp:TextBox ID="TextBox1" runat="server" Width="96%"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td>
			<asp:Button ID="Button7" runat="server" Text="7" onclick="ButtonNumber_Click" />
		</td>
		<td>
			<asp:Button ID="Button8" runat="server" Text="8" onclick="ButtonNumber_Click" />
		</td>
		<td>
			<asp:Button ID="Button9" runat="server" Text="9" onclick="ButtonNumber_Click" />
		</td>
		<td>
			<!-- Using expression builders to inject "content" to the button -->
			<asp:Button ID="ButtonDivide" runat="server" Text="/" Enabled="<%$ CurrentItem: EnableDivide %>"
				onclick="ButtonDivide_Click" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:Button ID="Button4" runat="server" Text="4" onclick="ButtonNumber_Click" />
		</td>
		<td>
			<asp:Button ID="Button5" runat="server" Text="5" onclick="ButtonNumber_Click" />
		</td>
		<td>
			<asp:Button ID="Button6" runat="server" Text="6" onclick="ButtonNumber_Click" />
		</td>
		<td>
			<asp:Button ID="ButtonMultiply" runat="server" Text="*"  Enabled="<%$ CurrentItem: EnableMultiply %>"
				onclick="ButtonMultiply_Click" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:Button ID="Button1" runat="server" Text="1" onclick="ButtonNumber_Click" />
		</td>
		<td>
			<asp:Button ID="Button2" runat="server" Text="2" onclick="ButtonNumber_Click" />
		</td>
		<td>
			<asp:Button ID="Button3" runat="server" Text="3" onclick="ButtonNumber_Click" />
		</td>
		<td>
			<asp:Button ID="ButtonSubtract" runat="server" Text="-"  Enabled="<%$ CurrentItem: EnableSubtract %>"
				onclick="ButtonSubtract_Click" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:Button ID="Button0" runat="server" Text="0" onclick="ButtonNumber_Click" />
		</td>
		<td>
			
			<asp:Button ID="ButtonEquals" runat="server" Text="=" 
				onclick="ButtonEquals_Click"/>
			
		</td>
		<td>
			
			<asp:Button ID="ButtonClear" runat="server" Text="C" 
				onclick="ButtonClear_Click"/>
			
		</td>
		<td>
			<asp:Button ID="ButtonAdd" runat="server" Text="+"  Enabled="<%$ CurrentItem: EnableAdd %>" onclick="ButtonAdd_Click" />
		</td>
	</tr>
	</tbody>
</table>
