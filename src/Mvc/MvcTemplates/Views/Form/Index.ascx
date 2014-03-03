<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.ContentViewUserControl<FormModel, Form>" %>

<div class="uc">
	<%if(Model.FormSubmitted){%>
		<%=ContentHtml.DisplayContent(m => m.SubmitText)%>
	<%}else{%>
		<%= Html.ValidationSummary() %>
		<%using (Html.BeginForm("Submit", "Form", FormMethod.Post, new { enctype = "multipart/form-data", @class = "form-horizontal" })) {%>
			<%=ContentHtml.DisplayContent(m => m.Title)%>
			<%=ContentHtml.DisplayContent(m => m.IntroText)%>
			<div class="inputForm">
				<%foreach(var formElement in Model.Elements){%>
					<div class="row cf">
						<label class="label" for="<%=formElement.ElementID %>"><%=formElement.QuestionText%></label>
						<%=formElement.CreateHtmlElement()%>
					</div>
				<%}%>
			</div>
			<input type="submit" value="Send" />
		<%}%>
	<%}%>
</div>