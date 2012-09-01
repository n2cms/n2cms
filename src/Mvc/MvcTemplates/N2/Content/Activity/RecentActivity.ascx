<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentActivity.ascx.cs" Inherits="N2.Management.Content.Activity.RecentActivity" %>
<%@ Register TagPrefix="n2" Namespace="N2.Web.UI.WebControls" Assembly="N2" %>
<%@ Import Namespace="N2.Web" %>
<n2:Box ID="boxActivity" HeadingText="Recent Activity" CssClass="box activityBox" runat="server" Visible="<%# ShowActivities %>" meta:resourceKey="boxActivity">
</n2:Box>
<script type="text/template" id="activityTemplate">
	<table class="gv">
		<thead>
			<tr><td><%= GetLocalResourceString("bfOperation.HeaderText", "Operation") %></td><td><%= GetLocalResourceString("bfBy.HeaderText", "By") %></td><td></td></tr>
		</thead>
		<tbody>
		{{#Activities}}
			<tr>
				<td>{{Operation}}</td>
				<td>{{PerformedBy}}</td>
				<td>{{AddedDate}}</td>
			</tr>
		{{/Activities}}
		</tbody>
	</table>
</script>

<script type="text/javascript">
	var activityContainer = "#<%= boxActivity.ClientID %> .box-inner";
	var activities = <%= ActivitiesJson %>;
	$(function () {
		var template = Hogan.compile($("#activityTemplate").html());
		$(activityContainer).html(template.render(activities));

		var isDirty = false;
		$(document).keyup(function () {
			isDirty = true;
		});
		if (window.tinymce){
			tinymce.onAddEditor.add(function(mce, ed){
				ed.onKeyUp.add(function() {
					isDirty = true;
				})
			});
		}
		setInterval(function () {
			if (!$(activityContainer).is(":visible"))
				return;
			$.ajax({
				method: "POST",
				url: "editing.n2.ashx",
				dataType: 'json',
				data: { selected: n2ctx.selectedPath, action: "editing", activity: isDirty },
				success: function (data) {
					if (data.LastChange !== activities.LastChange)
						$(activityContainer).html(template.render(data));
				}
			});
			isDirty = false;
		}, 60000);
	});
</script>