<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntiViewer.ascx.cs" Inherits="GestionePEC.Controls.EntiViewer" %>   
<%@ Import Namespace="SendMail.Model.RubricaMapping" %>
<%@ Import Namespace="SendMail.Model" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="SendMail.Model.Utility" %>
<%@ Import Namespace="GestionePEC.Extensions" %>
<%@ Register Src="~/Controls/Paging.ascx" TagName="UCPaging" TagPrefix="uc2" %>
<%@ Register Assembly="GestionePEC" Namespace="GestionePEC.Extensions" TagPrefix="inl" %>
<div id="FormView1" class="body-panel" style="padding: 5px 5px 5px 5px;" runat="server">
    <asp:HiddenField ID="hidInsertType" runat="server" />
    <div class="control-header-blue">
        <div class="control-header-title">
            <div class="control-header-text-left">
                <label class="header-title">
                    Ente</label>
            </div>
            <div id="headerBut" runat="server" class="control-header-text-right" visible='<%# EntFormView.CurrentMode == FormViewMode.ReadOnly %>'>
                <asp:ImageButton runat="server" ID="ibEntitaEdit" ImageUrl="~/App_Themes/Delta/images/buttons/modify.png"
                    OnClick="ibEntitaEdit_Click" />
            </div>
        </div>
    </div>
    <div class="control-body-gray">
        <asp:ObjectDataSource runat="server" ID="odsEntita" EnableCaching="false" DataObjectTypeName="SendMail.Model.RubricaMapping.RubricaEntita"
            TypeName="SendMail.Model.RubricaMapping.RubricaEntita" SelectMethod="select"
            UpdateMethod="save" OnObjectCreating="odsEntita_ObjectCreating" OnUpdating="odsEntita_Updating" />
        <asp:FormView ID="EntFormView" runat="server" DefaultMode="ReadOnly" Font-Size="Small"
            DataSourceID="odsEntita" OnItemCommand="EntFormView_ItemCommand" OnItemUpdating="EntFormView_ItemUpdating">
            <ItemTemplate>
                <asp:HiddenField runat="server" ID="hfIdEntita" Value='<%# Eval("IdReferral") %>' />
                <div runat="server" id="divOrg">
                    <span class="LabelBlack Fixed120">Organizzazione:</span>
                    <label class="LabelBlackBold">
                        <%# (OrgDen ?? "").Trim() %></label>
                </div>
                <div runat="server" id="divDen">
                    <span class="LabelBlack Fixed120">Denominazione:</span>
                    <label class="LabelBlackBold">
                        <%# String.Format("{0} {1} {2}", Eval("DisambPre"), Eval("RagioneSociale"), Eval("DisambPost")).Trim() %></label>
                </div>
                <div runat="server" id="divType">
                    <span class="LabelBlack Fixed120">Tipo:</span>
                    <label class="LabelBlackBold">
                        <%#(Eval("ReferralType") as Enum).GetDecodedType() %></label>
                </div>
                <div runat="server" id="divUff" visible='<%# ((EntitaType)Eval("ReferralType")).ToString().EndsWith(new string[]{"_UFF", "_PF"}) %>'>
                    <span class="LabelBlack Fixed120">Ufficio:</span>
                    <label class="LabelBlackBold">
                        <%# Eval("Ufficio") %></label>
                </div>
                <div runat="server" id="divSito" visible='<%# new EntitaType[]{ EntitaType.PA, EntitaType.PA_SUB, EntitaType.AZ_PS, EntitaType.AZ_PRI }.Contains((EntitaType)Eval("ReferralType")) %>'>
                    <span class="LabelBlack Fixed120">Sito Web:</span>
                    <label class="LabelBlackBold">
                        <%# Eval("SitoWeb")%></label>
                </div>
                <div runat="server" id="divCognNom" visible='<%# ((EntitaType)Eval("ReferralType")).ToString().EndsWith(new string[]{"PF","PG"}) %>'>
                    <div>
                        <span class="LabelBlack Fixed120">Cognome:</span>
                        <label class="LabelBlackBold">
                            <%# Eval("Cognome")%></label>
                    </div>
                    <div>
                        <span class="LabelBlack Fixed120">Nome:</span>
                        <label class="LabelBlackBold">
                            <%# Eval("Nome")%></label>
                    </div>
                </div>
                <div runat="server" id="divPIvaCodFis" visible='<%# ((EntitaType)Eval("ReferralType")).ToString().Contains(new string[]{ "PF", "PG" }) %>'>
                    <div>
                        <span class="LabelBlack Fixed120">Codice Fiscale:</span>
                        <label class="LabelBlackBold">
                            <%# Eval("CodiceFiscale")%></label>
                    </div>
                    <div>
                        <span class="LabelBlack Fixed120">Partita IVA:</span>
                        <label class="LabelBlackBold">
                            <%# Eval("PartitaIva")%></label>
                    </div>
                </div>
                <div>
                    <span class="LabelBlack Fixed120" style="vertical-align: top;">Note:</span>
                    <div style="max-height: 4em; overflow: auto; display: inline;">
                        <%# Eval("Note")%></div>
                </div>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:HiddenField runat="server" ID="hfIdEntita" Value='<%# Bind("IdReferral") %>' />
                <div runat="server" id="divOrg">
                    <span class="LabelBlack Fixed120">Organizzazione:</span>
                    <label class="LabelBlackBold">
                        <%# (OrgDen ?? "").Trim() %></label>
                </div>
                <div runat="server" id="divDen">
                    <span class="LabelBlack Fixed120">Denominazione:</span>
                    <label class="LabelBlackBold">
                        <%# String.Format("{0} {1} {2}", (Eval("DisambPre") ?? "").ToString(), Eval("RagioneSociale").ToString(), (Eval("DisambPost") ?? "").ToString()).Trim() %></label>
                </div>
                <div runat="server" id="divType">
                    <span class="LabelBlack Fixed120">Tipo:</span>
                    <label class="LabelBlackBold">
                        <%#(Eval("ReferralType") as Enum).GetDecodedType() %></label>
                </div>
                <div runat="server" id="divUff" visible='<%# ((EntitaType)Eval("ReferralType")).ToString().EndsWith(new string[]{"_UFF", "_PF"}) %>'>
                    <span class="LabelBlack Fixed120">Ufficio:</span>
                    <asp:TextBox runat="server" Text='<%#Bind("Ufficio")%>' ID="TxtUfficio" Columns="60"
                        Enabled='<%# ((long?)Eval("RefOrg")).HasValue == true %>' />
                </div>
                <div runat="server" id="divSito" visible='<%# new EntitaType[]{ EntitaType.PA, EntitaType.PA_SUB, EntitaType.AZ_PS, EntitaType.AZ_PRI }.Contains((EntitaType)Eval("ReferralType")) %>'>
                    <span class="LabelBlack Fixed120">Sito Web:</span>
                    <asp:TextBox runat="server" Text='<%#Bind("SitoWeb") %>' ID="TxtSitoWeb" Columns="60" />
                    <asp:RegularExpressionValidator ID="revWebSite" ControlToValidate="TxtSitoWeb" runat="server"
                        ValidationGroup="vgEnt" ErrorMessage="Formato URL non corretto" ValidationExpression="^(http(s)?://)?([\w-]+\.)+[\w-]+(/[\w- ;,./?%&=]*)?$"
                        Display="Dynamic" />
                </div>
                <div runat="server" id="divCognNom" visible='<%# ((EntitaType)Eval("ReferralType")).ToString().EndsWith(new string[]{"PF","PG"}) %>'>
                    <div>
                        <span class="LabelBlack Fixed120">Cognome:</span>
                        <asp:TextBox runat="server" Text='<%#Bind("Cognome") %>' ID="TxtCognome" CssClass="FixedBorder300"
                            Font-Bold="true" Columns="60" />
                        <asp:RegularExpressionValidator ID="revCognome" ControlToValidate="TxtCognome" runat="server"
                            ValidationExpression="^[A-Za-zÀÃÄÇÈÉËÌÏÒÖÙÜàãäçèéëìïòöùü'\- ]*$" ErrorMessage="Formato Cognome"
                            ValidationGroup="vgEnt" Display="Dynamic" />
                    </div>
                    <div>
                        <span class="LabelBlack Fixed120">Nome:</span>
                        <asp:TextBox runat="server" Text='<%#Bind("Nome") %>' ID="TxtNome" CssClass="FixedBorder300"
                            Font-Bold="true" Columns="60" />
                        <asp:RegularExpressionValidator ID="revNome" ControlToValidate="TxtNome" runat="server"
                            ValidationExpression="^[A-Za-zÀÃÄÇÈÉËÌÏÒÖÙÜàãäçèéëìïòöùü'\- ]*$" ErrorMessage="Formato Nome"
                            Display="Dynamic" ValidationGroup="vgEnt" />
                    </div>
                </div>
                <div runat="server" id="divPIvaCodFis" visible='<%# ((EntitaType)Eval("ReferralType")).ToString().Contains(new string[]{ "PF", "PG" }) %>'>
                    <div>
                        <span class="LabelBlack Fixed120">Codice Fiscale:</span>
                        <asp:TextBox runat="server" CssClass="Fixed300" Font-Bold="true" Text='<%#Bind("CodiceFiscale") %>'
                            ID="TxtCodFis" Columns="60" />
                        <asp:CustomValidator runat="server" ID="custCodFisValidate" ControlToValidate="TxtCodFis"
                            Display="Dynamic" ErrorMessage="Codice fiscale" ValidationGroup="vgEnt" ValidateEmptyText="false"
                            OnServerValidate="custCodFisValidate_ServerValidate" ClientValidationFunction="CheckCodFis" />
                        <inl:InlineScript ID="InlineScript1" runat="server">

                            <script type="text/javascript" defer="defer">
                                var txtCodFis = Ext.get('<%= TxtCodFisClientID %>');
                                if (txtCodFis != null) {
                                    txtCodFis.on('blur', function(e, t, o) {
                                        t.value = t.value.toUpperCase();
                                    });
                                };
                            </script>

                        </inl:InlineScript>
                    </div>
                    <div>
                        <span class="LabelBlack Fixed120">Partita IVA:</span>
                        <asp:TextBox runat="server" CssClass="Fixed300" Font-Bold="true" Text='<%#Bind("PartitaIva") %>'
                            ID="TextPIva" Columns="60" />
                        <asp:RegularExpressionValidator ID="revPIva" ControlToValidate="TextPIva" runat="server"
                            ValidationExpression="^[0-9]{11}$" ErrorMessage="Parita Iva" Display="Dynamic"
                            ValidationGroup="vgEnt" />
                    </div>
                </div>
                <div>
                    <span class="LabelBlack Fixed120" style="vertical-align: top;">Note:</span>
                    <asp:TextBox runat="server" TextMode="MultiLine" Rows="3" Font-Bold="true" Text='<%#Bind("Note") %>'
                        ID="TxtNote" Columns="60" Width="400" />
                </div>
                <div style="text-align: left; margin: 10px 0 5px 435px;">
                    <asp:Button ID="btnEntUpdate" runat="server" ToolTip="Aggiorna i dati inseriti" CommandName="Update"
                        Text="Salva Modifiche" ValidationGroup="vgEnt" OnClientClick="" />
                    <inl:InlineScript ID="InlineScript2" runat="server">

                        <script type="text/javascript">
                            function ToUpperCase() {
                                if (typeof txtCodFis != 'undefined' && txtCodFis != null) {
                                    t.set({ value: t.value.toUpperCase() });
                                }
                            };
                        </script>

                    </inl:InlineScript>
                </div>
            </EditItemTemplate>
        </asp:FormView>
        <asp:ScriptManagerProxy ID="ScriptManagerProxy" runat="server">
            <Scripts>
                <asp:ScriptReference Path="~/scripts/CheckCodiceFiscale.js" />
            </Scripts>
        </asp:ScriptManagerProxy>
    </div>
</div>
<div style="clear: both;" />
<div id="FormView2" class="body-panel" style="padding: 5px 5px 5px 5px;" runat="server">
    <div class="control-header-blue">
        <div class="control-header-title">
            <div class="control-header-text-left">
                <label class="header-title">
                    Contatti</label>
            </div>
            <div class="header-text-right">
                <asp:ImageButton runat="server" ID="btnContactInsert" ToolTip="Crea nuovo contatto"
                    ImageUrl="~/App_Themes/Delta/images/buttons/add_contact.gif" Width="24px" ImageAlign="Top"
                    CommandName="Insert_c"
                    OnClick="ContactsFormView_InsertCommand" />
                <asp:ImageButton runat="server" ID="ImageButton1" ToolTip="Crea nuovo ufficio" ImageAlign="Top" ImageUrl="~/App_Themes/Delta/images/buttons/building_add.png"
                    Width="24px" CommandName="Insert_u" OnClick="ContactsFormView_InsertCommand" />
                <asp:ImageButton runat="server" ID="ImageButton2" ToolTip="Crea nuovo gruppo" ImageAlign="Top" ImageUrl="~/App_Themes/Delta/images/buttons/company_star.png"
                    Width="24px" CommandName="Insert_g" OnClick="ContactsFormView_InsertCommand" />
            </div>
        </div>
    </div>
    <div class="control-body-gray">
        <asp:GridView ID="gvContacts" AutoGenerateColumns="false" runat="server" AllowPaging="true"
            RowStyle-Wrap="true" OnRowCommand="gvContacts_RowCommand">
            <Columns>
                <asp:TemplateField HeaderText="Email">
                    <ItemTemplate>
                        <asp:Label ID="Label4" runat="server" Text='<%#Eval("Mail") %>' />
                        <asp:HiddenField ID="IdRef" runat="server" Value='<%#Eval("IdContact") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Telefono" HeaderText="Telefono" />
                <asp:BoundField DataField="Fax" HeaderText="Fax" />
                <asp:ButtonField ButtonType="Image" ImageUrl="~/App_Themes/Delta/images/buttons/select.png" />
            </Columns>
            <PagerTemplate>
                <uc2:UCPaging ID="ucContactsPaging" runat="server" OnPagerIndexChanged="OnContactsPagerIndexChanged"
                    OnInit="ucContactsPaging_Init" />
            </PagerTemplate>
            <EmptyDataTemplate>
                Nessun contatto presente</EmptyDataTemplate>
        </asp:GridView>
        <div style="clear: both; margin-bottom: 10px;">
        </div>
        <asp:FormView ID="ContactsFormView" runat="server" DefaultMode="Edit" Font-Size="Small"
            OnItemCommand="ContactsFormView_ItemCommand" OnItemUpdating="ContactsFormView_ItemUpdating"
            OnItemInserting="ContactsFormView_ItemInserting" OnItemInserted="ContactsFormView_ItemInserted"
            OnModeChanging="ContactsFormView_ModeChanging" OnDataBound="ContactsFormView_DataBound">
            <EditItemTemplate>
                <asp:HiddenField ID="hfIdContact" runat="server" Value='<%# Eval("IdContact") %>' />
                <div>
                    <span class="LabelBlack Fixed120">Telefono:</span>
                    <asp:TextBox ID="TextTelefono" runat="server" CssClass="Fixed300" Font-Bold="true"
                        Text='<%#Bind("Telefono")%>' Columns="60" />
                    <asp:RegularExpressionValidator ID="revTel" ControlToValidate="TextTelefono" runat="server"
                        ValidationExpression="^[\+]?[0-9]{2,}[/\-\.]?[0-9]{3,}$" ErrorMessage="Formato telefono non corretto"
                        Display="Dynamic" ValidationGroup="vgEditContact" />
                </div>
                <div>
                    <span class="LabelBlack Fixed120">Email:</span>
                    <asp:TextBox ID="TextMail" runat="server" CssClass="Fixed300" Font-Bold="true" Text='<%#Bind("Mail")%>'
                        Columns="60" />
                    <asp:RegularExpressionValidator ID="revMail" OnInit="revMail_Init" ControlToValidate="TextMail" runat="server"
                        ErrorMessage="Formato email non corretto" Display="Dynamic" ValidationGroup="vgEditContact" />
                    <asp:CheckBox runat="server" Text="PEC" ToolTip="Posta Certificata" ID="chkPec" BorderColor="LightSteelBlue"
                        ForeColor="#4f4f4f" Checked='<%# Bind("IsPec") %>' />
                </div>
                <div>
                    <span class="LabelBlack Fixed120">Fax:</span>
                    <asp:TextBox ID="TextFax" runat="server" CssClass="Fixed300" Font-Bold="true" Text='<%#Bind("Fax")%>'
                        Columns="60" />
                    <asp:RegularExpressionValidator ID="revFax" ControlToValidate="TextFax" runat="server"
                        ValidationExpression="^[\+]?[0-9]{2,}[/\-\.]?[0-9]{3,}$" ErrorMessage="Formato fax non corretto"
                        Display="Dynamic" ValidationGroup="vgEditContact" />
                </div>
                <div>
                    <span class="LabelBlack Fixed120">Riferimento:</span>
                    <asp:TextBox ID="TextRef" runat="server" CssClass="Fixed300" Font-Bold="true" Text='<%#Bind("ContactRef")%>'
                        Columns="60" />
                </div>
                <div>
                    <span class="LabelBlack Fixed120">Note:</span>
                    <asp:TextBox ID="TextNote" runat="server" TextMode="MultiLine" Rows="1" Font-Bold="true"
                        Text='<%#Bind("Note")%>' Columns="60" Width="400" />
                </div>
                <div style="text-align: left; margin: 10px 0 5px 435px;">
                    <asp:Button ID="btnCommand" runat="server" ToolTip="Aggiorna i dati inseriti" CommandName="Update"
                        Text="Salva Modifiche" ValidationGroup="vgEditContact" />
                </div>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:Panel ID="panelGrp" runat="server">
                    <span class="LabelBlack Fixed120">Gruppo:</span>
                    <asp:TextBox ID="TextGruppo" runat="server" Font-Bold="true" CssClass="Fixed300"
                        Columns="60" />
                </asp:Panel>
                <asp:Panel ID="panelUff" runat="server">
                    <span class="LabelBlack Fixed120">Ufficio:</span>
                    <asp:TextBox ID="TextUfficio" runat="server" Font-Bold="true" CssClass="Fixed300"
                        Columns="60" />
                </asp:Panel>
                <asp:Panel ID="panelCon" runat="server">
                    <div>
                        <span class="LabelBlack Fixed120">Telefono:</span>
                        <asp:TextBox ID="TextTelefono" runat="server" Font-Bold="true" Text='<%#Bind("Telefono")%>'
                            CssClass="Fixed300" Columns="60" />
                        <asp:RegularExpressionValidator ID="revTel" ControlToValidate="TextTelefono" runat="server"
                            ValidationExpression="^[\+]?[0-9]{2,}[/\-\.]?[0-9]{3,}$" ErrorMessage="Formato telefono non corretto"
                            Display="Dynamic" ValidationGroup="vgInsertContact" />
                    </div>
                    <div>
                        <span class="LabelBlack Fixed120">Email:</span>
                        <asp:TextBox ID="TextMail" runat="server" Font-Bold="true" Text='<%#Bind("Mail")%>'
                            CssClass="Fixed300" Columns="60" />
                        <asp:RegularExpressionValidator ID="revMail" OnInit="revMail_Init" ControlToValidate="TextMail" runat="server"
                            ErrorMessage="Formato email non corretto" Display="Dynamic" ValidationGroup="vgInsertContact" />
                        <asp:CheckBox runat="server" Text="PEC" ToolTip="Posta Certificata" ID="chkPec" BorderColor="LightSteelBlue"
                            ForeColor="#4f4f4f" Checked="false" />
                    </div>
                    <div>
                        <span class="LabelBlack Fixed120">Fax:</span>
                        <asp:TextBox ID="TextFax" runat="server" Font-Bold="true" Text='<%#Bind("Fax")%>'
                            CssClass="Fixed300" Columns="60" />
                        <asp:RegularExpressionValidator ID="revFax" ControlToValidate="TextFax" runat="server"
                            ValidationExpression="^[\+]?[0-9]{2,}[/\-\.]?[0-9]{3,}$" ErrorMessage="Formato fax non corretto"
                            Display="Dynamic" ValidationGroup="vgInsertContact" />
                    </div>
                    <div>
                        <span class="LabelBlack Fixed120">Riferimento:</span>
                        <asp:TextBox ID="TextRef" runat="server" Font-Bold="true" Text='<%#Bind("ContactRef")%>'
                            CssClass="Fixed300" Columns="60" />
                    </div>
                    <div>
                        <span class="LabelBlack Fixed120">Note:</span>
                        <asp:TextBox ID="TextNote" runat="server" TextMode="MultiLine" Rows="1" Font-Bold="true"
                            Text='<%#Bind("Note")%>' CssClass="Fixed300" Columns="60" Width="400" />
                    </div>
                </asp:Panel>
                <div style="text-align: left; margin: 10px 0 5px 435px;">
                    <asp:Button ID="btnCommand" runat="server" ToolTip="Inserisci nuovo contatto" CommandName="Insert"
                        Text="Inserisci Nuovo" ValidationGroup="vgInsertContact" />
                </div>
            </InsertItemTemplate>
        </asp:FormView>
    </div>
</div>
