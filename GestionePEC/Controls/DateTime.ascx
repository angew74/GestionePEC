<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DateTime.ascx.cs" Inherits="GestionePEC.Controls.DateTime" %>
<%@ Register Assembly="AjaxControlToolkit"
    Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Panel ID="Contenitore" runat="server">
    <asp:Label runat="server" ID="lblTitle" CssClass="LabelBlackMiddle" Text="" Visible="false"></asp:Label>
    <asp:TextBox ID="tdDataG" SkinID="tb2Char" MaxLength="2" runat="server"></asp:TextBox>
    <label class="LabelBlackMiddle">/</label>
    <asp:TextBox ID="tdDataM" SkinID="tb2Char" MaxLength="2" runat="server" ></asp:TextBox>
    <label class="LabelBlackMiddle">/</label>
    <asp:TextBox ID="tdDataY" SkinID="tbDateTimeMiddle" MaxLength="4"  runat="server" ></asp:TextBox>
    <asp:RequiredFieldValidator ID="rfvAnno" runat="server" ErrorMessage="L'anno e' obbligatorio"
        ControlToValidate="tdDataY" Display="None" SetFocusOnError="True"></asp:RequiredFieldValidator>
    <cc1:ValidatorCalloutExtender CssClass="CustomValidator" ID="vceAnno" runat="server"
        TargetControlID="rfvAnno" Enabled="True">
    </cc1:ValidatorCalloutExtender>
    <asp:RegularExpressionValidator ID="revAnno" runat="server" ErrorMessage="Specificare l'anno di 4 cifre"
        ControlToValidate="tdDataY" ValidationExpression="[0-9]{4}" Display="None" SetFocusOnError="True">
    </asp:RegularExpressionValidator>
    <cc1:ValidatorCalloutExtender CssClass="CustomValidator" ID="cvREVAnno" runat="server"
        TargetControlID="revAnno" Enabled="True">
    </cc1:ValidatorCalloutExtender>
    </asp:Panel>