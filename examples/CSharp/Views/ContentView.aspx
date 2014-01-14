<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContentView.aspx.cs" Inherits="CSharp.Views.ContentView" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <n2:SlidingCurtain runat="server">
            <n2:ControlPanel runat="server" />
        </n2:SlidingCurtain>
        <div>
            <n2:Display runat="server" PropertyName="Title" />
            <n2:Display runat="server" PropertyName="Text" />
        </div>
    </form>
</body>
</html>
