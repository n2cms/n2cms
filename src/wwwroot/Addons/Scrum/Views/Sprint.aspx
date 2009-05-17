<%@ Page Language="C#" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.Master" AutoEventWireup="true" CodeBehind="Sprint.aspx.cs" Inherits="N2.Templates.Scrum.UI.Sprint" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentAndSidebar" runat="server">
	<div class="singleColumn">
		<h1><%= CurrentItem.Title %></h1>
		<table class="c4"><tr><td>
			<h3 id="Pending">Open</h3>
			<n2:DroppableZone ID="zPending" ZoneName="Pending" runat="server" />
		</td><td>
			<h3 id="InProgress">In progress</h3>
			<n2:DroppableZone ID="zInProgress" ZoneName="InProgress" runat="server" />
		</td><td>
			<h3 id="Done">Done</h3>
			<n2:DroppableZone ID="zDone" ZoneName="Done" runat="server" />
		</td><td>
			<h3 id="Impediments">Impediments</h3>
			<n2:DroppableZone ID="zImpediments" ZoneName="Impediments" runat="server" />
		</td></tr></table>
	</div>
</asp:Content>
