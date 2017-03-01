<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailModal.ascx.cs" Inherits="GestionePEC.Controls.MailModal" %>
<%@ Register TagPrefix="cc1" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="~/Controls/NewMail.ascx" TagName="SendMail" TagPrefix="uc1" %>

<cc1:ModalPopupExtender ID="PopUpExt" runat="server"
    TargetControlID="btnPopUpTarget"
    PopupControlID="divPopUp"
    CancelControlID="btnPopUpTarget"
    BackgroundCssClass="modalBackground">
</cc1:ModalPopupExtender>
<div id="divPopUp" runat="server" class="modalPopup" style="display: none; width: 900px">
    <asp:UpdatePanel runat="server" ID="upnlMailPopUp" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            <uc1:SendMail ID="SendMailMessage" runat="server" OnMailSent="SendMailMessage_OnMailSent"
                OnMessageInvalid="SendMailMessage_MessageInvalid" OnAccountInvalid="SendMailMessage_AccountInvalid" />
            <div style="text-align: right; margin: 2px;">
                <asp:Button ID="btnPopUpCancel" runat="server" Text="Chiudi" OnClick="btnPopUpCancel_Click" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
<asp:Button ID="btnPopUpTarget" runat="server" Text="" Style="display: none;" />
