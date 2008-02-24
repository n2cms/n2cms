<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tree.aspx.cs" Inherits="N2.Edit.Navigation.Tree" meta:resourceKey="treePage" %>
<%@ Register TagPrefix="nav" TagName="ContextMenu" Src="ContextMenu.ascx" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Navigation</title>
        <link rel="stylesheet" href="../Css/All.css" type="text/css" />
        <link rel="stylesheet" href="../Css/Framed.css" type="text/css" />
        <script src="../Js/plugins.ashx" type="text/javascript" ></script>
        <script src="../Js/jquery.ui.ashx" type="text/javascript" ></script>
    </head>
<body class="navigation tree">
    <form id="form1" runat="server">
        <div id="nav" class="tree">
            <edit:Tree ID="siteTreeView" runat="server" Target="preview" />
        </div>
        <script type="text/javascript">
            $(document).ready(function(){
                $("#nav").treeview({collapsed: true});
                
                for(var i=0; i<icons.length; ++i){
                    $(".i" + i).prepend("<img src='" + icons[i] + "' alt='icon'/>");
                }
                
                var dragMemory = null;
                $("#nav li li a").draggable({
					snapDistance: 5,
					start : function(e,ui){
						dragMemory = this.rel;

						$("#nav li a").droppable({
							accept: '#nav li li',
							hoverClass: 'droppable-hover',
							tolerance: 'pointer',
							drop: function(e, ui) {
								var action = e.ctrlKey ? "copy" : "move";
								var to = this.rel;
								var from = dragMemory;
								parent.preview.location = "../paste.aspx?action="
									+ action 
									+ "&memory=" 
									+ encodeURIComponent(from)
									+ "&selected=" 
									+ encodeURIComponent(to);
							}
						});

					},
					helper: 'clone'
                });
	            
                $("#nav .locked").append("<img src='<%= VirtualPathUtility.ToAbsolute("~/Edit/Img/Ico/bullet_key.gif") %>' alt='new'/>");
                $("#nav .unpublished").append("<img src='<%= VirtualPathUtility.ToAbsolute("~/Edit/Img/Ico/bullet_arrow_down.gif") %>' alt='new'/>");
                $("#nav .expired").append("<img src='<%= VirtualPathUtility.ToAbsolute("~/Edit/Img/Ico/bullet_arrow_top.gif") %>' alt='new'/>");
            });
        </script>
        <nav:ContextMenu id="cm" runat="server" />
    </form>
</body>
</html>
