<%@ Page Language="C#" MasterPageFile="../Site.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage<NewsContainer>" Title="" %>

<asp:Content ContentPlaceHolderID="PostContent" runat="server">
    <div class="list">
    <%for(var i = 0; i < Model.NewsItems.Count; i++){%>
        <div class="item i<%=i%> a<%=i % 2%>">
            <span class="date"><%=Model.NewsItems[i].Published%></span>
            <a href="<%=Model.NewsItems[i].Url%>"><%=Model.NewsItems[i].Title%></a>
            <p><%=Model.NewsItems[i].Introduction%></p>
        </div>
    <%}%>
    </div>
</asp:Content>