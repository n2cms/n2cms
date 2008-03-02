''' <summary>
''' This is a custom content data container class. 
''' </summary>
''' <remarks>
''' Since this class derives from <see cref="N2.ContentItem"/> N2 will pick up 
''' this class and make it available when we create items in edit mode. 
''' 
''' Another thing to notice is that in addition to Text defined to be editable 
''' in this class it's Title and Name are also editable. This is because of the
''' abstract/MustInherit base class <see cref="MyItemBase"/> it inherits.
''' </remarks>
<N2.Definition("Default page")> _
Public Class PageItem
    Inherits MyItemBase

    <N2.Details.EditableFreeTextArea("Text", 100)> _
    Public Overridable Property Text() As String
        Get
            Return GetDetail("Text")
        End Get
        Set(ByVal value As String)
            SetDetail("Text", value)
        End Set
    End Property

End Class
