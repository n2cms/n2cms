<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ViewUserControl<Faq>" %>

<div class="a1">
	<%=this.Display(m => m.Title)%>
	<%=this.Display(m => m.Answer)%>
</div>