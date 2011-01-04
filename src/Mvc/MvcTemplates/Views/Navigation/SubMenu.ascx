<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<SubMenuModel>" %>

<%if(Model.Visible){%>
<div class="uc">
	<h4><a href="<%=Model.BranchRoot.Url%>"><%=Model.BranchRoot.Title%></a></h4>
	<div class="box">
		<div class="inner">
			<ul class="subMenu menu">
			<%foreach(var item in Model.Items){%>
				<li class="<%=item == Model.CurrentItem ? "current" : String.Empty %>"><a href="<%=item.Url%>"><%=item.Title%></a></li>
			<%}%>
			</ul>
		</div>
	</div>
</div>
<%}%>