<%@ Page Title="" Language="C#" AutoEventWireup="true" Inherits="N2.Web.UI.ContentPage" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="N2.Web" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="h" runat="server">
    <title id="t" runat="server" title='<%$ CurrentPage: Title %>' />
    <style>
        code,textarea { width:99%; height:100px; dipslay:block }
        a.current, li.current > a { font-weight:bold; text-decoration:none; }
        li.current > a { color:Red; }
        a.current { color:Green; }
        li.open > a { font-style:italic }
        li.first { border-left:solid 5px blue; }
    </style>
</head>
<body>
    <%= N2.Web.Link.To(CurrentPage).Text("Back") %>
    <h1>Tree.From</h1>
    <p>Tree is a simple fluent interface to help creating html for node hierarchies. It's an alternative to site map providers, repeaters and custom code.</p>

    <%
        // Nodes used for testing
        N2.ContentItem root = Create("root", null);
        N2.ContentItem childA = Create("childA", root);
        N2.ContentItem childB = Create("childB", root);
        N2.ContentItem childAA = Create("childAA", childA);
        N2.ContentItem childAB = Create("childAB", childA);
        N2.ContentItem childAAA = Create("childAAA", childAA);
        N2.ContentItem childAAB = Create("childAAB", childAA);
        
         %>
    
    <h3>Default (from root)</h3>
    <strong><code>&lt;%= N2.Web.Tree.From(root) %&gt;</code></strong>
    <textarea><%= FormatXml(N2.Web.Tree.From(root))%></textarea>
    <%= N2.Web.Tree.From(root)%>
    
    <h3>Default (from root until depth)</h3>
    <strong><code>&lt;%= N2.Web.Tree.From(root, 2) %&gt;</code></strong>
    <textarea><%= FormatXml(N2.Web.Tree.From(root, 2))%></textarea>
    <%= N2.Web.Tree.From(root, 2)%>


    <h3>ClassProvider</h3>
    <strong><code>&lt;%= N2.Web.Tree.From(root, 2).ClassProvider(delegate(N2.ContentItem i) { return i == childA ? "current" : ""; }) %&gt;</code></strong>
    <textarea><%= FormatXml(N2.Web.Tree.From(root, 2).ClassProvider(delegate(N2.ContentItem i) { return i == childA ? "current" : ""; }))%></textarea>
    <%= N2.Web.Tree.From(root, 2).ClassProvider(delegate(N2.ContentItem i) { return i == childA ? "current" : ""; })%>

    <h3>ClassProvider deluxe</h3>
    <script runat="server">
        public string GimmeClassForItem(N2.ContentItem current)
        {
            return current.Parent != null && current.Parent.GetChildren()[0] == current ? "first" : "";
        }
    </script> 
    <strong><code><pre>public string GimmeClassForItem(N2.ContentItem current)
{
    return current.Parent != null && current.Parent.GetChildren()[0] == current ? "first" : "";
}

&lt;%= N2.Web.Tree.From(root, 2).ClassProvider(GimmeClassForItem) %&gt;
</pre></code></strong>
    <textarea><%= FormatXml(N2.Web.Tree.From(root, 2).ClassProvider(GimmeClassForItem)) %></textarea>
    <%= N2.Web.Tree.From(root, 2).ClassProvider(GimmeClassForItem) %>


    <h3>ExcludeRoot</h3>
    <strong><code>&lt;%= N2.Web.Tree.From(root, 2).ExcludeRoot(true) %&gt;</code></strong>
    <textarea><%= N2.Web.Tree.From(root, 2).ExcludeRoot(true) %></textarea>
    <%= N2.Web.Tree.From(root, 2).ExcludeRoot(true) %>


    <h3>Filters</h3>
    <strong><code>&lt;%= N2.Web.Tree.From(N2.Find.StartPage, 2).Filters(new N2.Collections.NavigationFilter()) %&gt;</code></strong>
    <textarea><%= FormatXml(N2.Web.Tree.From(N2.Find.StartPage, 2).Filters(new N2.Collections.NavigationFilter())) %></textarea>
    <%= N2.Web.Tree.From(N2.Find.StartPage, 2).Filters(new N2.Collections.NavigationFilter()) %>


    <h3>LinkProvider</h3>
    <strong><code>&lt;%= N2.Web.Tree.From(root, 2).LinkProvider(delegate(N2.ContentItem i) { return N2.Web.Link.To(i).Class(i == childA ? "current" : ""); }) %&gt;</code></strong>
    <textarea><%= FormatXml(N2.Web.Tree.From(root, 2).LinkProvider(delegate(N2.ContentItem i) { return N2.Web.Link.To(i).Class(i == childA ? "current" : ""); }))%></textarea>
    <%= N2.Web.Tree.From(root, 2).LinkProvider(delegate(N2.ContentItem i) { return N2.Web.Link.To(i).Class(i == childA ? "current" : ""); })%>

    
    <h3>OpenTo</h3>
    <strong><code>&lt;%= N2.Web.Tree.From(root, 2).OpenTo(childA) %&gt;</code></strong>
    <textarea><%= FormatXml(N2.Web.Tree.From(root, 2).OpenTo(childA))%></textarea>
    <%= N2.Web.Tree.From(root, 2).OpenTo(childA)%>

    
    <h3>Current Site</h3>
    <strong><code>&lt;%= N2.Web.Tree.From(N2.Find.StartPage, 2) %&gt;</code></strong>
    <textarea><%= FormatXml(N2.Web.Tree.From(N2.Find.StartPage, 2)) %></textarea>
    <%= N2.Web.Tree.From(N2.Find.StartPage, 2)%>
    

</body>
</html>
<script runat="server">

    private string FormatXml(object xml)
    {
        System.Xml.XmlDocument xd = new System.Xml.XmlDocument();
        xd.LoadXml(xml.ToString());

        StringBuilder sb = new StringBuilder();
        
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.OmitXmlDeclaration = true;
        using (XmlWriter xw = XmlWriter.Create(sb, settings))
        {
            xd.WriteTo(xw);
        }
        return sb.ToString();
    }

    public N2.ContentItem Create(string name, N2.ContentItem parent)
    {
        N2.ContentItem item = new N2.Addons.UITests.Items.AdaptiveItemPage();
        item.Name = name;
        item.Title = name;
        item.AddTo(parent);
        return item;
    }

</script>
