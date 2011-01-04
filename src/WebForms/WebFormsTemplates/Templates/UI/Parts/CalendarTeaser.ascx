<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarTeaser.ascx.cs" Inherits="N2.Templates.UI.Parts.CalendarTeaser" %>
<n2:EditableDisplay ID="dt" runat="server" PropertyName="Title" />
<n2:Box runat="server">
	<asp:Calendar ID="cEvents" runat="server" BorderWidth="0" CssClass="calendar"
		TitleStyle-BackColor="Transparent"
		UseAccessibleHeader="true"
		OtherMonthDayStyle-CssClass="otherMonth" 
		SelectedDayStyle-CssClass="selectedDay" />
</n2:Box>