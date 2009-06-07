<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ViewUserControl<BoxedTextItem>" %>

<h4><%=Model.Title%></h4>
<%--<n2:H4 Text="<%$ CurrentItem: Title %>" Visible="<%$ HasValue: Title %>" runat="server" />--%>
<div class="box"><div class="inner">
	<n2:EditableDisplay runat="server" id="t" PropertyName="Text" />
</div></div>