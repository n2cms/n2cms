<%@ Page Language="C#" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.Master" AutoEventWireup="true" CodeBehind="Project.aspx.cs" Inherits="N2.Templates.Scrum.UI.Project" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentAndSidebar" runat="server">

		    <div id="plannedTaskContextMenu" class="contextMenu">
				<a href="#" id="A5">Move to sprint</a>
				<a href="#" id="A6">Defer</a>
				<a href="#" id="A7">Edit</a>
			</div>
		    <div id="pendingTaskContextMenu" class="contextMenu">
				<a href="#" id="A8">Move to product backlog</a>
				<a href="#" id="A10">Edit</a>
			</div>
		    <div id="sprintContextMenu" class="contextMenu">
				<a href="#" id="A1">Begin</a>
				<a href="#" id="A2">Edit</a>
			</div>
			<script type="text/javascript">
				var scrum = {
					makeCurrent: function(id){
						$.getJSON("makeCurrentSprint.n2.ashx", {action: "makeCurrentSprint", item: id}, function(data){
							location.reload();
						});
					}
				};
				$(document).ready(function(){
					$(".pending .task").n2contextmenu("#pendingTaskContextMenu");
					$(".planned .task").n2contextmenu("#plannedTaskContextMenu");
					$(".sprint").n2contextmenu("#sprintContextMenu");
				});
//				$(".task").contextMenu('contextMenu', {
//				  bindings: {
//						'open': function(t) {
//						  alert('Trigger was '+t.id+'\nAction was Open');
//						},
//						'email': function(t) {
//						  alert('Trigger was '+t.id+'\nAction was Email');
//						},
//						'save': function(t) {
//						  alert('Trigger was '+t.id+'\nAction was Save');
//						},
//						'delete': function(t) {
//						  alert('Trigger was '+t.id+'\nAction was Delete');
//						}
//					 }
//				  }
//				);
			</script>
	<div class="singleColumn">
		<h1><%= CurrentItem.Title %></h1>
		<table class="c3">
		<thead><tr><td>
			<h3><%= N2.Web.Link.To(CurrentItem.CurrentSprint) %></h3>
		</td><td>
			<h3>Product backlog</h3>
		</td><td>
			<h3>Sprints</h3>
		</td></tr></thead>
		<tbody><tr><td>
			<div>
				<div class="pending"><n2:DroppableZone id="zPending" runat="server" ZoneName="Pending" Path="<%$ CurrentItem: CurrentSprint.Name %>"/></div>
			</div>
			<div>
				<h3>Impediments: <a href="<%= N2.Utility.Evaluate(CurrentItem, "CurrentSprint.Url", "{0}#Impediments") %>"><%= GetCount("Impediments")%></a></h3>
			</div>
			<div>
				<h3>In progress: <a href="<%= N2.Utility.Evaluate(CurrentItem, "CurrentSprint.Url", "{0}#InProgress") %>"><%= GetCount("InProgress")%></a></h3>
			</div>
			<div>
				<h3>Done: <a href="<%= N2.Utility.Evaluate(CurrentItem, "CurrentSprint.Url", "{0}#Done") %>"><%= GetCount("Done")%></a></h3>
			</div>
		</td><td>
			<div class="planned"><n2:DroppableZone ID="zFeatures" runat="server" ZoneName="Planned"/></div>
			<h3>Deferred</h3>
			<n2:DroppableZone ID="zDeferred" runat="server" ZoneName="Deferred"/>
		</td><td>
			<n2:ItemDataSource ID="idsSprints" Query="<%$ Code: GetQuery() %>" runat="server" />
			<asp:Repeater ID="rptSprints" runat="server" DataSourceID="idsSprints" DataMember="Query">
				<ItemTemplate>
					<div class="sprint">
						<h4>
							<%# N2.Web.Link.To((N2.ContentItem)Container.DataItem) %> 
							(<%# Eval("ID", IsCurrent(Container.DataItem) ? "current" : "<a href='javascript:scrum.makeCurrent({0});'>start</a>")%>)
						</h4>
						<div>
							<%# Eval("SprintGoal") %>
						</div>
						<div>
							Value: <%# Eval("EsitimatedValue") %>
						</div>
						<div>
							Effort: <%# Eval("EsitimatedEffort") %>
						</div>
					</div>
				</ItemTemplate>
				<SeparatorTemplate><hr /></SeparatorTemplate>
			</asp:Repeater>
		</td></tr></tbody></table>
	</div>
</asp:Content>
