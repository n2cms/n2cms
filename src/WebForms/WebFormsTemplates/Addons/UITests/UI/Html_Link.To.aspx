<%@ Page Title="" Language="C#" AutoEventWireup="true" Inherits="N2.Web.UI.ContentPage" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="h" runat="server">
    <title id="t" runat="server" title='<%$ CurrentPage: Title %>' />
    <style>code,textarea { width:99%; dipslay:block } ul { margin-left:20px; list-style-type:circle }</style>
</head>
<body>
    <%= N2.Web.Link.To(CurrentPage).Text("Back") %>
    <h1>Link.To</h1>
    <p>Link.To is a very simple fluent interface for building links. It's a slightly more terse an alternative to using web controls, string.Format or MVC helpers.</p>

    <h3>Default</h3>
    <strong><code>&lt;%= N2.Web.Link.To(CurrentPage) %&gt;</code></strong>
    <textarea><%= N2.Web.Link.To(CurrentPage) %></textarea>
    <%= N2.Web.Link.To(CurrentPage) %>

    <h3>AddQuery</h3>
    <strong><code>&lt;%= N2.Web.Link.To(CurrentPage).AddQuery("hello", "world") %&gt;</code></strong>
    <textarea><%= N2.Web.Link.To(CurrentPage).AddQuery("hello", "world") %></textarea>
    <%= N2.Web.Link.To(CurrentPage).AddQuery("hello", "world") %>
    <hr />

    <h3>Query</h3>
    <strong><code>&lt;%= N2.Web.Link.To(CurrentPage).Query("hello=world") %&gt;</code></strong>
    <textarea><%= N2.Web.Link.To(CurrentPage).Query("hello=world") %></textarea>
    <%= N2.Web.Link.To(CurrentPage).Query("hello=world") %>
    <hr />

    <h3>Attribute</h3>
    <strong><code>&lt;%= N2.Web.Link.To(CurrentPage).Attribute("onclick", "return confirm('sure?');") %&gt;</code></strong>
    <textarea><%= N2.Web.Link.To(CurrentPage).Attribute("onclick", "return confirm('sure?');") %></textarea>
    <%= N2.Web.Link.To(CurrentPage).Attribute("onclick", "return confirm('sure?');") %>

    <h3>Class</h3>
    <strong><code>&lt;%= N2.Web.Link.To(CurrentPage).Class("link") %&gt;</code></strong>
    <textarea><%= N2.Web.Link.To(CurrentPage).Class("link") %></textarea>
    <%= N2.Web.Link.To(CurrentPage).Class("link") %>

    <h3>Target</h3>
    <strong><code>&lt;%= N2.Web.Link.To(CurrentPage).Target("_top") %&gt;</code></strong>
    <textarea><%= N2.Web.Link.To(CurrentPage).Target("_top") %></textarea>
    <%= N2.Web.Link.To(CurrentPage).Target("_top") %>
    <hr />

    <h3>Text</h3>
    <strong><code>&lt;%= N2.Web.Link.To(CurrentPage).Text("Link to " + CurrentPage.Title) %&gt;</code></strong>
    <textarea><%= N2.Web.Link.To(CurrentPage).Text("Link to " + CurrentPage.Title) %></textarea>
    <%= N2.Web.Link.To(CurrentPage).Text("Link to " + CurrentPage.Title) %>
    <hr />

    <h3>Title</h3>
    <strong><code>&lt;%= N2.Web.Link.To(CurrentPage).Title("Opens in top window") %&gt;</code></strong>
    <textarea><%= N2.Web.Link.To(CurrentPage).Title("Opens in top window") %></textarea>
    <%= N2.Web.Link.To(CurrentPage).Title("Opens in top window") %>
    <hr />


</body>
</html>
