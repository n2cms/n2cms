var collapseClass = "AspNet-TreeView-Collapse";
var expandClass = "AspNet-TreeView-Expand";
var showClass = "AspNet-TreeView-Show";
var hideClass = "AspNet-TreeView-Hide";

function IsExpanded__AspNetTreeView(element)
{
    return (HasClass__CssFriendlyAdapters(element, collapseClass));
}

function TogglePlusMinus__AspNetTreeView(element, showPlus)
{
    if (HasAnyClass__CssFriendlyAdapters(element))
    {
        var showPlusLocal = IsExpanded__AspNetTreeView(element);
        if ((typeof(showPlus) != "undefined") && (showPlus != null))
        {
            showPlusLocal = showPlus;
        }    
        var oldClass = showPlusLocal ? collapseClass : expandClass;
        var newClass = showPlusLocal ? expandClass : collapseClass;
        SwapClass__CssFriendlyAdapters(element, oldClass, newClass);
    }
}

function ToggleChildrenDisplay__AspNetTreeView(element, collapse)
{
    if ((element != null) && (element.parentNode != null) && (element.parentNode.getElementsByTagName != null))
    {    
        var childrenToHide = element.parentNode.getElementsByTagName("ul");
        var oldClass = collapse ? showClass : hideClass;
        var newClass = collapse ? hideClass : showClass;
    	for (var i=0; i<childrenToHide.length; i++)
    	{
    	    if ((childrenToHide[i].parentNode != null) && (childrenToHide[i].parentNode == element.parentNode))
    	    {
        	    SwapOrAddClass__CssFriendlyAdapters(childrenToHide[i], oldClass, newClass);
        	}
		}
	}
}

function ExpandCollapse__AspNetTreeView(sourceElement)
{
    if (HasAnyClass__CssFriendlyAdapters(sourceElement))
    {
        var expanded = IsExpanded__AspNetTreeView(sourceElement);
        TogglePlusMinus__AspNetTreeView(sourceElement, expanded);
        ToggleChildrenDisplay__AspNetTreeView(sourceElement, expanded);
    }
}
