<%@ Page Title="Mappatura" Language="C#" MasterPageFile="~/Master/Mail.Master" Theme="Delta" AutoEventWireup="true" CodeBehind="AccessoMappatura.aspx.cs" Inherits="GestionePEC.pages.Rubrica.AccessoMappatura" %>
<%@ Import Namespace="SendMail.Model.RubricaMapping" %>
<%@ Register Src="~/Controls/Paging.ascx" TagName="UCPaging" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div id="Div1">
        <div class="header-panel-blue">
            <div class="header-title">
                <div class="header-text-left">
                    <label>
                        Accesso Mappatura</label>
                </div>
            </div>
        </div>
        <asp:Panel ID="panParoleChiavi" runat="server" CssClass="control-main">
            <div id="divparole" class="control-header-blue">
                <div class="header-title">
                    <div class="header-text-left">
                        <label>Parole Chiavi di Ricerca</label>
                    </div>
                    <%--<div class="header-text-right">--%>
                        <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" SkinID="ToolboxButton"
                            CssClass="bSearchWhite" Style="float: right; margin-top: 2px;" UseSubmitBehavior="false"
                            CausesValidation="false" />
                        <asp:Button ID="btnIns" runat="server" OnClick="btnIns_Click" SkinID="ToolboxButton"
                            CssClass="bAdd" Style="float: right; margin-top: 2px;" UseSubmitBehavior="false"
                            CausesValidation="false" />
                    <%--</div>--%>
                </div>
            </div>
          
            <div class="control-body-gray">

                <table align="center" border="0">
                    <tr>
                        <td>
                            <asp:Label ID="bkcode" runat="server" CssClass="LabelBlack">Codice Backend</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox align="left" ID="codiceback" runat="server" Width="200px" CssClass="formtext"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="codback" runat="server" ErrorMessage="Immettere codice backend"
                                ControlToValidate="codiceback" Display="None" SetFocusOnError="True"></asp:RequiredFieldValidator>
                        </td>
                        <td>
                            <asp:Label ID="bkdescr" runat="server" CssClass="LabelBlack">Descrizione Backend</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox align="right" ID="descrizioneback" runat="server" Width="200px" CssClass="formtext"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="coddescr" runat="server" ErrorMessage="Immettere descrizione backend"
                                ControlToValidate="descrizioneback" Display="None" SetFocusOnError="True"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>
            </div>
          
        </asp:Panel>
        <asp:Panel ID="pnlGrid" runat="server" CssClass="control-main">
            <div id="divRicerca" class="control-header-blue">
                <div class="header-title">
                    <div class="header-text-left">
                        <label>
                            Ricerca</label>
                    </div>
                </div>
            </div>
            <div class="control-body-gray">
                <asp:GridView ID="griBackend" runat="server" AutoGenerateColumns="false" EnableViewState="true"
                    AllowPaging="true" RowStyle-Wrap="true" OnRowCommand="BkAccesso_RowCommand">
                    <Columns>
                        <asp:TemplateField HeaderText="Codice Backend">
                            <ItemTemplate>
                                <asp:Label ID="LblCodice" runat="server" Text='<%#Eval("Codice") %>'></asp:Label>
                                <asp:HiddenField ID="IdRef" runat="server" Value='<%# Eval("Id") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--<asp:BoundField DataField="Codice" HeaderText="Codice Backend" />--%>
                        <asp:BoundField DataField="Descrizione" HeaderText="Descrizione" />
                        <asp:BoundField DataField="Categoria" HeaderText="Categoria" />
                        <asp:BoundField DataField="DescrizionePlus" HeaderText="DescrizionePlus" />
                        <asp:ButtonField ButtonType="Image" ImageUrl="~/App_Themes/Delta/images/buttons/select.png" />
                    </Columns>
                    <PagerTemplate>
                        <uc2:UCPaging ID="ucAccessoPaging" runat="server" OnPagerIndexChanged="OnAccessoPagerIndexChanged"
                            OnInit="ucAccessoPaging_Init" />
                    </PagerTemplate>
                    <EmptyDataTemplate>
                        Nessun contatto presente
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </asp:Panel>
        <asp:Panel ID="PanelInsDettRic" runat="server" CssClass="control-main">
            <div id="divInsdett" class="control-header-blue">
                <div class="header-title">
                    <div class="header-text-left">
                        <label>
                            Inserimento o Dettaglio della Ricerca</label>
                    </div>
                </div>
            </div>
            <div class="control-body-gray">
                <div class="update-panel">
                    <asp:ObjectDataSource runat="server" ID="odsBackend" EnableCaching="false" DataObjectTypeName="SendMail.Model.BackEndRefCode"
                        TypeName="SendMail.Model.BackEndRefCode" SelectMethod="select" UpdateMethod="save"
                        OnObjectCreating="odsBackend_ObjectCreating" OnUpdating="odsBackend_Updating"
                        OnDeleting="odsBackend_Deleting" />
                    <asp:FormView ID="AccessoView" runat="server" DefaultMode="Edit" Font-Size="Small"
                        OnItemCommand="AccessoView_ItemCommand" OnItemUpdating="AccessoView_ItemUpdating"
                        OnItemDeleting="AccessoView_ItemDeleting" OnItemInserting="AccessoView_ItemInserting">
                        <ItemTemplate>
                            <asp:HiddenField runat="server" ID="hfIdAccesso" Value='<%# Eval("Id") %>' />
                            <div>
                                <span class="LabelBlack Fixed120">Id:</span>
                                <label class="LabelBlackBold">
                                    <%# CheckString(Eval("Id")) %></label>
                            </div>
                            <div>
                                <span class="LabelBlack Fixed120">Codice:</span>
                                <label class="LabelBlackBold">
                                    <%# CheckString(Eval("Codice")) %></label>
                                <%--<label class="LabelBlackBold"><%# Eval("Codice") %></label>--%>
                            </div>
                            <div>
                                <span class="LabelBlack Fixed120">Descrizione:</span>
                                <label class="LabelBlackBold">
                                    <%# CheckString(Eval("Descrizione")) %></label>
                            </div>
                            <div>
                                <span class="LabelBlack Fixed120">Categoria:</span>
                                <label class="LabelBlackBold">
                                    <%# CheckString(Eval("Categoria")) %></label>
                            </div>
                            <div>
                                <span class="LabelBlack Fixed120">DescrizionePlus:</span>
                                <label class="LabelBlackBold">
                                    <%# CheckString(Eval("DescrizionePlus")) %></label>
                            </div>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:HiddenField runat="server" ID="hfIdAccesso" Value='<%# Bind("Id") %>' />
                            <div>
                                <span class="LabelBlack Fixed120">Id:</span>
                                <asp:Label ID="LblId" runat="server" Text='<%#Bind("Id") %>'></asp:Label>
                                <%--<asp:TextBox runat="server" Text='<%#Bind("Id") %>' ID="TextId" CssClass="FixedBorder300"
                                        Font-Bold="true" Columns="60" />--%>
                                <%--<label class="LabelBlackBold"><%# Bind("Id") %></label>--%>
                                <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="TextCodice"
                                            runat="server" ValidationExpression="^[0-9]{11}$" ErrorMessage="Parita Iva" Display="Dynamic"
                                            ValidationGroup="vgUpdAccesso" />--%>
                            </div>
                            <div>
                                <span class="LabelBlack Fixed120">Codice:</span>
                                <%--<asp:Label ID="LblCode" runat="server" Text='<%#Bind("Codice") %>'></asp:Label>--%>
                                <asp:TextBox runat="server" Text='<%#Bind("Codice") %>' ID="TextCode" CssClass="FixedBorder300"
                                    Font-Bold="true" Columns="60" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator6" ControlToValidate="TextCode"
                                    runat="server" ValidationExpression="^[A-Z a-z 0-9]*" ErrorMessage="CODICE non corretto"
                                    Display="Dynamic" ValidationGroup="vgUpdAccesso" />
                            </div>
                            <div>
                                <span class="LabelBlack Fixed120">Descrizione:</span>
                                <asp:TextBox runat="server" Text='<%#Bind("Descrizione") %>' ID="TextDescrizione"
                                    CssClass="FixedBorder300" Font-Bold="true" Columns="60" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator7" ControlToValidate="TextDescrizione"
                                    runat="server" ValidationExpression="^[A-Za-zÀÃÄÇÈÉËÌÏÒÖÙÜàãäçèéëìïòöùü'\-/ - ]*$"
                                    ErrorMessage="DESCRIZIONE non corretta" Display="Dynamic" ValidationGroup="vgUpdAccesso" />
                            </div>
                            <div>
                                <span class="LabelBlack Fixed120">Categoria:</span>
                                <asp:TextBox runat="server" Text='<%#Bind("Categoria") %>' ID="TextCategoria" CssClass="FixedBorder300"
                                    Font-Bold="true" Columns="60" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator8" ControlToValidate="TextCategoria"
                                    runat="server" ValidationExpression="^[A-Za-zÀÃÄÇÈÉËÌÏÒÖÙÜàãäçèéëìïòöùü'\- ]*$"
                                    ErrorMessage="CATEGORIA non corretta" Display="Dynamic" ValidationGroup="vgUpdAccesso" />
                            </div>
                            <div>
                                <span class="LabelBlack Fixed120">DescrizionePlus:</span>
                                <asp:TextBox runat="server" Text='<%#Bind("DescrizionePlus") %>' ID="TextDescrizionePlus"
                                    CssClass="FixedBorder300" Font-Bold="true" Columns="60" MaxLength="10" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator9" ControlToValidate="TextDescrizionePlus"
                                    runat="server" ValidationExpression="^[A-Za-zÀÃÄÇÈÉËÌÏÒÖÙÜàãäçèéëìïòöùü'(\) ]*$"
                                    ErrorMessage="DESCRIZIONEPLUS non corretta" Display="Dynamic" ValidationGroup="vgUpdAccesso" />
                            </div>
                            <div style="text-align: left; margin: 10px 0 5px 435px;">
                                <asp:Button ID="btnUpdate" runat="server" ToolTip="Aggiorna i dati inseriti" CommandName="Update"
                                    Text="Aggiorna" ValidationGroup="vgUpdAccesso" />
                                <asp:Button ID="btnDelete" runat="server" ToolTip="Cancella i dati inseriti" CommandName="Delete"
                                    Text="Cancella" ValidationGroup="vgDelAccesso" />
                            </div>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:HiddenField runat="server" ID="hfIdAccesso" Value='<%# Bind("Id") %>' />
                            <%--<div>
                                        <span class="LabelBlack Fixed120">Id:</span>
                                        <asp:TextBox runat="server" Text='<%#Bind("Id") %>' ID="TextId" CssClass="FixedBorder300"
                                        Font-Bold="true" Columns="60" />
                                    </div>--%>
                            <div>
                                <span class="LabelBlack Fixed120">Codice:</span>
                                <asp:TextBox runat="server" Text='<%#Bind("Codice") %>' ID="TextCode" CssClass="FixedBorder300"
                                    Font-Bold="true" Columns="60" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator6" ControlToValidate="TextCode"
                                    runat="server" ValidationExpression="^[A-Z a-z 0-9]*" ErrorMessage="CODICE non corretto"
                                    Display="Dynamic" ValidationGroup="vgInsAccesso" />
                            </div>
                            <div>
                                <span class="LabelBlack Fixed120">Descrizione:</span>
                                <asp:TextBox runat="server" Text='<%#Bind("Descrizione") %>' ID="TextDescrizione"
                                    CssClass="FixedBorder300" Font-Bold="true" Columns="60" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator7" ControlToValidate="TextDescrizione"
                                    runat="server" ValidationExpression="^[A-Za-zÀÃÄÇÈÉËÌÏÒÖÙÜàãäçèéëìïòöùü'\-/ - ]*$"
                                    ErrorMessage="DESCRIZIONE non corretta" Display="Dynamic" ValidationGroup="vgInsAccesso" />
                            </div>
                            <div>
                                <span class="LabelBlack Fixed120">Categoria:</span>
                                <asp:TextBox runat="server" Text='<%#Bind("Categoria") %>' ID="TextCategoria" CssClass="FixedBorder300"
                                    Font-Bold="true" Columns="60" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator8" ControlToValidate="TextCategoria"
                                    runat="server" ValidationExpression="^[A-Za-zÀÃÄÇÈÉËÌÏÒÖÙÜàãäçèéëìïòöùü'\- ]*$"
                                    ErrorMessage="CATEGORIA non corretta" Display="Dynamic" ValidationGroup="vgInsAccesso" />
                            </div>
                            <div>
                                <span class="LabelBlack Fixed120">DescrizionePlus:</span>
                                <asp:TextBox runat="server" Text='<%#Bind("DescrizionePlus") %>' ID="TextDescrizionePlus"
                                  MaxLength="10"  CssClass="FixedBorder300" Font-Bold="true" Columns="60" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator9" ControlToValidate="TextDescrizionePlus"
                                    runat="server" ValidationExpression="^[A-Za-zÀÃÄÇÈÉËÌÏÒÖÙÜàãäçèéëìïòöùü'(\) ]*$"
                                    ErrorMessage="DESCRIZIONEPLUS non corretta" Display="Dynamic" ValidationGroup="vgInsAccesso" />
                            </div>
                            <div style="text-align: left; margin: 10px 0 5px 435px;">
                                <asp:Button ID="btnInsert" runat="server" ToolTip="Inserisci i dati inseriti" CommandName="Insert"
                                    Text="Inserisci" ValidationGroup="vgInsAccesso" />
                            </div>
                        </InsertItemTemplate>
                    </asp:FormView>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
