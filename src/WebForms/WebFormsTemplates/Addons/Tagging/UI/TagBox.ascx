<%@ Import Namespace="N2.Addons.Tagging.Details"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TagBox.ascx.cs" Inherits="N2.Addons.Tagging.UI.TagBox" %>
<div class="box">
	<div class='inner'>
		<ul>
		<% foreach(AppliedTags tags in Categories) {%>
			<li>
				<strong><%= tags.Group.Title %>: </strong>
				<% foreach(string tag in tags.Tags) {%>
					<span class="tag"><%= tag %></span>
				<%} %>
			</li>
		<%} %>
		</ul>
	</div>
</div>