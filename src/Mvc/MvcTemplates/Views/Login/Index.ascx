<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.ContentViewUserControl<LoginModel, LoginItem>" %>
<div class="uc login">
	<h4><%=Model.CurrentItem.Title%></h4>
	<div class="box">
		<div class="inner">
			<%if(Model.LoggedIn){%>
			<%=Html.ActionLink(Model.CurrentItem.LogoutText, "Logout", "Login")%>
			<%}else{%>
			<%using(Html.BeginForm("Login", "Login", FormMethod.Post, new  { @class = "inputForm" })){%>
			<div class="row username cf">
				<label for="userName"><%=GetLocalResourceObject("UserName") %></label>
				<input id="userName" name="userName" class="tb" />
			</div>
			<div class="row password cf">
				<label for="password"><%=GetLocalResourceObject("Password") %></label>
				<input id="password" name="password" class="tb" type="password" />
			</div>
			<div class="row remember cf">
				<input id="remember" name="remember" class="cb" type="checkbox" />
				<label for="remember"><%=GetLocalResourceObject("RememberMe") %></label>
			</div>
			<div class="row cf">
				<%=Html.AntiForgeryToken()%>
				<input value="Login" type="submit" />
				<%=Html.ValidationMessage("Login.Failed")%>
                <% if (CurrentItem.RegisterPage != null) { %>
				<%= Html.ActionLink(CurrentItem.RegisterPage) %>
                <% } %>
			</div>
			<%}%>
			<%}%>
		</div>
	</div>
</div>