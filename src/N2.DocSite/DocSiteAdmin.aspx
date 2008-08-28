<%@ Page Language="C#" MasterPageFile="~/DocSite.Master" AutoEventWireup="true" CodeBehind="DocSiteAdmin.aspx.cs" Inherits="N2.DocSite.DocSiteAdmin" Title="N2.DocSite - Administration" 
EnableEventValidation="false" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ Register Src="~/Controls/DocSiteLetterBar.ascx" TagName="DocSiteLetterBar" TagPrefix="DocSite" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceholder" runat="server">
  <h2 id="docsite_page_header"><asp:Localize runat="server" ID="administrationHeaderLocalize" Text="Administration" meta:resourcekey="administrationHeaderLocalize" /></h2>
  <asp:ObjectDataSource runat="server" ID="statisticsDataSource" TypeName="N2.DocSite.DocSiteAdmin" SelectMethod="GetStatisticsForBinding" />
	<asp:ObjectDataSource runat="server" ID="clientSettingsDataSource" TypeName="N2.DocSite.DocSiteAdmin" SelectMethod="GetSettingsForBinding" UpdateMethod="UpdateSettings"
	                      DataObjectTypeName="DaveSexton.DocProject.DocSites.Configuration.DocSiteSettings, DaveSexton.DocProject.DocSites, Version=1.0.1.0, Culture=neutral, PublicKeyToken=af1a4bab65cc4ece" />
	<asp:ObjectDataSource runat="server" ID="searchSettingsDataSource" TypeName="N2.DocSite.DocSiteAdmin" SelectMethod="GetSettingsForBinding" UpdateMethod="UpdateSettings"
	                      DataObjectTypeName="DaveSexton.DocProject.DocSites.Configuration.DocSiteSettings, DaveSexton.DocProject.DocSites, Version=1.0.1.0, Culture=neutral, PublicKeyToken=af1a4bab65cc4ece" />
	<asp:ObjectDataSource runat="server" ID="keywordSettingsDataSource" TypeName="N2.DocSite.DocSiteAdmin" SelectMethod="GetSettingsForBinding" UpdateMethod="UpdateSettings"
	                      DataObjectTypeName="DaveSexton.DocProject.DocSites.Configuration.DocSiteSettings, DaveSexton.DocProject.DocSites, Version=1.0.1.0, Culture=neutral, PublicKeyToken=af1a4bab65cc4ece" />
	<asp:ObjectDataSource runat="server" ID="weightFactorsDataSource" TypeName="N2.DocSite.DocSiteAdmin" SelectMethod="GetSettingsForBinding" UpdateMethod="UpdateSettings"
	                      DataObjectTypeName="DaveSexton.DocProject.DocSites.Configuration.DocSiteSettings, DaveSexton.DocProject.DocSites, Version=1.0.1.0, Culture=neutral, PublicKeyToken=af1a4bab65cc4ece" />
	<asp:ObjectDataSource runat="server" ID="additionalWeightsDataSource" TypeName="N2.DocSite.DocSiteAdmin" SelectMethod="GetSettingsForBinding" UpdateMethod="UpdateSettings"
	                      DataObjectTypeName="DaveSexton.DocProject.DocSites.Configuration.DocSiteSettings, DaveSexton.DocProject.DocSites, Version=1.0.1.0, Culture=neutral, PublicKeyToken=af1a4bab65cc4ece" />
	<div id="docsite_admin" class="content_padding">
    <fieldset>
			<legend><asp:Localize runat="server" ID="generalGroupLocalize" Text="General" meta:resourcekey="generalGroupLocalize" /></legend>
			<asp:UpdatePanel runat="server" ID="clientSettingsUpdatePanel">
			  <ContentTemplate>
          <asp:DetailsView runat="server" SkinID="adminDetailsView" ID="clientSettingsDetailsView" AutoGenerateRows="False" DataSourceID="clientSettingsDataSource" meta:resourcekey="clientSettingsDetailsViewResource1">
            <HeaderTemplate>
              <div class="docsite_admin_section">
                <div id="update_progress_floatright">
                  <asp:UpdateProgress runat="server" ID="updateProgress" AssociatedUpdatePanelID="clientSettingsUpdatePanel">
                    <ProgressTemplate>
                      <asp:Localize runat="server" ID="generalPleaseWaitLocalize" Text='<%$ Resources:General,PleaseWait %>' />
                    </ProgressTemplate>
                  </asp:UpdateProgress>
                </div>
                <asp:Localize runat="server" ID="clientSettingsLocalize" Mode="encode" Text="Client Settings" meta:resourcekey="clientSettingsLocalize"></asp:Localize>
                <span class="small content_padding_horizontal">
                  <asp:LinkButton runat="server" ID="editLinkButton" CssClass="textlink"
                                  CommandName="Edit" Visible='<%# clientSettingsDetailsView.CurrentMode != DetailsViewMode.Edit %>' meta:resourcekey="editLinkButtonResource1">Edit</asp:LinkButton>
                  <asp:LinkButton runat="server" ID="updateLinkButton" CssClass="textlink" 
                                  CommandName="Update" Visible='<%# clientSettingsDetailsView.CurrentMode == DetailsViewMode.Edit %>' meta:resourcekey="updateLinkButtonResource1">Update</asp:LinkButton>
                  <asp:LinkButton runat="server" ID="cancelLinkButton" CssClass="textlink" 
                                  CommandName="Cancel" Visible='<%# clientSettingsDetailsView.CurrentMode == DetailsViewMode.Edit %>' meta:resourcekey="cancelLinkButtonResource1">Cancel</asp:LinkButton>
                </span>
              </div>
            </HeaderTemplate>
            <Fields>
              <asp:BoundField DataField="LetterBar" HeaderText="Letter bar:" meta:resourcekey="BoundFieldResource1" >
                <ControlStyle Width="250px" />
              </asp:BoundField>
              <asp:CheckBoxField DataField="SidebarHandlePersisted" HeaderText="Sidebar size persisted:" meta:resourcekey="CheckBoxFieldResource1" />
            </Fields>
          </asp:DetailsView>
			  </ContentTemplate>
      </asp:UpdatePanel>
    </fieldset>
		<fieldset>
			<legend><asp:Localize runat="server" ID="searchGroupLocalize" Text="Search" meta:resourcekey="searchGroupLocalize" /></legend>
			<asp:UpdatePanel runat="server" ID="indexUpdatePanel">
			  <ContentTemplate>         
	        <div class="content_padding">
		        <asp:LinkButton runat="server" ID="createIndexLinkButton" OnClick="createIndexLinkButton_Click" ToolTip="Generates the keyword index used for the DocSite search and browse features."
              OnClientClick="return confirm('Are you sure you want to generate the entire full-text index now?');" CausesValidation="False" meta:resourcekey="createIndexLinkButtonResource1">Create Index</asp:LinkButton>
          </div>
	        <asp:UpdateProgress runat="server" ID="createIndexUpdateProgress" AssociatedUpdatePanelID="indexUpdatePanel">
            <ProgressTemplate>
              <div class="update_progress">
      		      <asp:Image runat="server" ID="createIndexWaitImage" SkinID="DocSitePleaseWait" ImageAlign="Left" ToolTip='<%$ Resources:General,PleaseWait %>' meta:resourcekey="createIndexWaitImageResource1" />
                <h5><asp:Localize runat="server" ID="createIndexPleaseWaitLine1Localize" Text='<%$ Resources:General,PleaseWaitGenerateIndexLine1 %>' /></h5>
                <span class="update_progress_message"><asp:Localize runat="server" ID="createIndexPleaseWaitLine2Localize" Text='<%$ Resources:General,PleaseWaitGenerateIndexLine2 %>' /></span>
              </div>
            </ProgressTemplate>
          </asp:UpdateProgress>
			  </ContentTemplate>
			</asp:UpdatePanel>
			<asp:UpdatePanel runat="server" ID="searchStatsUpdatePanel" ChildrenAsTriggers="False" UpdateMode="Conditional">
			  <ContentTemplate>
			    <asp:DetailsView runat="server" SkinID="adminDetailsView" ID="searchStatsDetailsView" AutoGenerateRows="False" DataSourceID="statisticsDataSource" meta:resourcekey="searchStatsDetailsViewResource1">
            <HeaderTemplate>
              <div class="docsite_admin_section">
                <div id="update_progress_floatright">
                  <asp:UpdateProgress runat="server" ID="updateProgress" AssociatedUpdatePanelID="searchStatsUpdatePanel">
                    <ProgressTemplate>
                      <asp:Localize runat="server" ID="searchIndexPleaseWaitLocalize" Text='<%$ Resources:General,PleaseWait %>' />
                    </ProgressTemplate>
                  </asp:UpdateProgress>
                </div>
                <asp:Localize runat="server" ID="indexStatisticsLocalize" Text="Index Statistics" meta:resourcekey="indexStatisticsLocalize" />
              </div>
            </HeaderTemplate>
            <Fields>
              <asp:BoundField HeaderText="Provider name:" DataField="ProviderName" ReadOnly="True" meta:resourcekey="BoundFieldResource2" />
              <asp:TemplateField HeaderText="Last creation date:" meta:resourcekey="TemplateFieldResource1">
                <ItemTemplate>
                  <asp:Label runat="server" ID="dateLabel" meta:resourcekey="dateLabelResource1"
                             Text='<%# ((bool) Eval("IndexCreated")) ? ((DateTime) Eval("LastCreationDate")).ToString("f") : GetLocalResourceObject("dateLabelResource1.NoDate") as string%>'/>
                </ItemTemplate>
              </asp:TemplateField>
              <asp:BoundField HeaderText="# keywords:" DataField="KeywordCount" ReadOnly="True" meta:resourcekey="BoundFieldResource3" />
              <asp:BoundField HeaderText="# documents:" DataField="DocumentCount" ReadOnly="True" meta:resourcekey="BoundFieldResource4" />
            </Fields>
          </asp:DetailsView>
			  </ContentTemplate>
			</asp:UpdatePanel>
			<asp:UpdatePanel runat="server" ID="searchUpdatePanel">
				<ContentTemplate>
          <asp:DetailsView runat="server" SkinID="adminDetailsView" ID="searchSettingsDetailsView" AutoGenerateRows="False" DataSourceID="searchSettingsDataSource"
                           OnItemUpdating="searchSettingsDetailsView_ItemUpdating" meta:resourcekey="searchSettingsDetailsViewResource1">
            <HeaderTemplate>
              <div class="docsite_admin_section">
                <div id="update_progress_floatright">
                  <asp:UpdateProgress runat="server" ID="updateProgress" AssociatedUpdatePanelID="searchUpdatePanel">
                    <ProgressTemplate>
                      <asp:Localize runat="server" ID="searchSettingsPleaseWaitLocalize" Text='<%$ Resources:General,PleaseWait %>' />
                    </ProgressTemplate>
                  </asp:UpdateProgress>
                </div>
                <asp:Localize runat="server" ID="searchSettingsLocalize" Text="Settings" meta:resourcekey="searchSettingsLocalize" />
                <span class="small content_padding_horizontal">
                  <asp:LinkButton runat="server" ID="editLinkButton" CssClass="textlink"
                                  CommandName="Edit" Visible='<%# searchSettingsDetailsView.CurrentMode != DetailsViewMode.Edit %>' meta:resourcekey="editLinkButtonResource2">Edit</asp:LinkButton>
                  <asp:LinkButton runat="server" ID="updateLinkButton" CssClass="textlink" 
                                  CommandName="Update" Visible='<%# searchSettingsDetailsView.CurrentMode == DetailsViewMode.Edit %>' meta:resourcekey="updateLinkButtonResource2">Update</asp:LinkButton>
                  <asp:LinkButton runat="server" ID="cancelLinkButton" CssClass="textlink" 
                                  CommandName="Cancel" Visible='<%# searchSettingsDetailsView.CurrentMode == DetailsViewMode.Edit %>' meta:resourcekey="cancelLinkButtonResource2">Cancel</asp:LinkButton>
                </span>
              </div>
            </HeaderTemplate>
            <Fields>
              <asp:TemplateField HeaderText="Root search path:" meta:resourcekey="TemplateFieldResource2">
                <ItemTemplate>
                  <span>
                    <%# (VirtualPathUtility.IsAppRelative((string) Eval("SearchPath"))) ? VirtualPathUtility.ToAbsolute((string) Eval("SearchPath")) : (string) Eval("SearchPath") %>
                  </span>
                </ItemTemplate>
                <EditItemTemplate>
                  <asp:TextBox runat="server" ID="searchPathTextBox" Text='<%# Eval("SearchPath") %>' Width="250px" meta:resourcekey="searchPathTextBoxResource1"></asp:TextBox>
                </EditItemTemplate>
              </asp:TemplateField>
              <asp:CheckBoxField DataField="SearchEnabled" HeaderText="Public search enabled:" meta:resourcekey="CheckBoxFieldResource2" />
              <asp:CheckBoxField DataField="BrowseIndexEnabled" HeaderText="Public browse enabled:" meta:resourcekey="CheckBoxFieldResource3" />
            </Fields>
          </asp:DetailsView>
        </ContentTemplate>
			</asp:UpdatePanel>
			<asp:UpdatePanel runat="server" ID="searchKeywordsUpdatePanel">
			  <ContentTemplate>
          <asp:DetailsView runat="server" SkinID="adminDetailsView" ID="searchKeywordsDetailsView" AutoGenerateRows="False" DataSourceID="keywordSettingsDataSource" meta:resourcekey="searchKeywordsDetailsViewResource1">
            <HeaderTemplate>
              <div class="docsite_admin_section">
                <div id="update_progress_floatright">
                  <asp:UpdateProgress runat="server" ID="updateProgress" AssociatedUpdatePanelID="searchKeywordsUpdatePanel">
                    <ProgressTemplate>
                      <asp:Localize runat="server" ID="searchKeywordsPleaseWaitLocalize" Text='<%$ Resources:General,PleaseWait %>' />
                    </ProgressTemplate>
                  </asp:UpdateProgress>
                </div>
                <asp:Localize runat="server" ID="searchKeywordSettingsLocalize" Text="Keyword Settings" meta:resourcekey="searchKeywordSettingsLocalize" />
                <span class="small content_padding_horizontal">
                  <asp:LinkButton runat="server" ID="editLinkButton" CssClass="textlink" 
                                  CommandName="Edit" Visible='<%# searchKeywordsDetailsView.CurrentMode != DetailsViewMode.Edit %>' meta:resourcekey="editLinkButtonResource3">Edit</asp:LinkButton>
                  <asp:LinkButton runat="server" ID="updateLinkButton" CssClass="textlink" 
                                  CommandName="Update" Visible='<%# searchKeywordsDetailsView.CurrentMode == DetailsViewMode.Edit %>' meta:resourcekey="updateLinkButtonResource3">Update</asp:LinkButton>
                  <asp:LinkButton runat="server" ID="cancelLinkButton" CssClass="textlink" 
                                  CommandName="Cancel" Visible='<%# searchKeywordsDetailsView.CurrentMode == DetailsViewMode.Edit %>' meta:resourcekey="cancelLinkButtonResource3">Cancel</asp:LinkButton>
                </span>
              </div>
            </HeaderTemplate>
            <Fields>
              <asp:BoundField DataField="SearchMinimumKeywordLength" HeaderText="Minimum keyword length:" meta:resourcekey="BoundFieldResource5" />
              <asp:BoundField DataField="SearchExcludedKeywords" HeaderText="Excluded keywords:" meta:resourcekey="BoundFieldResource6" >
                <ControlStyle Width="250px" />
              </asp:BoundField>
              <asp:BoundField DataField="SearchHotTitleKeywords" HeaderText="Hot title keywords:" meta:resourcekey="BoundFieldResource7" >
                <ControlStyle Width="250px" />
              </asp:BoundField>
            </Fields>
          </asp:DetailsView>
        </ContentTemplate>
			</asp:UpdatePanel>
			<asp:UpdatePanel runat="server" ID="searchWeightFactorsUpdatePanel">
			  <ContentTemplate>
          <asp:DetailsView runat="server" SkinID="adminDetailsView" ID="searchWeightFactorsDetailsView" AutoGenerateRows="False" DataSourceID="weightFactorsDataSource"
                           OnItemUpdating="searchWeightFactorsDetailsView_ItemUpdating" meta:resourcekey="searchWeightFactorsDetailsViewResource1">
            <HeaderTemplate>
              <div class="docsite_admin_section">
                <div id="update_progress_floatright">
                  <asp:UpdateProgress runat="server" ID="updateProgress" AssociatedUpdatePanelID="searchWeightFactorsUpdatePanel">
                    <ProgressTemplate>
                      <asp:Localize runat="server" ID="searchWeightFactorsPleaseWaitLocalize" Text='<%$ Resources:General,PleaseWait %>' />
                    </ProgressTemplate>
                  </asp:UpdateProgress>
                </div>
                <asp:Localize runat="server" ID="searchWeightFactorsLocalize" Text="Weight Factors" meta:resourcekey="searchWeightFactorsLocalize" />
                <span class="small content_padding_horizontal">
                  <asp:LinkButton runat="server" ID="editLinkButton" CssClass="textlink" 
                                  CommandName="Edit" Visible='<%# searchWeightFactorsDetailsView.CurrentMode != DetailsViewMode.Edit %>' meta:resourcekey="editLinkButtonResource4">Edit</asp:LinkButton>
                  <asp:LinkButton runat="server" ID="updateLinkButton" CssClass="textlink" 
                                  CommandName="Update" Visible='<%# searchWeightFactorsDetailsView.CurrentMode == DetailsViewMode.Edit %>' meta:resourcekey="updateLinkButtonResource4">Update</asp:LinkButton>
                  <asp:LinkButton runat="server" ID="cancelLinkButton" CssClass="textlink" 
                                  CommandName="Cancel" Visible='<%# searchWeightFactorsDetailsView.CurrentMode == DetailsViewMode.Edit %>' meta:resourcekey="cancelLinkButtonResource4">Cancel</asp:LinkButton>
                </span>
              </div>
            </HeaderTemplate>
            <Fields>
              <asp:TemplateField HeaderText="Early position keyword:" meta:resourcekey="TemplateFieldResource3">
                <ItemTemplate>
                  <asp:Label runat="server" ID="earlyKeywordWeighFormulaLabel" meta:resourcekey="earlyKeywordWeighFormulaLabel"
                             Text='<%# string.Format(System.Globalization.CultureInfo.CurrentCulture, GetLocalResourceObject("earlyKeywordWeighFormulaLabel.Formula") as string, " -<strong>" + Eval("SearchEarlyKeywordWeightFactor") + "</strong> + <strong>" + Eval("SearchEarlyKeywordWeightFactor") + "</strong>") %>' />
                  </strong>
                </ItemTemplate>
                <EditItemTemplate>
                  <asp:TextBox runat="server" ID="earlyKeywordWeightFactorTextBox" Text='<%# Eval("SearchEarlyKeywordWeightFactor") %>' meta:resourcekey="earlyKeywordWeightFactorTextBoxResource1"></asp:TextBox>
                </EditItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField HeaderText="Title keyword:" meta:resourcekey="TemplateFieldResource4">
                <ItemTemplate>
                  <asp:Label runat="server" ID="titleWeightFormulaLabel" meta:resourcekey="titleWeightFormulaLabel"
                             Text='<%# string.Format(System.Globalization.CultureInfo.CurrentCulture, GetLocalResourceObject("titleWeightFormulaLabel.Formula") as string, "<strong>" + Eval("SearchTitleKeywordWeightFactor") + "</strong>") %>' />
                </ItemTemplate>
                <EditItemTemplate>
                  <asp:TextBox runat="server" ID="titleKeywordWeightFactorTextBox" Text='<%# Eval("SearchTitleKeywordWeightFactor") %>' meta:resourcekey="titleKeywordWeightFactorTextBoxResource1"></asp:TextBox>
                </EditItemTemplate>
              </asp:TemplateField>
            </Fields>
          </asp:DetailsView>
        </ContentTemplate>
			</asp:UpdatePanel>
			<asp:UpdatePanel runat="server" ID="searchTitleWeightsUpdatePanel">
			  <ContentTemplate>
          <asp:DetailsView runat="server" SkinID="adminDetailsView" ID="searchTitleWeightsDetailsView" AutoGenerateRows="False" DataSourceID="additionalWeightsDataSource" meta:resourcekey="searchTitleWeightsDetailsViewResource1">
            <HeaderTemplate>
              <div class="docsite_admin_section">
                <div id="update_progress_floatright">
                  <asp:UpdateProgress runat="server" ID="updateProgress" AssociatedUpdatePanelID="searchTitleWeightsUpdatePanel">
                    <ProgressTemplate>
                      <asp:Localize runat="server" ID="searchTitleRankFactoresPleaseWaitLocalize" Text='<%$ Resources:General,PleaseWait %>' />
                    </ProgressTemplate>
                  </asp:UpdateProgress>
                </div>
                <asp:Localize runat="server" ID="searchTitleRankFactorsLocalize" Text="Title Rank Factors" meta:resourcekey="searchTitleRankFactorsLocalize" />
                <span class="small content_padding_horizontal">
                  <asp:LinkButton runat="server" ID="editLinkButton" CssClass="textlink" 
                                  CommandName="Edit" Visible='<%# searchTitleWeightsDetailsView.CurrentMode != DetailsViewMode.Edit %>' meta:resourcekey="editLinkButtonResource5">Edit</asp:LinkButton>
                  <asp:LinkButton runat="server" ID="updateLinkButton" CssClass="textlink" 
                                  CommandName="Update" Visible='<%# searchTitleWeightsDetailsView.CurrentMode == DetailsViewMode.Edit %>' meta:resourcekey="updateLinkButtonResource5">Update</asp:LinkButton>
                  <asp:LinkButton runat="server" ID="cancelLinkButton" CssClass="textlink" 
                                  CommandName="Cancel" Visible='<%# searchTitleWeightsDetailsView.CurrentMode == DetailsViewMode.Edit %>' meta:resourcekey="cancelLinkButtonResource5">Cancel</asp:LinkButton>
                </span>
              </div>
            </HeaderTemplate>
            <Fields>
              <asp:BoundField DataField="SearchExactTitleHotKeywordMatchRank" DataFormatString="+{0}" HeaderText="Exact title hot keyword match:" meta:resourcekey="BoundFieldResource8" />
              <asp:BoundField DataField="SearchPartialTitleHotKeywordMatchRank" DataFormatString="+{0}" HeaderText="Partial title hot keyword match:" meta:resourcekey="BoundFieldResource9" />
              <asp:BoundField DataField="SearchExactTitleKeywordMatchRank" DataFormatString="+{0}" HeaderText="Exact title keyword match:" meta:resourcekey="BoundFieldResource10" />
              <asp:BoundField DataField="SearchPartialTitleKeywordMatchRank" DataFormatString="+{0}" HeaderText="Partial title keyword match:" meta:resourcekey="BoundFieldResource11" />
            </Fields>
          </asp:DetailsView>
				</ContentTemplate>
			</asp:UpdatePanel>
		</fieldset>
	</div>
	<div id="docsite_page_footer">&nbsp;</div>
</asp:Content>
