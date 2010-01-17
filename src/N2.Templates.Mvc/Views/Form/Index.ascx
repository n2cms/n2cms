<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ModelViewUserControl<FormModel, Form>" %>
<%@ Import Namespace="System.Resources"%>

<%if(Model.FormSubmitted){%>
	<%=ContentHtml.Display(m => m.SubmitText)%>
<%}else{%>
	<%using(Html.BeginForm<FormController>(c => c.Submit(null), FormMethod.Post)){%>
		<%=ContentHtml.Display(m => m.Title)%>
		<%=ContentHtml.Display(m => m.IntroText)%>

		<div class="inputForm">
			<%foreach(var formElement in Model.Elements){%>
				<div class="row cf">
					<label class="label" for="<%=formElement.ElementID %>"><%=formElement.QuestionText%></label>
					<%=formElement.CreateHtmlElement()%>
				</div>
			<%}%>
		</div>

		<%=Html.SubmitButton("Submit", "Send")%>
	<%}%>
<%}%>