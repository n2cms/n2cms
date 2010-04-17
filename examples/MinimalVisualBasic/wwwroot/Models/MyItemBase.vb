Imports N2.Details
Namespace Models
	''' <summary>
	''' This is an abstract/MustInherit class that we can derive from on in all 
	''' situations we want edit the item's title and name.
	''' </summary>
	''' <remarks></remarks>
	<WithEditableName()> _
	<WithEditableTitle()> _
	Public MustInherit Class MyItemBase
		Inherits N2.ContentItem

	End Class
End Namespace
