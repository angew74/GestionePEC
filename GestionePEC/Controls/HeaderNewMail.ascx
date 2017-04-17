<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeaderNewMail.ascx.cs" Inherits="GestionePEC.Controls.HeaderNewMail" %>
<div class="control-main" id="PanelGrid">
    <asp:UpdatePanel runat="server" ID="pnlUpadteTitolo">
        <ContentTemplate>
            <table class="NewTable">
                <tr>
                    <td align="right">
                        <label class="LabelBlack">
                            Destinatario:
                        </label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" CssClass="Fixed300" Font-Bold="true" Columns="60" ID="TOMail" />
                         </td>
                </tr>
                <tr>
                    <td align="right">
                        <label class="LabelBlack">
                            Conoscenza:
                        </label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" CssClass="Fixed300" Font-Bold="true" Columns="60" ID="CCMail" />                        
                    </td>
                </tr>
                <tr id="trBCC" runat="server" visible="true">
                    <td align="right">
                        <label class="LabelBlack">
                            Destinatario nascosto:
                        </label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" CssClass="Fixed300" Font-Bold="true" Columns="60" ID="BCCMail" />                       
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <label class="LabelBlack">
                            Oggetto:
                        </label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" CssClass="Fixed300" Font-Bold="true" Columns="60" ID="ObjectMail" />
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <label class="LabelBlack">
                            Titolo:
                        </label>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" OnSelectedIndexChanged="OnChangeIndex_ddlTitolo"
                            AutoPostBack="true" ID="ddlTitolo">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredddlTitolo" runat="server" ControlToValidate="ddlTitolo"
                            ErrorMessage="Campo obbligatorio!" InitialValue="-- Selezionare un titolo --"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <label class="LabelBlack">
                            Sottotitolo:
                        </label>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlSottotitolo">
                            <asp:ListItem Value="" Text="Selezionare un valore" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfv1" runat="server" ValidationGroup="email" ControlToValidate="ddlSottotitolo"
                            Display="Dynamic" ErrorMessage="Selezionare un sottotitolo" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
