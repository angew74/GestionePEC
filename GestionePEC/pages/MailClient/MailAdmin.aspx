<%@ Page Title="Amministrazione" Language="C#" MasterPageFile="~/Master/Mail.Master" Theme="Delta" AutoEventWireup="true" CodeBehind="MailAdmin.aspx.cs" Inherits="GestionePEC.pages.MailClient.MailAdmin" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/MailBoxNavigator.ascx" TagName="MailNav" TagPrefix="mail" %>
<%@ Import Namespace="System.Linq" %>

<asp:Content ID="Content1" ContentPlaceHolderID="WestContentPlaceHolder" runat="server">
      <mail:MailNav ID="Navigator" runat="server" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
   <div class="content-panel-borderless">
        <div class="header-panel-blue">
            <div class="header-title">
                <div class="header-text-left">
                    <label>
                        Configurazione Mail</label>
                </div>
                <div class="header-text-right">
                </div>
            </div>
        </div>
        <div id="container" class="body-panel">
            <asp:UpdatePanel ID="upGestioneEmail" runat="server">
                <ContentTemplate>
                    <div class="grid-panel">
                        <asp:GridView ID="gvElencoEmailsShared" runat="server" AutoGenerateColumns="false"
                            OnRowCommand="gvElencoEmailsShared_RowCommand" OnPageIndexChanging="gvElencoEmailsShared_PageIndexChanging"
                            DataKeyNames="EMailAddress" AllowPaging="true" EnableViewState="true">
                            <Columns>
                                <asp:BoundField DataField="EMailAddress" HeaderText="Descrizione">
                                    <ItemStyle HorizontalAlign="Left" Wrap="false" />
                                </asp:BoundField>
                                <asp:TemplateField ItemStyle-Width="80px">
                                    <HeaderTemplate>
                                        <asp:Label ID="lbEmail" runat="server" Text="Email" SkinID="lab_blue_bold_10"></asp:Label>
                                        <asp:ImageButton ID="ibAggiungiEmail" ToolTip="Aggiungi Email" runat="server" CausesValidation="false"
                                            ImageUrl="~/App_Themes/Delta/images/buttons/add.png" Visible='<%# bUser.UserRole == 2 %>'
                                            OnClick="btnInserimentoEmail_OnClick" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ibGestioneEmail" runat="server" CommandName="GestioneEmail"
                                            CommandArgument='<%# Eval("UserId") %>' CssClass="iSelect" ToolTip="Gestione Email"
                                            CausesValidation="false" ImageUrl="~/App_Themes/Delta/images/buttons/Email.png" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="80px">
                                    <HeaderTemplate>
                                        <asp:Label ID="lbServer" runat="server" Text="Server" SkinID="lab_blue_bold_10"></asp:Label>
                                        <asp:ImageButton ID="ibAggiungiServer" ToolTip="Aggiungi Server" runat="server" CausesValidation="false"
                                            ImageUrl="~/App_Themes/Delta/images/buttons/server.png" Visible='<%# bUser.UserRole == 2 %>'
                                            OnClick="btnInserimentoServer_OnClick" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ibGestioneServer" runat="server" CommandName="GestioneServer"
                                            CommandArgument='<%# Eval("Id") %>' CssClass="iSelect" ToolTip="Gestione Server"
                                            CausesValidation="false" ImageUrl="~/App_Themes/Delta/images/buttons/server.png" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="80px" HeaderText="Utenti">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ibGestioneAssociazione" runat="server" CommandName="GestioneAssociazione"
                                            CommandArgument='<%# Eval("UserId") %>' CssClass="iVisualizza" ToolTip="Gestione Associazione"
                                            CausesValidation="false" ImageUrl="~/App_Themes/Delta/images/buttons/user.png"
                                            Visible='<%# int.Parse(Eval("MailAccessLevel").ToString()) == 1 %>' />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                 <asp:TemplateField ItemStyle-Width="80px" HeaderText="Amministratori">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ibGestioneAssociazioneAdmin" runat="server" CommandName="GestioneAssociazioneAdmin"
                                            CommandArgument='<%# Eval("UserId") %>' CssClass="iVisualizza" ToolTip="Gestione Amministratori"
                                            CausesValidation="false" ImageUrl="~/App_Themes/Delta/images/buttons/administrator.png"
                                            Visible='<%# int.Parse(Eval("MailAccessLevel").ToString()) == 1 %>' />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="80px" HeaderText="Folders">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ibElencoFolders" runat="server" CommandName="GestioneFolders"
                                            CommandArgument='<%# Eval("EMailAddress") %>'  ToolTip="Gestione Folders"
                                            CausesValidation="false" ImageUrl="~/App_Themes/Delta/images/buttons/folder.png"
                                            Visible="true" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                Utente non abilitato a nessuna mail
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                    <asp:Panel ID="pnlGestioneEmail" runat="server" CssClass="content-panel" Visible="false">
                        <div class="header-panel-gray">
                            <div class="header-title">
                                <div class="header-text-left">
                                    <asp:Label runat="server" SkinID="lab_blue_bold_10" ID="ControlHeaderLabel" Text="Dettaglio Email"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="body-panel-gray">
                            <div class="tabella" style="margin-top: 5px; text-align: left;">
                                <div class="grid-panel">
                                    <asp:ObjectDataSource runat="server" ID="odsMailConfig" EnableCaching="false" DataObjectTypeName="ActiveUp.Net.Mail.DeltaExt.MailUser"
                                        TypeName="ActiveUp.Net.Mail.DeltaExt.MailUser" SelectMethod="selectUser" UpdateMethod="saveUser"
                                        InsertMethod="saveUser" OnObjectCreating="odsMailConfig_ObjectCreating" OnInserting="odsMailConfig_Inserting"
                                        OnUpdating="odsMailConfig_Updating" />
                                    <asp:FormView ID="fvEmail" runat="server" DataKeyNames="UserId" DataSourceID="odsMailConfig"
                                        DefaultMode="ReadOnly" OnModeChanging="fvEmail_ModeChanging" OnItemInserting="fvEmail_ItemInserting"
                                        OnItemUpdating="fvEmail_ItemUpdating" OnItemCommand="fvEmail_ItemCommand">
                                        <ItemTemplate>
                                            <div style="display: table-row">
                                                <div style="width: 180px; display: table-cell">
                                                    <label class="LabelBlackMiddle">
                                                        Mail:</label>
                                                </div>
                                                <div style="display: table-cell">
                                                    <label class="LabelBlackBold">
                                                        <%# Eval("EmailAddress") %></label>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="width: 180px; display: table-cell">
                                                    <label class="LabelBlackMiddle">
                                                        Nome Utente:</label>
                                                </div>
                                                <div style="display: table-cell">
                                                    <label class="LabelBlackBold">
                                                        <%# Eval("LoginId") %></label>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="width: 180px; display: table-cell">
                                                    <label class="LabelBlackMiddle" style="min-width: 200px;">
                                                        Server Associato all'Email:</label>
                                                </div>
                                                <div style="display: table-cell">
                                                    <label class="LabelBlackBold">
                                                        <%# MailServers.SingleOrDefault(x => x.Id == decimal.Parse(Eval("Id").ToString())).DisplayName %></label>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <div style="display: table-row">
                                                <div style="width: 180px; display: table-cell">
                                                    <label class="LabelBlackMiddle">
                                                        Mail:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <label runat="server" class="LabelBlackBold" visible='<%# bUser.UserRole != 2 %>'>
                                                        <%# Eval("EmailAddress") %></label>
                                                    <asp:TextBox SkinID="tbbox18Char" runat="server" ID="tbDescrizioneEmail" Width="300"
                                                        Text='<%# Bind("EmailAddress") %>' Visible='<%# bUser.UserRole == 2 %>' />
                                                    <asp:RequiredFieldValidator ID="rfvDescrizioneEmail" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="tbNomeUtente" Display="None" ValidationGroup="btnSalvaEmail"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vceDescrizioneEmail" CssClass="CustomValidator"
                                                        runat="server" TargetControlID="rfvDescrizioneEmail">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="width: 180px; display: table-cell">
                                                    <label class="LabelBlackMiddle">
                                                        Nome Utente:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <label runat="server" class="LabelBlackBold" visible='<%# bUser.UserRole != 2 %>'>
                                                        <%# Eval("LoginId") %></label>
                                                    <asp:TextBox SkinID="tbbox18Char" runat="server" ID="tbNomeUtente" Width="300" Text='<%# Bind("LoginId") %>'
                                                        Visible='<%# bUser.UserRole == 2 %>' />
                                                    <asp:RequiredFieldValidator ID="rfvNomeUtente" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="tbNomeUtente" Display="None" ValidationGroup="btnSalvaEmail"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vceNomeUtente" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvNomeUtente">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="width: 180px; display: table-cell">
                                                    <label class="LabelBlackMiddle">
                                                        Nuova Password:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:TextBox SkinID="tbbox18Char" TextMode="Password" runat="server" ID="tbNuovaPassword"
                                                        Width="300" MaxLength="16" Text='<%# Bind("Password") %>'></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="tbNuovaPassword" Display="None" ValidationGroup="btnSalvaEmail"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vcePassword" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvPassword">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="width: 180px; display: table-cell">
                                                    <label class="LabelBlackMiddle">
                                                        Conferma Nuova Password:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:TextBox SkinID="tbbox18Char" TextMode="Password" runat="server" ID="tbConfermaNuovaPassword"
                                                        Width="300" MaxLength="16"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvConfPassw" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="tbConfermaNuovaPassword" Display="None" ValidationGroup="btnSalvaEmail"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vceNuovaPassword" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvConfPassw">
                                                    </cc1:ValidatorCalloutExtender>
                                                    <asp:CompareValidator runat="server" ID="cvNuovaPassword" ControlToCompare="tbNuovaPassword"
                                                        ControlToValidate="tbConfermaNuovaPassword" Display="None" ErrorMessage="Password diversa"
                                                        ValidationGroup="btnSalvaEmail" Operator="Equal" Type="String" />
                                                    <cc1:ValidatorCalloutExtender runat="server" ID="vceNewPassword" CssClass="CustomValidator"
                                                        TargetControlID="cvNuovaPassword" />
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="width: 180px; display: table-cell">
                                                    <label class="LabelBlackMiddle">
                                                        Server Associato all'Email:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <label runat="server" class="LabelBlackBold" visible='<%# bUser.UserRole != 2 %>'>
                                                        <%# MailServers.SingleOrDefault(x => x.Id == decimal.Parse(Eval("Id").ToString())).DisplayName %></label>
                                                    <asp:DropDownList ID="ddlListaServers" runat="server" DataTextField="DisplayName"
                                                        AutoPostBack="false" DataValueField="Id" Width="300px" DataSource='<%# MailServers %>'
                                                        SelectedValue='<%# Bind("Id") %>' Visible='<%# bUser.UserRole == 2 %>'>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div style="clear: both; min-height: 10px;">
                                            </div>
                                            <div class="buttons-panel" style="margin-top: 4px; margin-right: 8px">
                                                <asp:Button ID="butSalvaEmail" runat="server" Text="Salva" ToolTip="Effettua il Salvataggio"
                                                    CssClass="upd" ValidationGroup="btnSalvaEmail" CommandName="Update"></asp:Button>
                                                <asp:Button ID="butAnnullaEmail" runat="server" Text="Annulla" ToolTip="Annulla"
                                                    CssClass="upd" CausesValidation="false" CommandName="Cancel"></asp:Button>
                                            </div>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <div style="display: table-row">
                                                <div style="width: 180px; display: table-cell">
                                                    <label class="LabelBlackMiddle">
                                                        Mail:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:TextBox SkinID="tbbox18Char" runat="server" ID="tbDescrizioneEmail" Width="300"
                                                        Text='<%# Bind("EmailAddress") %>' />
                                                    <asp:RequiredFieldValidator ID="rfvDescrizioneEmail" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="tbNomeUtente" Display="None" ValidationGroup="btnSalvaEmail"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vceDescrizioneEmail" CssClass="CustomValidator"
                                                        runat="server" TargetControlID="rfvDescrizioneEmail">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="width: 180px; display: table-cell">
                                                    <label class="LabelBlackMiddle">
                                                        Nome Utente:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:TextBox SkinID="tbbox18Char" runat="server" ID="tbNomeUtente" Width="300" Text='<%# Bind("LoginId") %>' />
                                                    <asp:RequiredFieldValidator ID="rfvNomeUtente" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="tbNomeUtente" Display="None" ValidationGroup="btnSalvaEmail"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vceNomeUtente" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvNomeUtente">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="width: 180px; display: table-cell">
                                                    <label class="LabelBlackMiddle">
                                                        Nuova Password:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:TextBox SkinID="tbbox18Char" TextMode="Password" runat="server" ID="tbNuovaPassword"
                                                        Width="300" MaxLength="16" Text='<%# Bind("Password") %>'></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="tbNuovaPassword" Display="None" ValidationGroup="btnSalvaEmail"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vcePassword" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvPassword">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="width: 180px; display: table-cell">
                                                    <label class="LabelBlackMiddle">
                                                        Conferma Nuova Password:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:TextBox SkinID="tbbox18Char" TextMode="Password" runat="server" ID="tbConfermaNuovaPassword"
                                                        Width="300" MaxLength="16"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvConfPassw" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="tbConfermaNuovaPassword" Display="None" ValidationGroup="btnSalvaEmail"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vceNuovaPassword" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvConfPassw">
                                                    </cc1:ValidatorCalloutExtender>
                                                    <asp:CompareValidator runat="server" ID="cvNuovaPassword" ControlToCompare="tbNuovaPassword"
                                                        ControlToValidate="tbConfermaNuovaPassword" Display="None" ErrorMessage="Password diversa"
                                                        ValidationGroup="btnSalvaEmail" Operator="Equal" Type="String" />
                                                    <cc1:ValidatorCalloutExtender runat="server" ID="vceNewPassword" CssClass="CustomValidator"
                                                        TargetControlID="cvNuovaPassword" />
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="width: 180px; display: table-cell">
                                                    <label class="LabelBlackMiddle">
                                                        Server Associato all'Email:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:DropDownList ID="ddlListaServers" runat="server" DataTextField="DisplayName"
                                                        DataValueField="Id" Width="300px" DataSource='<%# MailServers %>' SelectedValue='<%# Bind("Id") %>' />
                                                </div>
                                            </div>
                                            <div style="clear: both; min-height: 10px;">
                                            </div>
                                            <div class="buttons-panel" style="margin-top: 4px; margin-right: 8px">
                                                <asp:Button ID="butSalvaEmail" runat="server" Text="Salva" ToolTip="Effettua il Savataggio"
                                                    CssClass="upd" ValidationGroup="btnSalvaEmail" CommandName="Insert"></asp:Button>
                                                <asp:Button ID="butAnnullaEmail" runat="server" Text="Annulla" ToolTip="Annulla"
                                                    CssClass="upd" CausesValidation="false" CommandName="Cancel"></asp:Button>
                                            </div>
                                        </InsertItemTemplate>
                                    </asp:FormView>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnlDettaglioEmailServer" runat="server" CssClass="content-panel" Visible="false">
                        <div class="header-panel-gray">
                            <div class="header-title">
                                <div class="header-text-left">
                                    <label>
                                        Dettaglio Email Server</label>
                                </div>
                            </div>
                        </div>
                        <div class="body-panel-gray">
                            <div class="tabella" style="margin-top: 5px; text-align: left;">
                                <div class="grid-panel">
                                    <asp:ObjectDataSource runat="server" ID="odsServerConfig" EnableCaching="false" DataObjectTypeName="ActiveUp.Net.Mail.DeltaExt.MailServer"
                                        TypeName="ActiveUp.Net.Mail.DeltaExt.MailServer" SelectMethod="selectServer"
                                        UpdateMethod="saveServer" InsertMethod="saveServer" OnObjectCreating="odsServerConfig_ObjectCreating"
                                        OnInserting="odsServerConfig_Inserting" OnUpdating="odsServerConfig_Updating" />
                                    <asp:FormView runat="server" ID="fvServer" DefaultMode="ReadOnly" DataKeyNames="Id"
                                        DataSourceID="odsServerConfig" OnModeChanging="fvServer_ModeChanging" OnItemInserting="fvServer_ItemInserting"
                                        OnItemUpdating="fvServer_ItemUpdating" OnItemCommand="fvServer_ItemCommand">
                                        <ItemTemplate>
                                            <div style="display: table-row">
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        Nome:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlackBold">
                                                        <%# Eval("DisplayName") %></label>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        Dominio:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlackBold">
                                                        <%# Eval("Dominus")%></label>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="display: table-cell; width: 200px;">
                                                    <label class="LabelBlack">
                                                        Posta Elettronica Certificata:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:CheckBox runat="server" ID="cbPEC" Checked='<%# Convert.ToBoolean(Eval("IsPec")) %>'
                                                        Enabled="false" />
                                                </div>
                                            </div>
                                            <div style="clear: both; min-height: 20px;" />
                                            <div style="display: table-row">
                                                <div style="display: table-cell; width: 200px;">
                                                    <label class="LabelBlack">
                                                        Indirizzo di Ingresso:</label>
                                                </div>
                                                <div style="display: table-cell; width: 200px;">
                                                    <label class="LabelBlackBold">
                                                        <%# Eval("IncomingServer") %></label>
                                                </div>
                                                <div style="display: table-cell; width: 180px;">
                                                    <label class="LabelBlack">
                                                        Indirizzo di Uscita:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlackBold">
                                                        <%# Eval("OutgoingServer")%></label>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        Porta di Ingresso:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlackBold">
                                                        <%# Eval("PortIncomingServer")%></label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        Porta di Uscita:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlackBold">
                                                        <%# Eval("PortOutgoingServer")%></label>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        SSL di Ingresso:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:CheckBox runat="server" ID="cbSSLIn" Checked='<%# Convert.ToBoolean(Eval("IsIncomeSecureConnection")) %>'
                                                        Enabled="false" />
                                                </div>
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        SSL di Uscita:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:CheckBox runat="server" ID="cbSSLOut" Checked='<%# Convert.ToBoolean(Eval("IsOutgoingSecureConnection")) %>'
                                                        Enabled="false" />
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        Protocollo di Ingresso:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlackBold">
                                                        <%# Eval("IncomingProtocol") %> </label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        Autorizzazione di Uscita:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:CheckBox runat="server" ID="cbAuthOut" Checked='<%# Convert.ToBoolean(Eval("IsOutgoingWithAuthentication")) %>'
                                                        Enabled="false" />
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:HiddenField runat="server" ID="hfIdServer" Value='<%# Eval("Id") %>' />
                                            <div style="display: table-row">
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        Nome:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:TextBox ID="txtNOME" runat="server" Width="250px" Text='<%# Bind("DisplayName") %>' />
                                                    <asp:RequiredFieldValidator ID="rfvNOME" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="txtNOME" Display="None" ValidationGroup="btnSalvaServer" SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vceNOME" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvNOME">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        Dominio:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:TextBox ID="txtDOMINUS" runat="server" Width="250px" Text='<%# Bind("Dominus")  %>' />
                                                    <asp:RequiredFieldValidator ID="rfvDOMINUS" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="txtDOMINUS" Display="None" ValidationGroup="btnSalvaServer"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vceDOMINUS" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvDOMINUS">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="display: table-cell; width: 200px;">
                                                    <label class="LabelBlack">
                                                        Posta Elettronica Certificata:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:CheckBox runat="server" ID="cbPEC" Checked='<%# Bind("IsPec") %>' />
                                                </div>
                                            </div>
                                            <div style="clear: both; min-height: 20px;" />
                                            <div style="display: table-row">
                                                <div style="display: table-cell; width: 200px;">
                                                    <label class="LabelBlack">
                                                        Indirizzo di Ingresso:</label>
                                                </div>
                                                <div style="display: table-cell; width: 200px;">
                                                    <asp:TextBox ID="txtINDIRIZZO_IN" runat="server" Width="150px" Text='<%# Bind("IncomingServer") %>' />
                                                    <asp:RequiredFieldValidator ID="rfvINDIRIZZO_IN" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="txtINDIRIZZO_IN" Display="None" ValidationGroup="btnSalvaServer"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vceINDIRIZZO_IN" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvINDIRIZZO_IN">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                                <div style="display: table-cell; width: 180px;">
                                                    <label class="LabelBlack">
                                                        Indirizzo di Uscita:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:TextBox ID="txtINDIRIZZO_OUT" runat="server" Width="150px" Text='<%# Bind("OutgoingServer") %>' />
                                                    <asp:RequiredFieldValidator ID="rfvINDIRIZZO_OUT" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="txtINDIRIZZO_OUT" Display="None" ValidationGroup="btnSalvaServer"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vceINDIRIZZO_OUT" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvINDIRIZZO_OUT">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        Porta di Ingresso:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:TextBox ID="txtPORTA_IN" runat="server" Width="150px" Text='<%# Bind("PortIncomingServer") %>'
                                                        onkeyup="onlyDigitsInteger(this);" />
                                                    <asp:RequiredFieldValidator ID="rfvPORTA_IN" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="txtPORTA_IN" Display="None" ValidationGroup="btnSalvaServer"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vcePORTA_IN" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvPORTA_IN">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        Porta di Uscita:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:TextBox ID="txtPORTA_OUT" runat="server" Width="150px" Text='<%# Bind("PortOutgoingServer") %>'
                                                        onkeyup="onlyDigitsInteger(this);" />
                                                    <asp:RequiredFieldValidator ID="rfvPORTA_OUT" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="txtPORTA_OUT" Display="None" ValidationGroup="btnSalvaServer"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vcePORTA_OUT" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvPORTA_OUT">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        SSL di Ingresso:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:CheckBox runat="server" ID="cbSSLIn" Checked='<%# Bind("IsIncomeSecureConnection") %>' />
                                                </div>
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        SSL di Uscita:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:CheckBox runat="server" ID="cbSSLOut" Checked='<%# Bind("IsOutgoingSecureConnection") %>' />
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        Protocollo di Ingresso:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:TextBox ID="txtPROTOCOLLO_IN" runat="server" Width="150px" Text='<%# Bind("IncomingProtocol") %>'
                                                        onkeyup="onlyDigitsInteger(this);" />
                                                    <asp:RequiredFieldValidator ID="rfvPROTOCOLLO_IN" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="txtPROTOCOLLO_IN" Display="None" ValidationGroup="btnSalvaServer"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vcePROTOCOLLO_IN" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvPROTOCOLLO_IN">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        Autorizzazione di Uscita:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:CheckBox runat="server" ID="cbAuthOut" Checked='<%# Bind("IsOutgoingWithAuthentication") %>' />
                                                </div>
                                            </div>
                                            <div style="clear: both; height: 10px;">
                                            </div>
                                            <div class="buttons-panel" style="margin-top: 4px; margin-right: 8px">
                                                <asp:Button ID="btnSalvaServer" runat="server" Text="Salva" ToolTip="Effettua il Savataggio"
                                                    CssClass="upd" CausesValidation="true" ValidationGroup="btnSalvaServer" CommandName="update">
                                                </asp:Button>
                                                <asp:Button ID="butAnnullaEmail" runat="server" Text="Annulla" ToolTip="Annulla"
                                                    CssClass="upd" CausesValidation="false" CommandName="annulla"></asp:Button>
                                            </div>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <div style="display: table-row">
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        Nome:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:TextBox ID="txtNOME" runat="server" Width="250px" Text='<%# Bind("DisplayName") %>' />
                                                    <asp:RequiredFieldValidator ID="rfvNOME" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="txtNOME" Display="None" ValidationGroup="btnSalvaServer" SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vceNOME" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvNOME">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        Dominio:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:TextBox ID="txtDOMINUS" runat="server" Width="250px" Text='<%# Bind("Dominus") %>' />
                                                    <asp:RequiredFieldValidator ID="rfvDOMINUS" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="txtDOMINUS" Display="None" ValidationGroup="btnSalvaServer"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vceDOMINUS" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvDOMINUS">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="display: table-cell; width: 200px;">
                                                    <label class="LabelBlack">
                                                        Posta Elettronica Certificata:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:CheckBox runat="server" ID="cbPEC" Checked='<%# Bind("IsPec") %>' />
                                                </div>
                                            </div>
                                            <div style="clear: both; min-height: 20px;" />
                                            <div style="display: table-row">
                                                <div style="display: table-cell; width: 200px;">
                                                    <label class="LabelBlack">
                                                        Indirizzo di Ingresso:</label>
                                                </div>
                                                <div style="display: table-cell; width: 200px;">
                                                    <asp:TextBox ID="txtINDIRIZZO_IN" runat="server" Width="150px" Text='<%# Bind("IncomingServer") %>' />
                                                    <asp:RequiredFieldValidator ID="rfvINDIRIZZO_IN" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="txtINDIRIZZO_IN" Display="None" ValidationGroup="btnSalvaServer"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vceINDIRIZZO_IN" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvINDIRIZZO_IN">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                                <div style="display: table-cell; width: 180px;">
                                                    <label class="LabelBlack">
                                                        Indirizzo di Uscita:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:TextBox ID="txtINDIRIZZO_OUT" runat="server" Width="150px" Text='<%# Bind("OutgoingServer") %>' />
                                                    <asp:RequiredFieldValidator ID="rfvINDIRIZZO_OUT" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="txtINDIRIZZO_OUT" Display="None" ValidationGroup="btnSalvaServer"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vceINDIRIZZO_OUT" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvINDIRIZZO_OUT">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        Porta di Ingresso:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:TextBox ID="txtPORTA_IN" runat="server" Width="150px" Text='<%# Bind("PortIncomingServer") %>'
                                                        onkeyup="onlyDigitsInteger(this);" />
                                                    <asp:RequiredFieldValidator ID="rfvPORTA_IN" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="txtPORTA_IN" Display="None" ValidationGroup="btnSalvaServer"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vcePORTA_IN" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvPORTA_IN">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        Porta di Uscita:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:TextBox ID="txtPORTA_OUT" runat="server" Width="150px" Text='<%# Bind("PortOutgoingServer") %>'
                                                        onkeyup="onlyDigitsInteger(this);" />
                                                    <asp:RequiredFieldValidator ID="rfvPORTA_OUT" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="txtPORTA_OUT" Display="None" ValidationGroup="btnSalvaServer"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vcePORTA_OUT" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvPORTA_OUT">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        SSL di Ingresso:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:CheckBox runat="server" ID="cbSSLIn" Checked='<%# Bind("IsIncomeSecureConnection") %>' />
                                                </div>
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        SSL di Uscita:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:CheckBox runat="server" ID="cbSSLOut" Checked='<%# Bind("IsOutgoingSecureConnection") %>' />
                                                </div>
                                            </div>
                                            <div style="display: table-row">
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        Protocollo di Ingresso:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:TextBox ID="txtPROTOCOLLO_IN" runat="server" Width="150px" Text='<%# Bind("IncomingProtocol") %>'
                                                        onkeyup="onlyDigitsInteger(this);" />
                                                    <asp:RequiredFieldValidator ID="rfvPROTOCOLLO_IN" runat="server" ErrorMessage="Campo Obbligatorio"
                                                        ControlToValidate="txtPROTOCOLLO_IN" Display="None" ValidationGroup="btnSalvaServer"
                                                        SetFocusOnError="true">
                                                    </asp:RequiredFieldValidator>
                                                    <cc1:ValidatorCalloutExtender ID="vcePROTOCOLLO_IN" CssClass="CustomValidator" runat="server"
                                                        TargetControlID="rfvPROTOCOLLO_IN">
                                                    </cc1:ValidatorCalloutExtender>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <label class="LabelBlack">
                                                        Autorizzazione di Uscita:</label>
                                                </div>
                                                <div style="display: table-cell;">
                                                    <asp:CheckBox runat="server" ID="cbAuthOut" Checked='<%# Bind("IsOutgoingWithAuthentication") %>' />
                                                </div>
                                            </div>
                                            <div style="clear: both; height: 10px;">
                                            </div>
                                            <div class="buttons-panel" style="margin-top: 4px; margin-right: 8px">
                                                <asp:Button ID="btnSalvaServer" runat="server" Text="Salva" ToolTip="Effettua il Savataggio"
                                                    CssClass="upd" CausesValidation="true" ValidationGroup="btnSalvaServer" CommandName="insert">
                                                </asp:Button>
                                                <asp:Button ID="butAnnullaEmail" runat="server" Text="Annulla" ToolTip="Annulla"
                                                    CssClass="upd" CausesValidation="false" CommandName="annulla"></asp:Button>
                                            </div>
                                        </InsertItemTemplate>
                                    </asp:FormView>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnlElencoUtenti" runat="server" Visible="false">
                        <div class="body-panel">
                            <div style="display: table; width: 100%;">
                                <div style="display: table-row;">
                                    <div style="float: left; display: table-cell;" class="content-panel-gray">
                                        <div class="header-panel-gray">
                                            <div class="header-title">
                                                <div class="header-text-left">
                                                    <label>
                                                        Elenco Utenti - Dipartimento:
                                                    </label>
                                                    <asp:Label runat="server" ID="lblDepartment" />
                                                    <asp:DropDownList ID="ddlListaDipartimenti" runat="server" DataTextField="DEPARTMENT"
                                                        Height="20px" Width="50px" AutoPostBack="true" DataValueField="DEPARTMENT" OnSelectedIndexChanged="ddlListaDipartimenti_itemSelected">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="body-panel" style="height: 400px; min-width: 400px; max-width: 900px">
                                            <asp:ListView ID="lvDipendentiNONAbilitati" runat="server">
                                                <LayoutTemplate>
                                                    <div id="itemContainer" style="display: inline-table;">
                                                        <div class="container">
                                                            <div id="itemPlaceholder" runat="server">
                                                            </div>
                                                        </div>
                                                    </div>
                                                </LayoutTemplate>
                                                <ItemTemplate>
                                                    <div style="display: table-row;">
                                                        <div class="left-content" style="display: table-cell;">
                                                            <asp:CheckBox ID="checkBoxUtenteNONAbilitati" runat="server" />
                                                        </div>
                                                        <div class="right-content" style="display: table-cell;">
                                                            <asp:HiddenField ID="lb_ID_UTENTE" Value='<%#Eval("UserId")%>' runat="server" />
                                                            <asp:Label ID="lb_LDAPID" Text='<%# Eval("UserName")+" ("+Eval("Cognome")+" "+Eval("Nome")+") "%>'
                                                                runat="server"></asp:Label>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:ListView>
                                        </div>
                                    </div>
                                    <div style="display: table-cell; vertical-align: middle; text-align: center;">
                                        <div style="display: inline-table; min-width: 35px; max-width: 50px;">
                                            <div style="display: table-row;">
                                                <div style="display: table-cell;">
                                                    <asp:ImageButton runat="server" ID="btnAbilita" OnClick="btnAbilita_Click" ImageUrl="~/App_Themes/Delta/images/buttons/navigate_right_256.png"
                                                        Width="80%" BorderColor="#99bbe8" BorderWidth="1" ToolTip="Abilita Utente" />
                                                </div>
                                            </div>
                                            <div style="display: table-row;">
                                                <div style="display: table-cell;">
                                                    <asp:ImageButton runat="server" ID="btnDisabilita" OnClick="btnDisabilita_Click"
                                                        ImageUrl="~/App_Themes/Delta/images/buttons/navigate_left_256.png" Width="80%"
                                                        BorderColor="#99bbe8" BorderWidth="1" ToolTip="Disabilita Utente" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div style="display: table-cell; float: right;" class="content-panel-gray">
                                        <div class="header-panel-gray">
                                            <div class="header-title">
                                                <div class="header-text-left">
                                                    <label>
                                                        Utenti Abilitati</label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="body-panel" style="overflow: auto; height: 400px; min-width: 450px;max-width:1200px">
                                            <asp:ListView ID="lvDipendentiAbilitati" runat="server" OnItemDataBound="lvDipendentiAbilitati_ItemDataBound">
                                                <LayoutTemplate>
                                                    <div id="itemContainer" style="display: inline-table;">
                                                        <div class="container">
                                                            <div id="itemPlaceholder" runat="server">
                                                            </div>
                                                        </div>
                                                    </div>
                                                </LayoutTemplate>
                                                <ItemTemplate>
                                                    <div style="display: table-row;">
                                                        <div class="left-content" style="display: table-cell;">
                                                            <asp:CheckBox ID="checkBoxUtenteAbilitati" runat="server" />
                                                        </div>
                                                        <div class="right-content" style="display: table-cell;">
                                                            <asp:HiddenField ID="lb_ID_UTENTE" Value='<%#Eval("UserId")%>' runat="server" />
                                                            <asp:HiddenField ID="lb_ROLE"  runat="server" />
                                                            <asp:Label ID="lb_LDAPID" Text='<%# Eval("UserName")+" ("+Eval("Cognome")+" "+Eval("Nome")+") - Dip.: "+Eval("Department")%>'
                                                                runat="server"></asp:Label>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:ListView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                     <asp:Panel ID="pnlAdmin" runat="server" Visible="false">
                        <div class="body-panel">
                            <div style="display: table; width: 100%;">
                                <div style="display: table-row;">
                                    <div style="float: left; display: table-cell;" class="content-panel-gray">
                                        <div class="header-panel-gray">
                                            <div class="header-title">
                                                <div class="header-text-left">
                                                    <label>
                                                        Elenco Utenti Email:
                                                    </label>
                                                    <asp:Label runat="server" ID="Label1" />                                                    
                                                </div>
                                            </div>
                                        </div>
                                        <div class="body-panel">
                                            <asp:ListView ID="lvUtenti" runat="server">
                                                <LayoutTemplate>
                                                    <div id="itemContainer" style="display: inline-table;">
                                                        <div class="container">
                                                            <div id="itemPlaceholder" runat="server">
                                                            </div>
                                                        </div>
                                                    </div>
                                                </LayoutTemplate>
                                                <ItemTemplate>
                                                    <div style="display: table-row;">
                                                        <div class="left-content" style="display: table-cell;">
                                                            <asp:CheckBox ID="checkBoxUtenteAdminNONAbilitati" runat="server" />
                                                        </div>
                                                        <div class="right-content" style="display: table-cell;">
                                                            <asp:HiddenField ID="lb_ID_UTENTE_ADMIN" Value='<%#Eval("UserId")%>' runat="server" />
                                                            <asp:Label ID="lb_LDAPID_ADMIN" Text='<%# Eval("UserName")+" ("+Eval("Cognome")+" "+Eval("Nome")+") "%>'
                                                                runat="server"></asp:Label>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:ListView>
                                        </div>
                                    </div>
                                    <div style="display: table-cell; vertical-align: middle; text-align: center;">
                                        <div style="display: inline-table; min-width: 35px; max-width: 50px;">
                                            <div style="display: table-row;">
                                                <div style="display: table-cell;">
                                                    <asp:ImageButton runat="server" ID="btnAdminAbil" OnClick="btnAbilitaAdmin_Click" ImageUrl="~/App_Themes/Delta/images/buttons/navigate_right_256.png"
                                                        Width="80%" BorderColor="#99bbe8" BorderWidth="1" ToolTip="Abilita Utente all'Amministrazione" />
                                                </div>
                                            </div>
                                            <div style="display: table-row;">
                                                <div style="display: table-cell;">
                                                    <asp:ImageButton runat="server" ID="btnAdminDis" OnClick="btnDisabilitaAdmin_Click"
                                                        ImageUrl="~/App_Themes/Delta/images/buttons/navigate_left_256.png" Width="80%"
                                                        BorderColor="#99bbe8" BorderWidth="1" ToolTip="Disabilita Utente dall'Amministrazione" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div style="display: table-cell; float: right;" class="content-panel-gray">
                                        <div class="header-panel-gray">
                                            <div class="header-title">
                                                <div class="header-text-left">
                                                    <label>
                                                        Utenti Amministratori</label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="body-panel" id="utentiAdmin">
                                            <asp:ListView ID="lvUtentiAdmin" runat="server" OnItemDataBound="lvUtentiAdmin_ItemDataBound">
                                                <LayoutTemplate>
                                                    <div id="itemContainer" style="display: inline-table;">
                                                        <div class="container">
                                                            <div id="itemPlaceholder" runat="server">
                                                            </div>
                                                        </div>
                                                    </div>
                                                </LayoutTemplate>
                                                <ItemTemplate>
                                                    <div style="display: table-row;">
                                                        <div class="left-content" style="display: table-cell;">
                                                            <asp:CheckBox ID="checkBoxUtentiAdmin" runat="server" />
                                                        </div>
                                                        <div class="right-content" style="display: table-cell;">
                                                            <asp:HiddenField ID="lb_ID_UTENTE_ADMIN" Value='<%#Eval("UserId")%>' runat="server" />
                                                            <asp:HiddenField ID="lb_ROLE_ADMIN"  runat="server" />
                                                            <asp:Label ID="lb_LDAPID_ADMIN" Text='<%# Eval("UserName")+" ("+Eval("Cognome")+" "+Eval("Nome")+") - Dip.: "+Eval("Department")%>'
                                                                runat="server"></asp:Label>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:ListView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                    
                      <asp:Panel ID="pnlGestioneFolders" runat="server" Visible="false">
                        <div class="body-panel">
                            <div style="display: table; width: 100%;">
                                <div style="display: table-row;">
                                    <div style="float: left; display: table-cell;" class="content-panel-gray">
                                        <div class="header-panel-gray">
                                            <div class="header-title">
                                                <div class="header-text-left">
                                                    <label>
                                                        Cartelle Non Abilitate
                                                    </label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="body-panel" id="cartelleNonAbilitate">
                                            <asp:ListView ID="lvCartelleNonAbilitate" runat="server">
                                                <LayoutTemplate>
                                                    <div id="itemContainer" style="display: inline-table;">
                                                        <div class="container">
                                                            <div id="itemPlaceholder" runat="server">
                                                            </div>
                                                        </div>
                                                    </div>
                                                </LayoutTemplate>
                                                <ItemTemplate>
                                                    <div style="display: table-row;">
                                                        <div class="left-content" style="display: table-cell;">
                                                            <asp:CheckBox ID="checkBoxCartelleNonAbilitate" runat="server" />
                                                        </div>
                                                        <div class="right-content" style="display: table-cell;">
                                                            <asp:HiddenField ID="lb_NOME_CARTELLA" Value='<%#Eval("IdNome")+";"+Eval("IdSender")+";"+Eval("System")%>' runat="server" />
                                                            <asp:Label ID="lb_LDAPID" Text='<%# Eval("Nome")+" ("+Eval("System")+") "%>'
                                                                runat="server"></asp:Label>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:ListView>
                                        </div>
                                    </div>
                                    <div style="display: table-cell; vertical-align: middle; text-align: center;">
                                        <div style="display: inline-table; min-width: 35px; max-width: 50px;">
                                            <div style="display: table-row;">
                                                <div style="display: table-cell;">
                                                    <asp:ImageButton runat="server" ID="ImageButton3" OnClick="btnAbilitaFolder_Click" ImageUrl="~/App_Themes/Delta/images/buttons/navigate_right_256.png"
                                                        Width="80%" BorderColor="#99bbe8" BorderWidth="1" ToolTip="Abilita Cartella" />
                                                </div>
                                            </div>
                                            <div style="display: table-row;">
                                                <div style="display: table-cell;">
                                                    <asp:ImageButton runat="server" ID="ImageButton4" OnClick="btnDisabilitaFolder_Click" ImageUrl="~/App_Themes/Delta/images/buttons/navigate_left_256.png"
                                                        Width="80%" BorderColor="#99bbe8" BorderWidth="1" ToolTip="Disabilita Cartella" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div style="display: table-cell; float: right;" class="content-panel-gray">
                                        <div class="header-panel-gray">
                                            <div class="header-title">
                                                <div class="header-text-left">
                                                    <label>
                                                        Cartelle Abilitate</label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="body-panel" id="cartelleAbilitate">
                                            <asp:ListView ID="lvCartelleAbilitate" runat="server" OnItemDataBound="lvCartelleAbilitate_ItemDataBound">
                                                <LayoutTemplate>
                                                    <div id="itemContainer" style="display: inline-table;">
                                                        <div class="container">
                                                            <div id="itemPlaceholder" runat="server">
                                                            </div>
                                                        </div>
                                                    </div>
                                                </LayoutTemplate>
                                                <ItemTemplate>
                                                    <div style="display: table-row;">
                                                        <div class="left-content" style="display: table-cell;">
                                                            <asp:CheckBox ID="checkBoxCartelleAbilitate" runat="server" Enabled = "true"  />
                                                        </div>
                                                        <div class="right-content" style="display: table-cell;">
                                                            <asp:HiddenField ID="lb_NOME_CARTELLA_ABILITATA" Value='<%#Eval("IdNome")+";"+Eval("IdSender")+";"+Eval("System")%>' runat="server" />
                                                            <asp:Label ID="lb_FOLDVALUE" Text='<%# Eval("Nome")+" ("+Eval("System")+") "%>'
                                                                runat="server"></asp:Label>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:ListView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <script type="text/javascript">
        /* Validate Integer */
        function onlyDigitsInteger(fld) {

            fld.value = fld.value.replace(/[^0-9]/ig, "");
        }
    </script>

</asp:Content>


