Imports N2.Details

''' <summary>
''' This is an abstract/MustInherit class that we can derive from on in all 
''' situations we want edit the item's title and name.
''' </summary>
''' <remarks></remarks>
<WithEditable("Name", GetType(N2.Web.UI.WebControls.NameEditor), "Text", 20, "Name")> _
Public MustInherit Class MyItemBase
    Inherits N2.ContentItem

    <DisplayableLiteral()> _
    <EditableTextBox("Title", 10)> _
    Public Overrides Property Title() As String
        Get
            Return MyBase.Title
        End Get
        Set(ByVal value As String)
            MyBase.Title = value
        End Set
    End Property

End Class
