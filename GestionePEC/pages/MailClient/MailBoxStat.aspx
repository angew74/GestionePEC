<%@ Page Title="Statistiche" Language="C#" Theme="Delta" MasterPageFile="~/Master/Mail.Master" AutoEventWireup="true" CodeBehind="MailBoxStat.aspx.cs" Inherits="GestionePEC.pages.MailClient.MailBoxStat" %>
<%@ Register TagPrefix="uc" TagName="Ricerca" Src="~/controls/RicercaStat.ascx" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<div id="mainDivNascita" class="content-panel-borderless">
        <div class="header-panel-blue">
            <div class="header-title">
                <div class="header-text-left">
                    <label>
                        Mail Box Statistica</label>
                </div>
            </div>
        </div>
        <div id="container" class="body-panel">
<uc:Ricerca ID="ucStatistica" runat="server" />
</div>
</asp:Content>
