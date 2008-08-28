<%@ Page Language="C#" MasterPageFile="~/DocSite.Master" AutoEventWireup="true" CodeBehind="DocSiteBrowseIndex.aspx.cs" Inherits="N2.DocSite.DocSiteBrowseIndex" Title="N2.DocSite - Browse Index" 
Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Register Src="~/Controls/DocSiteLetterBar.ascx" TagName="DocSiteLetterBar" TagPrefix="DocSite" %>

<asp:Content ID="Content4" ContentPlaceHolderID="contentPlaceholder" runat="server">
  <h2 id="docsite_page_header"><asp:Localize runat="server" ID="browseHeaderLocalize" Text="Browse Index" meta:resourcekey="browseHeaderLocalize" /></h2>
  <asp:UpdatePanel runat="server" ID="keywordsUpdatePanel">
	  <ContentTemplate>
	    <div class="docsite_page_options content_padding">
	      <div class="letter_bar">
		      <asp:LinkButton runat="server" ID="allKeywordsLinkButton" OnClick="allKeywordsLinkButton_Click" ToolTip="View all of the indexed keywords." meta:resourcekey="allKeywordsLinkButtonResource1">All Keywords</asp:LinkButton> | 
			    <DocSite:DocSiteLetterBar runat="server" ID="letterBar" OnLetterClick="letterBar_LetterClick" />
		    </div>
		    <br />
		    <asp:PlaceHolder runat="server" ID="keywordsCreateIndexProgressPlaceholder">
		      <asp:UpdateProgress runat="server" ID="keywordsUpdateProgress" AssociatedUpdatePanelID="keywordsUpdatePanel">
	          <ProgressTemplate>
	            <div class="update_progress">
        		    <asp:Image runat="server" ID="createIndexWaitImage" SkinID="DocSitePleaseWait" ImageAlign="Left" ToolTip='<%$ Resources:General,PleaseWait %>' meta:resourcekey="createIndexWaitImageResource1" />
                <h5><asp:Localize runat="server" ID="indexCreationPleaseWaitLine1Localize" Text='<%$ Resources:General,PleaseWaitGenerateIndexLine1 %>' /></h5>
                <span class="update_progress_message"><asp:Localize runat="server" ID="indexCreationPleaseWaitLine2Localize" Text='<%$ Resources:General,PleaseWaitGenerateIndexLine2 %>' /></span>
	            </div>
	          </ProgressTemplate>
          </asp:UpdateProgress>
		    </asp:PlaceHolder>
        <asp:Repeater runat="server" ID="userKeywordsRepeater" DataSource='<%# Keywords %>'>
          <HeaderTemplate>
            <div id="docsite_index_keywordslist">
              <asp:ImageButton runat="server" ID="searchImageButton" SkinID="SearchImageButton" CausesValidation="False" OnClick="searchImageButton_Click" Visible='<%# ShowSearchButton %>' 
                               ToolTip='<%$ Resources:General,DocSiteSearchButtonToolTip %>' meta:resourcekey="searchImageButtonResource1" />
              <strong><asp:LinkButton runat="server" ID="keywordsLinkButton" OnClick="keywordsLinkButton_Click" ToolTip="Show the files that contain all of these keywords." meta:resourcekey="keywordsLinkButtonResource1">Keywords:</asp:LinkButton></strong>&nbsp;
          </HeaderTemplate>
          <ItemTemplate>
            <span class="docsite_index_keyword">
              <asp:Label runat="server" ID="keywordLabel" CssClass="special" ForeColor='<%# (!IsKeywordIndexed((string) Container.DataItem)) ? Color.Red : Color.Empty %>'
                    ToolTip='<%# (!IsKeywordIndexed((string) Container.DataItem)) ? GetLocalResourceObject("keywordLabel.NotFoundTitle") as string : GetLocalResourceObject("keywordLabel.FoundTitle") as string %>'><%# Container.DataItem %></asp:Label>
              <asp:ImageButton runat="server" ID="removeKeywordImageButton" SkinID="indexRemoveKeywordImageButton" ToolTip="Remove this keyword from the filter."
                              OnCommand="removeKeywordImageButton_Command" CommandName="Remove" CommandArgument='<%# Container.DataItem %>' meta:resourcekey="removeKeywordImageButtonResource1" />
            </span>&nbsp;
          </ItemTemplate>
          <FooterTemplate>
            </div><br />
          </FooterTemplate>
        </asp:Repeater>
        <asp:PlaceHolder runat="server" ID="keywordsPleaesWaitProgressPlaceholder">
		      <asp:UpdateProgress runat="server" ID="keywordsPleaseWaitUpdateProgress" AssociatedUpdatePanelID="keywordsUpdatePanel">
	          <ProgressTemplate>
	            <div class="update_progress">
        		    <asp:Image runat="server" ID="pleaseWaitImage" SkinID="DocSitePleaseWait" ImageAlign="Left" ToolTip='<%$ Resources:General,PleaseWait %>' meta:resourcekey="pleaseWaitImageResource1" />
                <h5><asp:Localize runat="server" ID="pleaseWaitLocalize" Text='<%$ Resources:General,PleaseWait %>' /></h5>
                <span class="update_progress_message"><asp:Localize runat="server" ID="pleaseWaitLine2" Text='<%$ Resources:General,PleaseWaitLine2 %>' /></span>
	            </div>
	          </ProgressTemplate>
          </asp:UpdateProgress>
		    </asp:PlaceHolder>
      </div>
      <script runat="server">
        private int keywordPosition, entryPosition;
		  </script>
		  <asp:Panel runat="server" ID="keywordEntriesPanel" Visible="False" meta:resourcekey="keywordEntriesPanelResource1">
			  <asp:GridView runat="server" ID="keywordEntriesGridView" AllowPaging="True" AutoGenerateColumns="False" SkinID="IndexGridView" 
										  Width="100%" PageSize="20" DataSourceID="keywordEntriesDataSource" meta:resourcekey="keywordEntriesGridViewResource1">
				  <Columns>
				    <asp:TemplateField meta:resourcekey="TemplateFieldResource1">
				      <ItemTemplate>
				        <asp:Label runat="server" ID="positionLabel"><%# ++entryPosition + (keywordEntriesGridView.PageIndex * keywordEntriesGridView.PageSize) %></asp:Label>)
				      </ItemTemplate>
              <ItemStyle Width="35px" />
				    </asp:TemplateField>
				    <asp:TemplateField HeaderText="Title" meta:resourcekey="TemplateFieldResource2">
				      <ItemTemplate>
				        <asp:HyperLink runat="server" ID="topicHyperlink" NavigateUrl='<%# DocSiteFramePath + "?helpfile=" + HttpUtility.HtmlEncode((string) Eval("VirtualPath")) %>'><%# Eval("Title") %></asp:HyperLink>
				      </ItemTemplate>
              <HeaderStyle HorizontalAlign="Left" />
				    </asp:TemplateField>
					  <asp:BoundField HeaderText="File" DataField="Name" meta:resourcekey="BoundFieldResource1">
              <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
					  <asp:BoundField HeaderText="# Occurrences" DataField="Occurrences" meta:resourcekey="BoundFieldResource2">
              <ItemStyle HorizontalAlign="Center" Width="105px" />
            </asp:BoundField>
					  <asp:BoundField HeaderText="Weight" DataField="Weight" meta:resourcekey="BoundFieldResource3">
              <ItemStyle HorizontalAlign="Center" Width="75px" />
            </asp:BoundField>
				  </Columns>
				  <EmptyDataTemplate>
				    <strong><asp:Localize runat="server" ID="noMatchingKeywordsLocalize" Text="There are no files that contain all of the selected keywords." meta:resourcekey="noMatchingKeywordsLocalize" /></strong>
				  </EmptyDataTemplate>
			  </asp:GridView>
			  <asp:ObjectDataSource runat="server" ID="keywordEntriesDataSource" SelectMethod="KeywordEntriesList" OnSelecting="keywordEntriesDataSource_Selecting"
			                        TypeName="DaveSexton.DocProject.DocSites.DocSiteSearch, DaveSexton.DocProject.DocSites, Version=1.0.1.0, Culture=neutral, PublicKeyToken=af1a4bab65cc4ece" />
		  </asp:Panel>
		  <asp:Panel runat="server" ID="keywordsListPanel" Visible="False" meta:resourcekey="keywordsListPanelResource1">
		    <asp:Label runat="server" ID="letterLabel" SkinID="indexLetterLabel" 
		               Text='<%# string.Format(System.Globalization.CultureInfo.CurrentCulture, GetLocalResourceObject("letterLabel.TextFormat") as string, (KeywordFirstLetter.HasValue) ? char.ToUpper(KeywordFirstLetter.Value).ToString() : (string) GetLocalResourceObject("letterLabel.NoLetter")) %>'><br /></asp:Label>
			  <asp:GridView runat="server" ID="keywordsGridView" AllowPaging="True" AutoGenerateColumns="False" SkinID="IndexGridView"
										  Width="100%" PageSize="20" DataSourceID="keywordsDataSource" meta:resourcekey="keywordsGridViewResource1">
				  <EmptyDataTemplate>
					  <strong><asp:Localize runat="server" ID="noKeywordsLocalize" Text="The index does not contain any keywords." meta:resourcekey="noKeywordsLocalize" /></strong>
				  </EmptyDataTemplate>
				  <Columns>
				    <asp:TemplateField meta:resourcekey="TemplateFieldResource3">
				      <ItemTemplate>
				        <asp:Label runat="server" ID="positionLabel"><%# ++keywordPosition + (keywordsGridView.PageIndex * keywordsGridView.PageSize) %></asp:Label>)
				      </ItemTemplate>
              <ItemStyle Width="35px" />
				    </asp:TemplateField>
				    <asp:BoundField HeaderText="# Files" DataField="EntryCount" meta:resourcekey="BoundFieldResource4">
              <ItemStyle Width="80px" />
              <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
					  <asp:TemplateField HeaderText="Keyword" meta:resourcekey="TemplateFieldResource4">
						  <ItemTemplate>
							  <asp:LinkButton runat="server" ID="keyowrdHyperLink" Text='<%# HttpUtility.HtmlEncode(Eval("Keyword") as string) %>' OnCommand="keyword_Command"
							                  CommandName="Keyword" CommandArgument='<%# Eval("Keyword") %>' meta:resourcekey="keyowrdHyperLinkResource1" />
						  </ItemTemplate>
              <HeaderStyle HorizontalAlign="Left" />
					  </asp:TemplateField>
				  </Columns>
			  </asp:GridView>
			  <asp:ObjectDataSource runat="server" ID="keywordsDataSource" SelectMethod="KeywordsList"
			                        TypeName="DaveSexton.DocProject.DocSites.DocSiteSearch, DaveSexton.DocProject.DocSites, Version=1.0.1.0, Culture=neutral, PublicKeyToken=af1a4bab65cc4ece">
				  <SelectParameters>
					  <asp:Parameter Name="firstLetter" Type="String" />
				  </SelectParameters>
			  </asp:ObjectDataSource>
		  </asp:Panel>
	  </ContentTemplate>
  </asp:UpdatePanel>
  <div id="docsite_page_footer">&nbsp;</div>
</asp:Content>