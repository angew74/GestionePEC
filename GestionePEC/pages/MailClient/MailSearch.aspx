<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Mail.Master" Theme="Delta" AutoEventWireup="true" CodeBehind="MailSearch.aspx.cs" Inherits="GestionePEC.pages.MailClient.MailSearch" %>
<%@ Register Src="~/controls/RicercaMail.ascx" TagName="MailSearch" TagPrefix="uc2" %>
<%@ Register Src="~/Controls/MailViewer.ascx" TagName="MailViewer" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/MailTreeViewer.ascx" TagName="MailTreeViewer" TagPrefix="mail" %>
<%@ Register Src="~/Controls/MailModal.ascx" TagName="SendMail" TagPrefix="mail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="WestContentPlaceHolder" runat="server">
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
                <ContentTemplate>   
                <uc2:MailSearch ID="MailResearch" runat="server" OnSelectMail="MailResearch_Mail" OnHideMail="MailResearch_Hide" />  
                </ContentTemplate>
            </asp:UpdatePanel>             
              <asp:UpdatePanel ID="pnlMail" runat="server" RenderMode="Block" UpdateMode="Conditional"
                ChildrenAsTriggers="true">
                 <Triggers>
                 <asp:AsyncPostBackTrigger ControlID="mailTreeViewer" />
                 </Triggers>
                  <ContentTemplate>
                    <uc1:MailViewer ID="MailViewer1" runat="server" EnableAcquire="false" EnableForward="true"
                        EnableRating="true" EnableReplyAll="false" EnableReplyTo="true" EnableMailTree="true"
                        OnRequireAction="MailViewer1_OnRequireAction" OnAccountInvalid="MailViewer1_AccountInvalid" />   
                         <mail:SendMail ID="ucSendMail" runat="server" Visible="false" SottoTitolo="0" OnMessageInvalid="ucSendMail_MessageInvalid" />              
                </ContentTemplate>
                </asp:UpdatePanel>                
        </div>
           <mail:MailTreeViewer runat="server" ID="mailTreeViewer" OnMailSelected="mailTreeViewer_MailSelected" />
           </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="RightContentPlaceHolder" runat="server">
</asp:Content>
