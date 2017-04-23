<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Mail.Master" Theme="Delta" AutoEventWireup="true" CodeBehind="RegisterFolder.aspx.cs" Inherits="GestionePEC.pages.Folders.RegisterFolder" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div id="pnlMain" class="content-panel-borderless">
        <div class="header-panel-blue">
            <div class="header-title">
                <div class="header-text-left">
                    <label>
                        REGISTRAZIONE CARTELLA</label>
                </div>
            </div>
        </div>
        <div id="container" class="control-main">
            <div id="divparole" class="control-header-blue">
                <div class="header-title">
                    <div class="header-text-left">
                        <label>Dati Cartella</label>
                    </div>
                </div>
            </div>
            <div class="control-body-gray">
                <div class="control-body-gray">
                    <div class="tabella" style="margin-top: 5px; text-align: left;">
                        <div class="grid-panel">
                            <div style="display: table-row">
                                <div style="display: table-cell;">
                                    <label class="LabelBlack">
                                        *  Nome:</label>
                                </div>
                                <div style="display: table-cell;">
                                    <asp:TextBox runat="server" SkinID="tbLong" MaxLength="20" Text="" ID="NomeFolder"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="buttons-panel" style="margin-top: 4px; margin-right: 8px">
                <asp:Button ID="btnSalva" runat="server" Text="Salva" ToolTip="Effettua il Savataggio"
                    CssClass="upd" CausesValidation="true" ValidationGroup="btnSalvaServer" OnClick="btnSalva_Click"></asp:Button>
            </div>
        </div>
    </div>
</asp:Content>
