<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Mail.Master" Theme="Delta" AutoEventWireup="true" ValidateRequest="false"  CodeBehind="MailBoxInBox.aspx.cs" Inherits="GestionePEC.pages.MailClient.MailBoxInBox" %>
<%@ Register Src="~/Controls/MailBoxLogin.ascx" TagName="MailBoxLogin" TagPrefix="mail" %>
<%@ Register Src="~/Controls/Inbox.ascx" TagName="Inbox" TagPrefix="mail" %>
<%@ Register Src="~/Controls/MailViewer.ascx" TagName="MailViewer" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/MailBoxNavigator.ascx" TagName="MailNav" TagPrefix="mail" %>
<%--<%@ Register Src="~/pages/MailClient/MailTreeViewer.ascx" TagName="MailTreeViewer" TagPrefix="mail" %>--%>
    
<%@ Register Src="~/Controls/MailModal.ascx" TagName="SendMail" TagPrefix="mail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="WestContentPlaceHolder" runat="server">
    <asp:UpdatePanel runat="server" ID="pnlNav" UpdateMode="Conditional">
        <ContentTemplate> 
            <mail:MailNav ID="Navigator" runat="server" EnableNewMail="false" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div id="mainDivNascita" class="content-panel-borderless">
        <div class="header-panel-blue">
            <div class="header-title">
                <div class="header-text-left">
                    <label>
                        Mail Box</label>
                </div>
            </div>
        </div>
        <div id="container" class="body-panel">
            <asp:UpdatePanel runat="server" ID="pnlLogin" UpdateMode="Conditional" ChildrenAsTriggers="false">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="Login" />
                </Triggers>
                <ContentTemplate>
                    <mail:MailBoxLogin ID="Login" runat="server" OnChangeStatus="Login_OnChangeStatus">
                    </mail:MailBoxLogin>
                    <mail:Inbox ID="Inbox1" runat="server" Visible="false" OnRowSelected="Inbox1_OnRowSelected"
                        MailBoxProvider="~/api/PECController/GetMails" />
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="pnlMail" runat="server" RenderMode="Block" UpdateMode="Conditional"
                ChildrenAsTriggers="true">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="Inbox1" />
                    <%--<asp:AsyncPostBackTrigger ControlID="mailTreeViewer" />--%>
                </Triggers>
                <ContentTemplate>
                    <uc1:MailViewer ID="MailViewer1" runat="server" EnableAcquire="true" EnableForward="true"
                     OnMailSelected="mailTreeViewer_MailSelected"   EnableRating="true" EnableReplyAll="false" EnableReplyTo="true" EnableMailTree="true"
                        OnRequireAction="MailViewer1_OnRequireAction" OnAccountInvalid="MailViewer1_AccountInvalid" />
                    <mail:SendMail ID="ucSendMail" runat="server" Visible="false" SottoTitolo="0" OnMessageInvalid="ucSendMail_MessageInvalid" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
     <%--   <mail:MailTreeViewer runat="server" ID="mailTreeViewer" OnMailSelected="mailTreeViewer_MailSelected" />--%>
        <!-- id="container"-->
    </div>
</asp:Content>

