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
				toolbarSelect("tree");
            
                $("#nav").SimpleTree({
					success: function(el){
						toDraggable(el);
						n2nav.refreshLinks(el);
						setUpContextMenu(el);
					}
				});

                var dragMemory = null;
                function toDraggable(container){
					$("a", container).draggable({
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
									parent.preview.location = "../paste.aspx?action=" + action 
										+ "&memory=" + encodeURIComponent(from)
										+ "&selected=" + encodeURIComponent(to);
								}
							});

						},
						helper: 'clone'
					});
		        }
		        toDraggable("#nav li li");
		        
                $("#nav .locked").append("<img src='<%= N2.Utility.ToAbsolute("~/Edit/Img/Ico/bullet_key.gif") %>' alt='secured'/>");
                $("#nav .unpublished").append("<img src='<%= N2.Utility.ToAbsolute("~/Edit/Img/Ico/bullet_arrow_down.gif") %>' alt='pending publish'/>");
                $("#nav .expired").append("<img src='<%= N2.Utility.ToAbsolute("~/Edit/Img/Ico/bullet_arrow_top.gif") %>' alt='expired'/>");
            });
        </script>
        <nav:ContextMenu id="cm" runat="server" />
    </form>
</body>
</html>
