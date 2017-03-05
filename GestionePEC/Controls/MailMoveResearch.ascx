<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailMoveResearch.ascx.cs" Inherits="GestionePEC.Controls.MailMoveResearch" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register TagPrefix="uc1" TagName="DataTime" Src="~/Controls/DateTime.ascx" %>
<%@ Register TagPrefix="uc" TagName="Paging" Src="~/Controls/Paging.ascx" %>
<asp:Panel ID="pnlContent" runat="server" DefaultButton="btnRicerca" CssClass="body-panel">
    <asp:HiddenField runat="server" ID="hdTabIndex" />
    <asp:Panel runat="server" ID="pnlTabContainer">
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlCasella" Style="height: 130px; font-size: smaller" CssClass="control-tab-gray">
        <table class="NewTableSmall">
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
                <td>
                    <label class="LabelRedMiddle">
                        OutBox/Inbox:
                    </label>
                </td>
                <td>
                    <asp:RadioButtonList RepeatDirection="Horizontal" AutoPostBack="true" runat="server"
                        OnSelectedIndexChanged="rblIOBox_Changed" ID="rblIOBox">
                        <asp:ListItem Text="InBox" Value="I"></asp:ListItem>
                        <asp:ListItem Text="OutBox" Value="O"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td>
                    <label class="LabelRedMiddle">
                        Tipo Cartella:
                    </label>
                </td>
                <td>
                    <asp:RadioButtonList RepeatDirection="Horizontal" AutoPostBack="true" runat="server"
                        OnSelectedIndexChanged="rblTipoFolder_Changed" ID="rblTipoFolder">
                        <asp:ListItem Text="Ordinaria" Value="O" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Archivio" Value="A"></asp:ListItem>
                        <asp:ListItem Text="Cestino" Value="C"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td>
                    <label class="LabelRedMiddle">
                        Cartella:</label>
                </td>
                <td>
                    <asp:UpdatePanel ID="UpdCartella" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DropDownList ID="ddlCartella" runat="server" SkinID="ddlMiddle">
                                <asp:ListItem Text="Selezionare un valore" Value="" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvddlCartella" runat="server" ErrorMessage="Dato obbligatorio"
                                ControlToValidate="ddlCartella" Display="None" SetFocusOnError="True" ValidationGroup="vgTabCasella">
                            </asp:RequiredFieldValidator>
                            <cc1:ValidatorCalloutExtender CssClass="CustomValidator" ID="vceCartella" runat="server"
                                TargetControlID="rfvddlCartella" Enabled="True">
                            </cc1:ValidatorCalloutExtender>
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
                    <uc1:DataTime ID="dtInizio" EnabledValidator="true" runat="server" ValidationGroup="vgTabCasella" />
                </td>
                <td>
                    <label class="LabelRedMiddle">
                        Data Fine:</label>
                </td>
                <td>
                    <uc1:DataTime ID="dtFine" EnabledValidator="true" runat="server" ValidationGroup="vgTabCasella" />
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
                <td>
                      <label class="LabelBlackMiddle">
                        Solo Utenti Ufficio:</label>
                </td>
                <td>
                    <asp:CheckBox ID="chkUfficio" runat="server" />
                </td>
                 </tr>
            </table>
         </asp:Panel>
    <div style="color: Maroon; border: 1px solid #6593cf; margin-top: 5px; padding: 5px">
        <div>
            Attenzione!!!! I dati minimi per la ricerca sono contrassegnati in grassetto. L'utente
            nella ricerca sulla inbox è l'<u>ultimo</u> che ha preso in carico l'email. Il titolo,
            il sottotitolo e lo status sono validi solo per l'outbox. I marcatori o pallini
            sono validi solo per l'inbox.
        </div>
    </div>
    <div class="buttons-panel" style="margin-top: 4px">
        <asp:Button ID="btnRicerca" runat="server" Text="Cerca" ToolTip="Effettua la ricerca"
            OnClientClick="return RIAValidatePage(this, event)" OnClick="OnClickRicerca" />
    </div>
     <asp:UpdatePanel ID="pnlMainUpdate" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnStampaDistinta" />
        </Triggers>
        <ContentTemplate>
            <asp:Panel runat="server" CssClass="body-panel" ID="PanelGrid">
                <asp:GridView ID="gridBox" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    OnRowDataBound="Grid_RowDataBound" Font-Size="Smaller" PageSize="5" DataKeyNames="UniqueId" OnRowCommand="Grid_RowCommand" OnRowEditing="Grid_RowEditing"
                    OnDataBound="Grid_DataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="ID Mail" Visible="false" HeaderStyle-Wrap="false"
                            ItemStyle-Width="15px" ItemStyle-CssClass="hide" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="lblUnId" runat="server" Text='<%#  Eval("UniqueId").ToString() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Center" DataField="Utente"
                            HeaderText="Utente" HeaderStyle-Wrap="false" />
                        <asp:TemplateField HeaderText="Mittente" ItemStyle-Width="250px" ItemStyle-Wrap="true">
                            <ItemStyle Wrap="true" Width="250px" />
                            <ItemTemplate>
                                <div style="width: 250px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;">
                                    <asp:Label ID="lblFrom" runat="server" Width="240px" Text='<%# Eval( "From")%>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Destinatario" ItemStyle-Width="250px" ItemStyle-Wrap="true">
                            <ItemStyle Wrap="true" Width="250px" />
                            <ItemTemplate>
                                <div style="width: 250px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;">
                                    <asp:Label ID="lblTo" runat="server" Width="240" Text='<%# Eval( "To")%>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Oggetto" ItemStyle-Width="250px" ItemStyle-Wrap="true">
                            <ItemStyle Wrap="true" Width="250px" />
                            <ItemTemplate>
                                <div style="width: 250px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;">
                                    <asp:Label ID="lblSubject" runat="server" Width="240px" Text='<%# Eval("Subject")%>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Operazione" Visible="true" HeaderStyle-Wrap="false"
                            ItemStyle-Width="20px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="lblDataInvio" runat="server" Text='<%#  Eval("ReceiveDate").ToString() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Dimensione" Visible="false" HeaderStyle-Wrap="false"
                            ItemStyle-Width="15px" ItemStyle-CssClass="hide" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="lblDimension" runat="server" Text='<%#  Eval("Dimensione").ToString() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cartella" HeaderStyle-Wrap="false" ItemStyle-Width="15px"
                            ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="lblFolder" runat="server" Text='<%#  Eval("NomeFolder").ToString() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" HeaderStyle-Wrap="false" ItemStyle-Width="15px" Visible="true"
                            ItemStyle-CssClass="hide" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="lblTipo" runat="server" Text='<%#  Eval("FolderTipo").ToString() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="IdFolder" HeaderStyle-Wrap="false" ItemStyle-Width="15px" Visible="false"
                            ItemStyle-CssClass="hide" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="lblIdFolder" runat="server" Text='<%#  Eval("FolderId").ToString() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status" HeaderStyle-Wrap="false" ItemStyle-Width="15px" 
                            ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server" Text='<%#  Eval("MailStatus").ToString() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:CommandField SelectImageUrl="~/App_Themes/Delta/images/buttons/iApri.gif" ButtonType="Image"
                            ShowSelectButton="true" ItemStyle-Width="30px" CausesValidation="false" ItemStyle-HorizontalAlign="Center"
                            HeaderStyle-Wrap="false" />
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
            </asp:Panel>
            <div class="buttons-panel" style="margin-top: 4px">
                <asp:Button ID="btnStampaDistinta" runat="server" Visible="false" Text="Stampa Distinta"
                    CausesValidation="false" OnClick="btnStampa_Click" />
                <asp:Button ID="btnLetta" runat="server" Visible="false" Text="Segna come lette"
                    CausesValidation="false" OnClick="btnLetta_Click" />
                <asp:Button ID="btnUnLetta" runat="server" Visible="false" Text="Segna come da leggere"
                    CausesValidation="false" OnClick="btnUnLetta_Click" />
                <asp:Button ID="btnSposta" runat="server" Visible="false" Text="Sposta" CausesValidation="false"
                    OnClick="btnSposta_Click" />
                <asp:UpdatePanel ID="UpdCartellaSposta" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel runat="server" ID="pnlSposta" CssClass="control-main" Visible="false">
                            <div class="control-header-gray">
                                <div class="control-header-title">
                                    <div class="control-header-text-left">
                                        Scelta cartella
                                    </div>
                                </div>
                            </div>
                            <div class="control-body-gray" style="padding-top: 6px; padding-bottom: 6px;">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="ddlCartellaSposta" CausesValidation="false" runat="server"
                                                SkinID="ddlMiddle">
                                                <asp:ListItem Text="Selezionare un valore" Value="" Selected="True"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Button runat="server" ID="btnOk" SkinID="ImageButton" OnClick="btnOk_Click"
                                                CausesValidation="false" CssClass="iDone" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>
<script type="text/javascript">
    var RIAValidationGroup = 'vgTabCasella';

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
            if (document.getElementById('<%=pnlCasella.ClientID %>') != null)
            { document.getElementById('<%=pnlCasella.ClientID %>').style.display = 'block'; }
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
                    contentEl: document.getElementById('<%=pnlCasella.ClientID %>'),
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
                    RIAValidationGroup = 'vgTabCasella';
                    index = 0;
                    break;
            }
            var tabIndex =document.getElementById('<%=hdTabIndex.ClientID %>').value;
            if (tabIndex != null)
                tabIndex.value = index;
        }

        

        


    });
</script>
