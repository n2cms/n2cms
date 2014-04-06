<%@ control language="C#" autoeventwireup="true" codebehind="Login.ascx.cs" inherits="N2.Templates.UI.Parts.Login" %>
<n2:hn text="<%$ CurrentItem: Title %>" visible="<%$ HasValue: Title %>" runat="server" />
<n2:box id="boxLogin" runat="server">
	<asp:LoginStatus ID="Status" runat="server" LogoutText="<%$ CurrentItem: LogoutText %>" LoginText="" />
	<asp:Login ID="LoginBox" runat="server" FailureText="<%$ CurrentItem: FailureText %>" VisibleWhenLoggedIn="false">
		<LayoutTemplate>
		    <asp:Panel runat="server" DefaultButton="LoginButton">
			    <div class="ff username">
				    <asp:Label Text="User Name" ID="UserNameLabel" runat="server" AssociatedControlID="UserName" />
				    <asp:TextBox ID="UserName" runat="server" CssClass="tb"/>
				    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
					    ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="Login1"
					    Text="*" Display="Dynamic" />
			    </div>
			    <div class="ff password">
				    <asp:Label Text="Password" ID="PasswordLabel" runat="server" AssociatedControlID="Password" />
				    <asp:TextBox ID="Password" runat="server" TextMode="Password" CssClass="tb" />
				    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
					    ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="Login1"
					    Text="*" Display="Dynamic" />
			    </div>
			    <div class="ff remember">
				    <asp:CheckBox ID="RememberMe" runat="server" Text="Remember me" />
			    </div>
			    <div class="bf">
				    <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Log In" ValidationGroup="Login1" />
				    <asp:Literal ID="FailureText" runat="server" EnableViewState="False" />
				    <n2:Display runat="server" PropertyName="RegisterPage" />
			    </div>
			</asp:Panel>
		</LayoutTemplate>
	</asp:Login>
</n2:box>
