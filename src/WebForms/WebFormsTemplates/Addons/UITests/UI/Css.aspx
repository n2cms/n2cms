<%@ Page Title="" Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="h" runat="server">
    <script type="text/javascript" src="/N2/Resources/js/jquery-1.3.2.js"></script>
    <script type="text/javascript" src="/N2/Resources/js/plugins/jquery.n2glow.js"></script>
    <style>
        body { background-color:Silver }
        div { background: #fff url(/N2/Resources/Img/toolbar_button_bg.png) no-repeat scroll 0 0; height:30px; }
        div:hover { background-image: url(/N2/Resources/Img/toolbar_button_bg_hover.png); }
        .r1 { background-position: 0% 10px }
        .r2 { background-position: 33% 10px }
        .r3 { background-position: 66% 10px }
        .r4 { background-position: 99% 10px }
        .r5 { background-position: 0px 10px }
        .r6 { background-position: 25px 10px }
        .c1 { width: 20px }
        .c1 { width: 40px }
        .c2 { width: 60px }
        .c3 { width: 80px }
        .c4 { width: 100px }
    </style>
</head>
<body>
    <table>
        <tr>
            <td><div class="r1 c1"></div></td>
            <td><div class="r1 c2"></div></td>
            <td><div class="r1 c3"></div></td>
            <td><div class="r1 c4"></div></td>
        </tr>
        <tr>
            <td><div class="r2 c1"></div></td>
            <td><div class="r2 c2"></div></td>
            <td><div class="r2 c3"></div></td>
            <td><div class="r2 c4"></div></td>
        </tr>
        <tr>
            <td><div class="r3 c1"></div></td>
            <td><div class="r3 c2"></div></td>
            <td><div class="r3 c3"></div></td>
            <td><div class="r3 c4"></div></td>
            <td><div class="r3 c5"></div></td>
        </tr>
        <tr>
            <td><div class="r4 c1"></div></td>
            <td><div class="r4 c2"></div></td>
            <td><div class="r4 c3"></div></td>
            <td><div class="r4 c4"></div></td>
        </tr>
        <tr>
            <td><div class="r5 c1"></div></td>
            <td><div class="r5 c2"></div></td>
            <td><div class="r5 c3"></div></td>
            <td><div class="r5 c4"></div></td>
        </tr>
        <tr>
            <td><div class="r6 c1"></div></td>
            <td><div class="r6 c2"></div></td>
            <td><div class="r6 c3"></div></td>
            <td><div class="r6 c4"></div></td>
        </tr>
    </table>
    <script type="text/javascript">
        $(document).ready(function() {
            $("div").n2glow();
        });
    </script>
</body>
</html>
