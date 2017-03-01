<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewMail.ascx.cs" ValidateRequestMode="Disabled" Inherits="GestionePEC.Controls.NewMail" %>
<%@ Register Src="~/Controls/MailBoxLogin.ascx" TagName="MailBoxLogin" TagPrefix="mail" %>
<%@ Register Src="~/Controls/MailComposer.ascx" TagName="MailComposer" TagPrefix="mail" %>
<div id="pnlMain" class="content-panel-borderless">
    <div class="header-panel-blue">
        <div class="header-title">
            <div class="header-text-left">
                <label>
                    Mail Box</label>
            </div>
        </div>
    </div>
    <mail:MailBoxLogin ID="Login" runat="server" OnChangeStatus="Login_OnChangeStatus">
    </mail:MailBoxLogin>
</div>
<mail:MailComposer ID="MailComposer1" runat="server" Visible="false" EnableNewAttachments="true"
    OnMailSent="MailComposer1_OnMailSent"
    OnAccountInvalid="MailComposer1_AccountInvalid" OnMessageInvalid="MailComposer1_MessageInvalid" />
