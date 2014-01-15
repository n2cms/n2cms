<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ABTestingModel>" %>

<% if (N2.Web.UI.WebControls.ControlPanel.GetState(Html.ContentEngine()) == N2.Web.UI.WebControls.ControlPanelState.DragDrop) { %>
	<% for (int i = 0; i < Model.Zones.Length; i++) {
		if (Model.Percentages[i] <= 0) continue; %>
		<fieldset>
			<legend>Bucket <%= i + 1 %> (<%= Model.Percentages[i] %>/<%= Model.PercentageSum %>)</legend>
			<% Html.DroppableZone(Model.Zones[i]).Render(); %>
		</fieldset>
	<% } %>
<% } else { %>
	<% Html.Zone(Model.ChosenZone).Render(); %>
<% } %>