<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Result.ascx.cs" Inherits="N2.Templates.UI.Parts.Result" %>
<n2:EditableDisplay runat="server" PropertyName="Title" />
<n2:Box runat="server">
    <asp:Repeater ID="rptAnswers" runat="server">
        <HeaderTemplate>
            <div class='question'><%# CurrentItem.Question.Title %></div>
        </HeaderTemplate>
        <ItemTemplate>
            <label class="alternative">
                <%# Eval("Title") %>
            </label>
            <div class="total"><div class="bar" style="width:<%# 100 * (int)Eval("Answers") / Total %>%"><%# 100 * (int)Eval("Answers") / Total %>%</div></div>
        </ItemTemplate>
    </asp:Repeater>
</n2:Box>