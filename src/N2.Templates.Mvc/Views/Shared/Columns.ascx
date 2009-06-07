<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ViewUserControl<Columns>" %>

<div class="uc columns cf">
	<div class="left">
		<%=this.DroppableZone(Zones.ColumnLeft).AllowExternalManipulation()%>
	</div>
	<div class="right">
		<%=this.DroppableZone(Zones.ColumnRight).AllowExternalManipulation()%>
	</div>
</div>