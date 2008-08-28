<%@ Page Language="C#" MasterPageFile="~/DocSite.Master" AutoEventWireup="true" CodeBehind="DocSiteSearch.aspx.cs" Inherits="N2.DocSite.DocSiteSearch" Title="N2.DocSite - Search" 
Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceholder" runat="server">
  <h2 id="docsite_page_header"><asp:Localize runat="server" ID="searchResultsHeaderLocalize" Text="Search Results" meta:resourcekey="searchResultsHeaderLocalize" /></h2>
	<div class="content_padding">
	  <asp:Panel runat="server" ID="statisticsPanel" CssClass="content_margin_bottom" meta:resourcekey="statisticsPanelResource1">
		  <h3><asp:Label runat="server" ID="resultsCountLabel" /></h3>
		  <h6 style="padding-top: 5px;"><em><%= HttpUtility.HtmlEncode(Request.QueryString["q"] ?? "") %></em>
		    &nbsp;<asp:ImageButton runat="server" ID="browseImageButton" SkinID="BrowseImageButton" CausesValidation="False" OnClick="browseImageButton_Click" 
		                           ToolTip='<%$ Resources:General,DocSiteBrowseButtonToolTip %>' meta:resourcekey="browseImageButtonResource1" />
		  </h6>
		</asp:Panel>
		<script runat="server">
      private int position = 0;
		</script>
		<asp:GridView runat="server" ID="resultsGridView" AutoGenerateColumns="False" DataSourceID="resultsDataSource" SkinID="SearchResultsGridView" meta:resourcekey="resultsGridViewResource1">
			<Columns>
				<asp:TemplateField meta:resourcekey="TemplateFieldResource1">
					<ItemTemplate>
					  <asp:Label runat="server" ID="positionLabel" SkinID="searchResultPosition" meta:resourcekey="positionLabelResource1"><%# ++position + (resultsGridView.PageIndex * resultsGridView.PageSize) %></asp:Label>)
					  <asp:Label runat="server" ID="rankLabel" SkinID="searchResultRank" meta:resourcekey="rankLabelResource1"><%# "[" + Eval("Rank") + "]" %></asp:Label>
					  <asp:HyperLink runat="server" ID="titleHyperlink" SkinID="searchResultTitle" 
					                 ToolTip='<%# Eval("Title") %>' NavigateUrl='<%# DocSiteFramePath + "?helpfile=" + HttpUtility.HtmlEncode((string) Eval("VirtualPath")) %>' meta:resourcekey="titleHyperlinkResource1"><%# HttpUtility.HtmlEncode((string) Eval("FullTitle")) %></asp:HyperLink>
					  &nbsp;<asp:Label runat="server" ID="localeLabel" SkinID="searchResultLocale" meta:resourcekey="localeLabelResource1"><%# "(" + HttpUtility.HtmlEncode((string) Eval("Locale")) + ")" %></asp:Label><br />
					  <asp:Label runat="server" ID="abstractLabel" SkinID="searchResultAbstract" meta:resourcekey="abstractLabelResource1"><%# HttpUtility.HtmlEncode((string) Eval("Abstract")) %></asp:Label><br />
					  
					  <div class="small special">
					    (<asp:Label runat="server" ID="nameLabel" SkinID="searchResultName" meta:resourcekey="nameLabelResource1"
					                Text='<%# (string.IsNullOrEmpty((string) Eval("ApiName"))) ? "" : HttpUtility.HtmlEncode((string) Eval("ApiName")) + ", " %>'
               /><asp:Label runat="server" ID="locationLabel" SkinID="searchResultLocation" meta:resourcekey="locationLabelResource1"
                            Text='<%# (string.IsNullOrEmpty(Eval("ApiLocation") as string)) ? Eval("VirtualPath") : HttpUtility.HtmlEncode((string) Eval("ApiLocation")) %>' />)
					  </div>
					</ItemTemplate>
				</asp:TemplateField>
			</Columns>
		</asp:GridView>
		
		<asp:ObjectDataSource runat="server" ID="resultsDataSource" SelectMethod="SearchList" OnSelected="resultsDataSource_Selected"
		                      TypeName="DaveSexton.DocProject.DocSites.DocSiteSearch, DaveSexton.DocProject.DocSites, Version=1.0.1.0, Culture=neutral, PublicKeyToken=af1a4bab65cc4ece">
			<SelectParameters>
				<asp:QueryStringParameter Name="text" Type="String" QueryStringField="q" />
			</SelectParameters>
		</asp:ObjectDataSource>
		
		<asp:Panel runat="server" ID="searchHelpPanel" Visible="False" meta:resourcekey="searchHelpPanelResource1">
		  <table class="table_info" rules="all" frame="void" cellpadding="8" cellspacing="0" width="600px">
		    <asp:Localize runat="server" ID="helpTableLocalize" Mode="passThrough" meta:resourcekey="helpTableLocalize" />
		  </table>
		</asp:Panel>
	</div>
</asp:Content>
