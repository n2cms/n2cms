<%@ Page MasterPageFile="../Framed.master" Language="C#" AutoEventWireup="true" CodeBehind="Bulk.aspx.cs" Inherits="N2.Management.Content.Export.Bulk" EnableViewState="true" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
    
<asp:Content ID="CH" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="CT" ContentPlaceHolderID="Toolbar" runat="server">
    <edit:CancelLink ID="hlCancel" runat="server">Close</edit:CancelLink>
</asp:Content>
<asp:Content ID="CC" ContentPlaceHolderID="Content" runat="server">
    <n2:tabpanel id="tpUpload" runat="server" ToolTip="Upload">
        <div>
            <asp:Label ID=lblImportFile" AssociatedControlID="fuImportFile" Text="Import file" />
            <asp:FileUpload ID="fuImportFile" runat="server" />
            <asp:RequiredFieldValidator ID="rfvImportFile" Text="Import file is required" runat="server" ControlToValidate="fuImportFile" />
        </div>
        <div>
            <asp:Button ID="btnUpload" runat="server" Text="Upload and continue" OnCommand="UploadCommand" />
        </div>
        <div>
			<edit:InfoLabel id="lblLocation" Label="Import to" runat="server" />
        </div>
        <div>
			<asp:Label runat="server" Text="Type" />
			<asp:DropDownList ID="ddlTypes" DataTextField="Title" DataValueField="Discriminator" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddlTypes_OnSelectedIndexChanged" />
		</div>
		<div>
			<asp:CheckBox ID="chkFirstRow" runat="server" Text="First row is header row" AutoPostBack="true" OnCheckedChanged="chkFirstRow_OnCheckedChanged" />

			<table class="gv">
				<thead><tr>
					<asp:Repeater id="rptPreview" runat="server" DataSource="<%# FirstRow.Columns %>">
						<ItemTemplate>
							<th>
								<span><%# Container.DataItem %></span>
								<asp:DropDownList id="ddlColumnMap" runat="server" DataSource="<%# Editables %>" DataValueField="Name" DataTextField="Title" />
							</th>
						</ItemTemplate>
					</asp:Repeater>
				</tr></thead>

			<% if(Rows != null && Rows.Any()) { %>
				<%--<thead><tr>
				<% foreach(var col in FirstRow){ %>
				<% string bestSelection = GetBestSelection(col); %>
					<th>
						<span><%= col %></span>
						<select>
							<option value="">None</option>
							<% foreach (var editable in Editables) { %>
								<% if(editable.Name == bestSelection) { %>
									<option value="<%= editable.Name %>" selected="selected"><%= editable.Title %></option>
								<% } else { %>
									<option value="<%= editable.Name %>"><%= editable.Title %></option>
								<% } %>
								<% } %>
						</select>
					</th>
				<%} %>
				</tr></thead>
--%>
				<tbody>
				<% foreach(var row in Rows.Take(10)){ %>
					<tr>
					<% foreach(var col in row.Columns){ %>
						<td><%= col %></td>
					<%} %>
					</tr>
				<%} %>
				</tbody>
			<%} %>
			</table>

			<asp:Button ID="btnImport" Text="Import" runat="server" OnCommand="ImportCommand"  />

		</div>
    </n2:tabpanel>
    <n2:tabpanel id="tpImport" runat="server" ToolTip="Import" enabled="false">
    </n2:tabpanel>
</asp:Content>