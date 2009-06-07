<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ModelViewUserControl<FormModel, Form>" %>
<%@ Import Namespace="System.Resources"%>
<%--<asp:MultiView ID="mv" runat=server ActiveViewIndex="0">
    <asp:View ID="View1" runat="server">
		<n2:Display runat="server" PropertyName="Title" />
		<n2:Display runat="server" PropertyName="IntroText" />
        <div class="inputForm">
            <n2:Zone id="zq" runat="server" ZoneName="Questions" />
        </div>
        <asp:Button Text="Send" ID="btnSubmit" runat="server" OnCommand="btnSubmit_Command" meta:resourcekey="btnSubmitResource1" />
    </asp:View>
    <asp:View ID="View2" runat="server" EnableViewState="false">
        <n2:Display runat="server" PropertyName="SubmitText" />
    </asp:View>
</asp:MultiView>--%>

<%if(Model.FormSubmitted){%>
	<%=this.Display(m => m.SubmitText)%>
<%}else{%>
	<%using(Html.BeginForm<FormController>(c => c.Submit(null), FormMethod.Post)){%>
		<%=this.Display(m => m.Title)%>
		<%=this.Display(m => m.IntroText)%>

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