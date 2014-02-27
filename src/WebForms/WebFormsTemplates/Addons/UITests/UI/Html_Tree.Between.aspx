<%@ Page Title="" Language="C#" AutoEventWireup="true" Inherits="N2.Web.UI.ContentPage" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="N2.Web" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="h" runat="server">
    <title id="t" runat="server" title='<%$ CurrentPage: Title %>' />
    <style>
        code,textarea { width:99%; height:100px; display:block }
        a.current, li.current > a { font-weight:bold; text-decoration:none; }
        li.current > a { color:Red; }
        a.current { color:Green; }
        li.open > a { font-style:italic }
        li.first { border-left:solid 5px blue; }
        li { margin-left:20px; list-style-type:circle }
    </style>
</head>
<body>
    <%= N2.Web.Link.To(CurrentPage).Text("Back") %>
    <h1>Tree.Between</h1>
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
    
    <h3>Default (trail leading to node)</h3>
    <strong><code>&lt;%= N2.Web.Tree.Between(childAA, root) %&gt;</code></strong>
    <textarea><%= FormatXml(N2.Web.Tree.Between(childAA, root))%></textarea>
    <%= N2.Web.Tree.Between(childAA, root)%>


    <h3>ClassProvider</h3>
    <strong><code>&lt;%= N2.Web.Tree.Between(childAA, root).ClassProvider(delegate(N2.ContentItem i) { return i == childA ? "current" : ""; }) %&gt;</code></strong>
    <textarea><%= FormatXml(N2.Web.Tree.Between(childAA, root).ClassProvider(delegate(N2.ContentItem i) { return i == childA ? "current" : ""; }))%></textarea>
    <%= N2.Web.Tree.Between(childAA, root).ClassProvider(delegate(N2.ContentItem i) { return i == childA ? "current" : ""; })%>

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

&lt;%= N2.Web.Tree.Between(childAA, root).ClassProvider(GimmeClassForItem) %&gt;
</pre></code></strong>
    <textarea><%= FormatXml(N2.Web.Tree.Between(childAA, root).ClassProvider(GimmeClassForItem)) %></textarea>
    <%= N2.Web.Tree.Between(childAA, root).ClassProvider(GimmeClassForItem) %>


    <h3>ExcludeRoot</h3>
    <strong><code>&lt;%= N2.Web.Tree.Between(childAA, root).ExcludeRoot(true) %&gt;</code></strong>
    <textarea><%= N2.Web.Tree.Between(childAA, root).ExcludeRoot(true) %></textarea>
    <%= N2.Web.Tree.Between(childAA, root).ExcludeRoot(true) %>


    <h3>Filters</h3>
    <strong><code>&lt;%= N2.Web.Tree.Between(CurrentPage, N2.Find.StartPage).Filters(new N2.Collections.NavigationFilter()) %&gt;</code></strong>
    <textarea><%= FormatXml(N2.Web.Tree.Between(CurrentPage, N2.Find.StartPage).Filters(new N2.Collections.NavigationFilter())) %></textarea>
    <%= N2.Web.Tree.Between(CurrentPage, N2.Find.StartPage).Filters(new N2.Collections.NavigationFilter()) %>


    <h3>LinkProvider</h3>
    <strong><code>&lt;%= N2.Web.Tree.Between(childAA, root).LinkProvider(delegate(N2.ContentItem i) { return N2.Web.Link.To(i).Class(i == childA ? "current" : ""); }) %&gt;</code></strong>
    <textarea><%= FormatXml(N2.Web.Tree.Between(childAA, root).LinkProvider(delegate(N2.ContentItem i) { return N2.Web.Link.To(i).Class(i == childA ? "current" : ""); }))%></textarea>
    <%= N2.Web.Tree.Between(childAA, root).LinkProvider(delegate(N2.ContentItem i) { return N2.Web.Link.To(i).Class(i == childA ? "current" : ""); })%>

    
    <h3>OpenTo</h3>
    <strong><code>&lt;%= N2.Web.Tree.Between(childAA, root).OpenTo(childA) %&gt;</code></strong>
    <textarea><%= FormatXml(N2.Web.Tree.Between(childAA, root).OpenTo(childA))%></textarea>
    <%= N2.Web.Tree.Between(childAA, root).OpenTo(childA)%>

    
    <h3>Current Site</h3>
    <strong><code>&lt;%= N2.Web.Tree.Between(CurrentPage, N2.Find.StartPage) %&gt;</code></strong>
    <textarea><%= FormatXml(N2.Web.Tree.Between(CurrentPage, N2.Find.StartPage)) %></textarea>
    <%= N2.Web.Tree.Between(CurrentPage, N2.Find.StartPage)%>
    

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
