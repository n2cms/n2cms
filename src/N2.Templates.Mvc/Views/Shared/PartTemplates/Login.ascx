<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ModelViewUserControl<LoginModel, LoginItem>" %>
<div class="uc">
	<h4><%=Model.CurrentItem.Title%></h4>
	<div class="box">
		<div class="inner">
			<%if(Model.LoggedIn){%>
			<%=Html.ActionLink<LoginController>(c => c.Logout(), Model.CurrentItem.LogoutText)%>
			<%}else{%>
			<%using(Html.BeginForm<LoginController>(c => c.Login(null, null, null), FormMethod.Post)){%>
			<div class="ff username">
				<label for="userName"><%=GetLocalResourceObject("UserName") %></label>
				<input id="userName" name="userName" class="tb" />
			</div>
			<div class="ff password">
				<label for="password"><%=GetLocalResourceObject("Password") %></label>
				<input id="password" name="password" class="tb" type="password" />
			</div>
			<div class="ff remember">
				<label for="remember"><%=GetLocalResourceObject("RememberMe") %></label>
				<input id="remember" name="remember" class="tb" type="checkbox" />
			</div>
			<div class="bf">
				<%=Html.AntiForgeryToken("login")%>
				<input value="Login" type="submit" />
				<%=Html.ValidationMessage("Login.Failed")%>
				<a href="<%=CurrentItem.RegisterPage.Url%>"><%=CurrentItem.RegisterPage.Title%></a>
			</div>
			<%}%>
			<%}%>
		</div>
	</div>
</div>