<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ViewUserControl<Poll>" %>
<%@ Import Namespace="N2"%>

<n2:EditableDisplay runat="server" PropertyName="Title" />

<div class="box">
	<div class="inner">
		<%using(Html.BeginForm<PollController>(c => c.Submit(null))){%>
			<p><%=Model.Question.Title%></p>
			<%foreach(var option in Model.Question.Options){ %>
				<input type="radio" name="selectedItem" value="<%=option.ID%>" id="poll_<%=Model.ID + "_" + option.ID%>" />
				<label for="poll_<%=Model.ID + "_" + option.ID%>"><%=option.Title%></label>
				<%if (Request.QueryString["p"] == "show"){%>
					<span class="answer">
						<%=option.Answers%>
					</span>
				<%}%>
				<br />
			<%}%>
			<%=Html.ValidationMessage("Poll.Errors")%>
			<div class="buttons">
				<input type="submit" value="Submit" />
				<a href="<%=CurrentPage.Url%>?p=show">Display results</a>
			</div>
		<%}%>
	</div>
</div>

<style type="text/css">
	@import "<%=ResolveUrl("~/Content/Css/Poll.css")%>";
</style>
