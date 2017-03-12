<%@ Page Title="Ricerca Rubrica" Language="C#" MasterPageFile="~/Master/Mail.Master" Theme="Delta" AutoEventWireup="true" CodeBehind="EntiResearch.aspx.cs" Inherits="GestionePEC.pages.Rubrica.EntiResearch" %>
<%@ Register Src="~/Controls/EntiViewer.ascx" TagName="UCEntiViewer" TagPrefix="uc2" %>
<%@ Register Src="~/Controls/GeneralEntiyTree.ascx" TagName="UCGeneraltree"
    TagPrefix="tree" %>
<%@ Register Src="~/Controls/EntitaSearch.ascx" TagName="UCEntiSearcher" TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="WestContentPlaceHolder" runat="server">
    <tree:UCGeneraltree ID="ucGeneralTree" runat="server" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">   
    <div id="Div2" class="content-panel-borderless">
        <div class="header-panel-blue">
            <div class="header-title">
                <div class="header-text-left">
                    <label>
                        RICERCHE</label>
                </div>
            </div>
        </div>
        <div id="container" class="body-panel" style="padding: 5px 5px 5px 5px;">
            <div class="content-panel">           
                <uc:UCEntiSearcher runat="server" ID="ucEntiSearcher" />
                <asp:Button ID="btnShowEntita" runat="server" CausesValidation="false" OnClick="btnShowEntita_OnClick" />
            </div>
            <div>
                <div id="divStruttura" style="vertical-align: top; width: 30%; display: none; max-width: 30%;
                    max-height: 500px; overflow: auto;">
                    <div class="body-panel" style="padding: 5px 5px 5px 5px;">
                        <div class="control-header-blue">
                            <div class="control-header-title">
                                <div class="control-header-text-left">
                                    <label class="header-title">
                                        Struttura Ente</label>
                                </div>
                            </div>
                        </div>
                        <div class="control-body-gray" id="divTree">
                        </div>
                        <asp:HiddenField ID="hfSelectedTreeNode" runat="server" />
                        <asp:Button ID="btnTreeNode" runat="server" CausesValidation="false" OnClick="btnTreeNode_Click" />
                    </div>
                </div>
                <div style="display: inline-block; border-collapse: collapse; width: 69%; overflow: auto;">
                    <asp:UpdatePanel runat="server" ID="pnlDownPage" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnShowEntita" />
                            <asp:AsyncPostBackTrigger ControlID="btnTreeNode" />                            
                            <asp:AsyncPostBackTrigger ControlID="UCEntiViewer" />
                        </Triggers>
                        <ContentTemplate>
                            <asp:Panel runat="server" ID="pnlDettagli">                             
                                <uc2:UCEntiViewer ID="UCEntiViewer" runat="server" />
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div id="divMappings" class="content-panel" style="display: none;">
                        <div class="control-header-blue" id="pnlHeader">
                            <div class="control-header-title">
                                <div class="control-header-text-left">
                                    <h2 style="text-align: center;">
                                        Mappature delle Applicazioni
                                    </h2>
                                </div>
                            </div>
                        </div>
                        <div class="body-panel-gray" id="mappings">
                            
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
//  <![CDATA[
        var config;
        Ext.onReady(function(){
            config = {
                btnShowEntita: Ext.get('<%= btnShowEntita.ClientID %>'),
                roles: Ext.decode('<%= this.Roles %>'),
                tree: {
                    divStruttura: Ext.get('divStruttura'),
                    divTree: 'divTree',
                    btnShowTreeNode: Ext.get('<%= btnTreeNode.ClientID %>'),
                    selectedNode: Ext.get('<%= hfSelectedTreeNode.ClientID  %>'),
                    provider: '<%= Page.ResolveClientUrl("~/Ashx/TreeEnt.ashx") %>'
                },
                mappings: {
                    divMappings: Ext.get('divMappings'),
                    mappingContainer: Ext.get('mappings'),
                    provider: '<%= Page.ResolveClientUrl("~/api/EntiMappingController") %>'
                }
            };
        });
//  ]]>
    </script>

    <script type="text/javascript" src='<%= Page.ResolveClientUrl("~/scripts/EntiResearch.js")%>' />

</asp:Content>
