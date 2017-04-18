<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Mail.Master" AutoEventWireup="true" CodeBehind="AddUserToRole.aspx.cs" Inherits="GestionePEC.pages.Users.AddUserToRole" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
     <div id="pnlMain" class="content-panel-borderless">
        <div class="header-panel-blue">
            <div class="header-title">
                <div class="header-text-left">
                    <label>
                        GESTIONE UTENTE RUOLO</label>
                </div>
            </div>
        </div>
        <div id="container" class="control-main">
            <div id="divparole" class="control-header-blue">
                <div class="header-title">
                    <div class="header-text-left">
                        <label>Dati Utente Ruolo</label>
                    </div>                  
                </div>
            </div>          
            <div class="control-body-gray">
                  <table align="center" border="0">
                    <tr>
                        <td>
                         <asp:Label runat="server" AssociatedControlID="UserName">Utenti</asp:Label>
                            </td>
                        <td>
                               <asp:DropDownList runat="server" ID="Users">                                       
                                     </asp:DropDownList>       
                        </td>
                        </tr>                      
                        <tr>
                         <td>
                               <asp:Label runat="server" AssociatedControlID="Role">Ruolo</asp:Label>
                          </td>
                          <td>
                                 <asp:DropDownList runat="server" ID="Role">                                       
                                     </asp:DropDownList>       
                          </td>
                    </tr>
                </table>
              <div style="text-align: left; margin: 10px 0 5px 435px;">
               <asp:Button runat="server" OnClick="CreateUserRole_Click" Text="Associa" />
                </div>
            </div>     
        </div>
    </div>
</asp:Content>

