<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<Poll>" %>
<%@ Import Namespace="N2"%>
<div class="uc">
	<%= Html.DisplayContent(m => m.Title) %>

	<div class="box">
		<div class="inner">
			<%using(Html.BeginForm("Submit", "Poll")){%>
				<p><%=Model.Question.Title%></p>
				<p>
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
				</p>
				<div class="buttons">
					<input type="submit" value="<%=(string)GetLocalResourceObject("btnSubmit.Text") %>" /><br />
					<a href="<%= N2.Web.Url.Parse(Html.CurrentPage().Url).AppendQuery("p=show") %>"><%=(string)GetLocalResourceObject("hlDisplay.Text") %></a>
				</div>
			<%}%>
		</div>
	</div>
</div>