<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Mail.Master" AutoEventWireup="true" CodeBehind="RegisterRole.aspx.cs" Inherits="GestionePEC.pages.Roles.RegisterRole" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
     <div id="pnlMain" class="content-panel-borderless">
        <div class="header-panel-blue">
            <div class="header-title">
                <div class="header-text-left">
                    <label>
                        REGISTRAZIONE RUOLO</label>
                </div>
            </div>
        </div>
        <div id="container" class="control-main">
            <div id="divparole" class="control-header-blue">
                <div class="header-title">
                    <div class="header-text-left">
                        <label>Dati Ruolo</label>
                    </div>                  
                </div>
            </div>          
            <div class="control-body-gray">
                  <table align="center" border="0">
                    <tr>
                        <td>
                          <asp:Label runat="server" AssociatedControlID="UserName">Nome Ruolo</asp:Label>
                            </td>
                        <td>
                               <asp:TextBox runat="server" ID="RoleName" /> 
                        </td>
                        </tr>                 
                      </table>
             <div style="text-align: left; margin: 10px 0 5px 435px;">
               <asp:Button runat="server" OnClick="CreateRole_Click" Text="Memorizza" />
                </div>
                </div>
        </div>
        </div>
</asp:Content>
