function CanHaveClass__CssFriendlyAdapters(element)
{
    return ((element != null) && (element.className != null));
}

function HasAnyClass__CssFriendlyAdapters(element)
{
    return (CanHaveClass__CssFriendlyAdapters(element) && (element.className.length > 0));
}

function HasClass__CssFriendlyAdapters(element, specificClass)
{
    return (HasAnyClass__CssFriendlyAdapters(element) && (element.className.indexOf(specificClass) > -1));
}

function AddClass__CssFriendlyAdapters(element, classToAdd)
{
    if (HasAnyClass__CssFriendlyAdapters(element))
    {
        if (!HasClass__CssFriendlyAdapters(element, classToAdd))
        {
            element.className = element.className + " " + classToAdd;
        }
    }
    else if (CanHaveClass__CssFriendlyAdapters(element))
    {
        element.className = classToAdd;
    }
}

function AddClassUpward__CssFriendlyAdapters(startElement, stopParentClass, classToAdd)
{
    var elementOrParent = startElement;
    while ((elementOrParent != null) && (!HasClass__CssFriendlyAdapters(elementOrParent, topmostClass)))
    {
        AddClass__CssFriendlyAdapters(elementOrParent, classToAdd);
        elementOrParent = elementOrParent.parentNode;
    }    
}

function SwapClass__CssFriendlyAdapters(element, oldClass, newClass)
{
    if (HasAnyClass__CssFriendlyAdapters(element))
    {
        element.className = element.className.replace(new RegExp(oldClass, "gi"), newClass);
    }
}

function SwapOrAddClass__CssFriendlyAdapters(element, oldClass, newClass)
{
    if (HasClass__CssFriendlyAdapters(element, oldClass))
    {
        SwapClass__CssFriendlyAdapters(element, oldClass, newClass);
    }
    else
    {
        AddClass__CssFriendlyAdapters(element, newClass);
    }
}

function RemoveClass__CssFriendlyAdapters(element, classToRemove)
{
    SwapClass__CssFriendlyAdapters(element, classToRemove, "");
}

function RemoveClassUpward__CssFriendlyAdapters(startElement, stopParentClass, classToRemove)
{
    var elementOrParent = startElement;
    while ((elementOrParent != null) && (!HasClass__CssFriendlyAdapters(elementOrParent, topmostClass)))
    {
        RemoveClass__CssFriendlyAdapters(elementOrParent, classToRemove);
        elementOrParent = elementOrParent.parentNode;
    }    
}
