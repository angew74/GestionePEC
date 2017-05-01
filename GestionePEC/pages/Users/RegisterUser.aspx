<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Mail.Master" AutoEventWireup="true" Theme="Delta" CodeBehind="RegisterUser.aspx.cs" Inherits="GestionePEC.pages.Users.RegisterUser" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div id="pnlMain" class="content-panel-borderless">
        <div class="header-panel-blue">
            <div class="header-title">
                <div class="header-text-left">
                    <label>
                        REGISTRAZIONE UTENTE</label>
                </div>
            </div>
        </div>
        <div id="container" class="control-main">
            <div id="divparole" class="control-header-blue">
                <div class="header-title">
                    <div class="header-text-left">
                        <label>Dati Utente</label>
                    </div>                  
                </div>
            </div>          
            <div class="control-body-gray">
                  <table align="center" border="0">
                    <tr>
                        <td>
                         <asp:Label runat="server" AssociatedControlID="UserName">UserName</asp:Label>
                            </td>
                        <td>
                               <asp:TextBox runat="server" SkinID="tb11Char" ID="UserName" /> 
                        </td>
                        </tr>
                      <tr>
                          <td>
                               <asp:Label runat="server" AssociatedControlID="Password">Password</asp:Label>
                          </td>
                          <td>
                                 <asp:TextBox runat="server" SkinID="tb10Char" MaxLength="10" ID="Password" TextMode="Password" />         
                          </td>
                      </tr>
                    <tr>
                        <td>
                              <asp:Label runat="server" AssociatedControlID="ConfirmPassword">conferma password</asp:Label>
                        </td>
                    <td>
                             <asp:TextBox runat="server" ID="ConfirmPassword" MaxLength="10"  SkinID="tb10Char" TextMode="Password" />     
                    </td>
                    </tr>
                       <tr>
                        <td>
                              <asp:Label runat="server" AssociatedControlID="Cognome">Cognome</asp:Label>
                        </td>
                    <td>
                             <asp:TextBox runat="server" SkinID="tbLong" ID="Cognome"  />     
                    </td>
                    </tr>
                       <tr>
                        <td>
                              <asp:Label runat="server" AssociatedControlID="Nome">Nome</asp:Label>
                        </td>
                    <td>
                             <asp:TextBox runat="server" SkinID="tbLong" ID="Nome"  />     
                    </td>
                    </tr>
                        <tr>
                        <td>
                              <asp:Label runat="server" AssociatedControlID="CodiceFiscale">Codice fiscale</asp:Label>
                        </td>
                    <td>
                             <asp:TextBox runat="server" ID="CodiceFiscale" SkinID="tb16Char"  />     
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
              <div style="text-align: left; margin: 10px 0 5px 935px;">
               <asp:Button runat="server" OnClick="CreateUser_Click" Text="Registra" />
                </div>
            </div>     
        </div>
    </div>
</asp:Content>


