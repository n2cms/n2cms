Namespace Items

    ' This attribute marks the MyPage class for usage in the CMS
    <N2.PageDefinition("My First Page (VB)", _
                       TemplateUrl:="~/UI/MyPage-VisualBasic.aspx")> _
    Public Class MyPage
        Inherits N2.Templates.Items.AbstractContentPage ' Since we're inheriting from TextPage all it's properties are inherited as well

        <N2.Details.EditableTextBox("Author", 160, ContainerName:=N2.Templates.Tabs.Content)> _
        Public Overridable Property Author() As String
            Get
                Return GetDetail("Author", "")
            End Get
            Set(ByVal value As String)
                SetDetail("Author", value, "")
            End Set
        End Property

    End Class

End Namespace
