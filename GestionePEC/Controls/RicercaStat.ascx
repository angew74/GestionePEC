<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RicercaStat.ascx.cs" Inherits="GestionePEC.Controls.RicercaStat" %>
<%@ Register TagPrefix="uc1" TagName="DataTime" Src="~/controls/DateTime.ascx" %>
<%@ Register TagPrefix="uc" TagName="Paging" Src="~/controls/Paging.ascx" %>
<asp:Panel ID="pnlContent" runat="server" DefaultButton="btnRicerca" CssClass="body-panel">
    <asp:HiddenField runat="server" ID="hdTabIndex" />
    <asp:Panel runat="server" ID="pnlTabContainer">
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlStatistica" CssClass="control-tab-gray">
        <table class="NewTable">
            <tr>
                <td>
                    <label class="LabelRedMiddle">
                        Casella:</label>
                </td>
                <td>
                    <asp:UpdatePanel ID="UpdAccounts" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DropDownList runat="server" ID="ddlManagedAccounts" ToolTip="Accounts gestiti"
                                AutoPostBack="true" OnDataBinding="ddlManagedAccounts_DataBinding" OnDataBound="ddlManagedAccounts_DataBound"
                                OnSelectedIndexChanged="ddlManagedAccounts_SelectedIndexChanged" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td>
                    <label class="LabelRedMiddle">
                        Data Inizio:</label>
                </td>
                <td>
                    <uc1:DataTime ID="dtInizio" EnabledValidator="true" runat="server" ValidationGroup="vgTabStatistica" />
                </td>
                <td>
                    <label class="LabelRedMiddle">
                        Data Fine:</label>
                </td>
                <td>
                    <uc1:DataTime ID="dtFine" EnabledValidator="true" runat="server" ValidationGroup="vgTabStatistica" />
                </td>
            </tr>
            <tr>
                <td>
                    <label class="LabelBlackMiddle">
                        Utente:</label>
                </td>
                <td>
                    <asp:UpdatePanel ID="UpdUtente" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DropDownList ID="ddlUtente" runat="server" SkinID="ddlMiddle">
                                <asp:ListItem Text="Selezionare un valore" Value=""></asp:ListItem>
                            </asp:DropDownList>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <div class="buttons-panel" style="margin-top: 4px">
        <asp:Button ID="btnRicerca" runat="server" Text="Cerca" ToolTip="Effettua la ricerca"
            OnClientClick="return RIAValidatePage(this, event)" OnClick="OnClickRicerca" />
    </div>
    <asp:Panel runat="server" CssClass="body-panel" ID="PanelGrid">
        <asp:GridView ID="gridStat" runat="server" AllowPaging="True" AutoGenerateColumns="False"
            PageSize="20">
            <Columns>
                <asp:BoundField ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center" DataField="User"
                    HeaderText="Utente" HeaderStyle-Wrap="false" />
                <asp:TemplateField HeaderText="Casella di Posta" ItemStyle-Width="130px" ItemStyle-Wrap="true">
                    <ItemStyle Wrap="true" Width="150px" />
                    <ItemTemplate>
                        <div style="width: 150px; overflow: hidden; white-space: nowrap;">
                            <asp:Label ID="lblAccount" runat="server" Text='<%# Eval( "Account")%>' />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Operazioni" ItemStyle-Width="160px" ItemStyle-Wrap="true">
                    <ItemStyle Wrap="true" Width="160px" />
                    <ItemTemplate>
                        <div style="width: 160px; overflow: hidden; white-space: nowrap;">
                            <asp:Label ID="lblOperazioni" runat="server" Text='<%# Eval( "Operazioni")%>' />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerTemplate>
                <uc:Paging ID="ucPaging" runat="server" OnPagerIndexChanged="OnPagerIndexChanged" />
            </PagerTemplate>
            <EmptyDataTemplate>
                <label style="color: Maroon">
                    Nessun riscontro alla ricerca effettuata
                </label>
            </EmptyDataTemplate>
        </asp:GridView>
        <asp:Panel runat="server"  BorderColor="#99BBE8" Visible="false" ID="PanelTotale">
            <asp:Label runat="server" ForeColor="Maroon" ID="totPeriodo" />
         </asp:Panel>
        <div class="buttons-panel" style="margin-top: 4px">
            <asp:Button ID="btnStampaStatistica" runat="server" Visible="false" Text="Stampa Statistica"
                CausesValidation="false" OnClick="btnStampa_Statistica" />
        </div>
    </asp:Panel>
</asp:Panel>
<script type="text/javascript">
    Ext.require([
'Ext.tab.Panel'
    ]);

    Ext.onReady(function () {
        Ext.tip.QuickTipManager.init();
        var tabIndex = document.getElementById('<%=hdTabIndex.ClientID %>').id;
        var tabValue = document.getElementById('<%=hdTabIndex.ClientID %>').value;
        var hdTabIndexID =document.getElementById('<%=hdTabIndex.ClientID %>');
        createPanel();
        function createPanel() {
            if (Ext.getCmp('PanelRichiesta')) {
                Ext.getCmp('PanelRichiesta').destroy();
            }
            superCreatePanel();
            if (document.getElementById('<%=pnlStatistica.ClientID %>') != null)
            { document.getElementById('<%=pnlStatistica.ClientID %>').style.display = 'block'; }
        }

        function superCreatePanel() {
            var PanelRicerca = Ext.create('Ext.tab.Panel', ({
                renderTo: document.getElementById('<%= pnlTabContainer.ClientID %>'),
                activeTab: 0,
                plain: true,
                forceFit: true,
                maxWidth:1700,
                id: 'PanelRichiesta',
                frame: false,
                defaults: { autoHeight: true, autoWidth: true, autoScroll: true },
                items: [{
                    title: 'Casella Mail',
                    contentEl: document.getElementById('<%=pnlStatistica.ClientID %>'),
                    listeners:
                        {
                            activate: RIAhandleActivate
                        }
                }], listeners: { 'tabchange': RIAactiveTabChanged }
            }))
        }

        function RIAhandleActivate(tab) {
            var pnl = document.getElementById(tab.contentEl);
            if (pnl != null) {
                pnl.style.display = 'block';
            }
        }

        function RIAactiveTabChanged(tab, tabPanel) {
            /*
             * azzera il riferimento al controllo non valido sul quale settare il focus
             * in quanto l'utente si è spostato di tab.
             * evita la comparsa di un errore js per control.focus() 
             * su di un controllo non accessibile.
            */
            Page_InvalidControlToBeFocused = null;
            var index = 0;
            switch (tabPanel.title) {
                case 'Casella Mail':
                    RIAValidationGroup = 'vgTabStatistica';
                    index = 0;
                    break;
            }
            var tabIndex =document.getElementById('<%=hdTabIndex.ClientID %>').value;
            if (tabIndex != null)
                tabIndex.value = index;
        }

        var RIAValidationGroup = 'vgTabStatistica';

        function RIAValidatePage(sender, event) {
            var fResult = false;
            if (RIAValidationGroup != '') {
                Page_ClientValidate(RIAValidationGroup);
                fResult = Page_IsValid;
                if (!fResult)
                    Page_BlockSubmit = false;
            }
            return fResult;
        }


    });
</script>
