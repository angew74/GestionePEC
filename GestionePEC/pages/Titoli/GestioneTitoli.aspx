<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Mail.Master" AutoEventWireup="true" Theme="Delta"  CodeBehind="GestioneTitoli.aspx.cs" Inherits="GestionePEC.pages.Titoli.GestioneTitoli" %>
    <asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div id="Div1">
        <div class="header-panel-blue">
            <div class="header-title">
                <div class="header-text-left">
                    <label>
                        Gestione Titoli</label>
                </div>
            </div>
        </div>
        <asp:Panel ID="panParoleChiavi" runat="server" CssClass="control-main">
            <div id="divHeaderTitolo" class="control-header-blue">
                <div class="header-title">
                    <div class="header-text-left">
                        <label>Creazione Titoli</label>
                    </div>
                </div>
            </div>
            <div class="control-body-gray">
                <div class="tabella" style="margin-top: 5px; text-align: left;">
                    <div class="grid-panel">
                        <div style="display: table-row">
                            <div style="display: table-cell;">
                                <label class="LabelBlack">
                                  *  Nome:</label>
                            </div>
                            <div style="display: table-cell;">
                                <asp:TextBox runat="server" SkinID="tbLong" MaxLength="20" Text="" ID="TitoloNome"></asp:TextBox>
                            </div>
                        </div>
                        <div style="display: table-row">
                            <div style="display: table-cell;">
                                <label class="LabelBlack">
                                   * Codice Applicazione:</label>
                            </div>
                            <div style="display: table-cell;">
                                <asp:TextBox runat="server" SkinID="tb4Char" MaxLength="3" Text="" ID="CodiceApplicazione"></asp:TextBox>
                            </div>
                        </div>
                        <div style="display: table-row">
                            <div style="display: table-cell;">
                                <label class="LabelBlack">
                                    Codice Protocollo:</label>
                            </div>
                            <div style="display: table-cell;">
                                <asp:TextBox runat="server" SkinID="tb4Char" MaxLength="3" Text="" ID="CodiceProtocollo"></asp:TextBox>
                            </div>
                        </div>
                         <div style="display: table-row">
                            <div style="display: table-cell;">
                                <label class="LabelBlack">
                                    Note:</label>
                            </div>
                            <div style="display: table-cell;">
                                <asp:TextBox runat="server" SkinID="tbLong" MaxLength="120" Text="" ID="Note"></asp:TextBox>
                            </div>
                        </div>
                         <div style="display: table-row">
                            <div style="display: table-cell;">
                                <label class="LabelBlack">
                                    Attivo:</label>
                            </div>
                            <div style="display: table-cell;">
                                <asp:CheckBox runat="server" Text="Attivo" Checked="True" ID="TitoloIsActive">                                    
                                </asp:CheckBox>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
       <div class="buttons-panel" style="margin-top: 4px; margin-right: 8px">
                                                <asp:Button ID="btnSalva" runat="server" Text="Salva" ToolTip="Effettua il Savataggio"
                                                    CssClass="upd" CausesValidation="true" ValidationGroup="btnSalvaServer" OnClick="btnSalva_Click">
                                                </asp:Button>
           </div>
             </asp:Panel>
        <asp:Panel ID="Panel1" runat="server" CssClass="control-main">
            <div id="divHeaderSottoTitoli" class="control-header-blue">
                <div class="header-title">
                    <div class="header-text-left">
                        <label>Creazione Sottotitoli</label>
                    </div>
                </div>
            </div>
            <div class="control-body-gray">                
                        <div class="tabella" style="margin-top: 5px; text-align: left;">
                    <div class="grid-panel">
                         <div style="display: table-row">
                            <div style="display: table-cell;">
                                <label class="LabelBlack">
                                 *   Titolo di riferimento:</label>
                            </div>
                            <div style="display: table-cell;">
                        <asp:DropDownList runat="server"
                     ID="ddlTitolo">
                            </asp:DropDownList>
                                </div>
                             </div>
                        <div style="display: table-row">
                            <div style="display: table-cell;">
                                <label class="LabelBlack">
                                *    Nome:</label>
                            </div>
                            <div style="display: table-cell;">
                                <asp:TextBox runat="server" SkinID="tbLong" MaxLength="20"  Text="" ID="NomeSottotitolo"></asp:TextBox>
                            </div>
                        </div>
                        <div style="display: table-row">
                            <div style="display: table-cell;">
                                <label class="LabelBlack">
                                 *   Codice Comunicazione:</label>
                            </div>
                            <div style="display: table-cell;">
                                <asp:TextBox runat="server" SkinID="tb10Char" MaxLength="9"  Text="" ID="SottoTitoloCodiceComunicazione"></asp:TextBox>
                            </div>
                        </div>
                        <div style="display: table-row">
                            <div style="display: table-cell;">
                                <label class="LabelBlack">
                                    Codice Protocollo:</label>
                            </div>
                            <div style="display: table-cell;">
                                <asp:TextBox runat="server" Text="" SkinID="tb10Char" MaxLength="9"  ID="SottoTitoloCodiceProtocollo"></asp:TextBox>
                            </div>
                        </div>
                         <div style="display: table-row">
                            <div style="display: table-cell;">
                                <label class="LabelBlack">
                                    Note:</label>
                            </div>
                            <div style="display: table-cell;">
                                <asp:TextBox runat="server" Text="" SkinID="tbLong" MaxLength="120"  ID="SottoTitoloNote"></asp:TextBox>
                            </div>
                        </div>
                         <div style="display: table-row">
                            <div style="display: table-cell;">
                                <label class="LabelBlack">
                                    Attivo:</label>
                            </div>
                            <div style="display: table-cell;">
                                <asp:CheckBox runat="server" Checked="true" Text="Attivo" ID="CheckSottoTitoloAttivo">                                    
                                </asp:CheckBox>
                            </div>
                        </div>
                         <div style="display: table-row">
                            <div style="display: table-cell;">
                                <label class="LabelBlack">
                                    Gestione Protocollo:</label>
                            </div>
                            <div style="display: table-cell;">
                                <asp:CheckBox runat="server" Checked="true" Text="Attivo" ID="CheckProtocolloAttivo">                                    
                                </asp:CheckBox>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
             <div class="buttons-panel" style="margin-top: 4px; margin-right: 8px">
                                                <asp:Button ID="buttonSalvaSottoTitolo" runat="server" Text="Salva" ToolTip="Effettua il Savataggio"
                                                    CssClass="upd" CausesValidation="true" ValidationGroup="btnSalvaServer" OnClick="buttonSalvaSottoTitolo_Click">
                                                </asp:Button>
           </div>
        </asp:Panel>
    </div>
</asp:Content>


