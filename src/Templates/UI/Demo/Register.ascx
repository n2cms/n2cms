<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Register.ascx.cs" Inherits="Demo.Register" %>
<div class="uc register">
    <h4><asp:Literal ID="ltTitle" runat="server" Text="<%$ CurrentItem: Title %>" /></h4>
    <div class="box"><div class="inner">
    
		<asp:Literal ID="ltText" runat="server" Text="<%$ CurrentItem: Text %>" />
		
        <asp:PlaceHolder ID="plhSubmit" runat="server"> 
			<asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" Text="Email (required)" />
			<asp:TextBox ID="txtEmail" runat="server" ValidationGroup="Register" Width="98%" />
			<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtEmail" ErrorMessage="need email" Display="Dynamic" ValidationGroup="Register" />
			<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtEmail" ErrorMessage="should at least resemble an email address" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic" ValidationGroup="Register" />
            
			<asp:Label ID="lblPraise" runat="server" AssociatedControlID="txtPraise" Text="Message (optional)" />
			<asp:TextBox ID="txtPraise" runat="server" TextMode="MultiLine" ValidationGroup="Register" Width="98%" />
            
			<asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" ValidationGroup="Register" />
			<asp:LinkButton ID="btnAutoLogin" Visible="<%$ HasValue: Username %>" runat="server" Text=" I'd rather not register, just let me in &raquo;" OnClick="btnAutoLogin_click" CausesValidation="false" />
        </asp:PlaceHolder>
    </div></div>
</div>
