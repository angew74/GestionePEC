<%@ Page Title="Gestione Errore" Language="C#" MasterPageFile="~/Master/Mail.Master" Theme="Delta" AutoEventWireup="true" CodeBehind="ErrorPage.aspx.cs" Inherits="GestionePEC.pages.common.ErrorPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="WestContentPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div class="body-panel" style="padding: 6px; padding-bottom: 0;">
        <div class="content-panel">
            <div class="header-panel-gray">
                <div class="header-title">
                    <div class="header-text-left">
                        <label>
                            Benvenuti nell'Applicazione Gestione PEC</label>
                    </div>
                </div>
            </div>
            <div class="body-panel-gray">
                <big><b><i>Informazioni</i></b></big>
                   <ul style="margin-top: 0px">
                     <li>Attenzione non è stato possibile interrogare i sistemi centrali, è possibile: </li>
                    <li>1) Autenticarsi nuovamente e riprovare utilizzando utilizzando il pannello superiore</li>
                    <li>2)Nel caso il problema persista segnalare l'inconveniente contattando uno dei numeri sotto indicati * GRAZIE *            
			     	Tel. : 06 XXXXXXX   </li>
                </ul>
                </div>
               </div>
               </div>
               </asp:Content>            
<asp:Content ID="Content3" ContentPlaceHolderID="RightContentPlaceHolder" runat="server">
</asp:Content>
