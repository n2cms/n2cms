<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ViewUserControl<Teaser>" %>
<n2:EditableDisplay ID="dt" runat="server" PropertyName="Title" />
<div class="box"><div class="inner">
		<a href="<%=Model.LinkUrl%>">
			<%=this.EditableDisplay(m => m.LinkText)%>
		</a>
</div></div>