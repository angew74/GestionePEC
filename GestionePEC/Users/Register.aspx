<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Mail.Master" AutoEventWireup="true" Theme="Delta"  CodeBehind="Register.aspx.cs" Inherits="GestionePEC.Users.Register" %>
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
        <div id="container" class="body-panel">
     <div>
        <h4 style="font-size: medium">Registra un utente</h4>
        <hr />
        <p>
            <asp:Literal runat="server" ID="StatusMessage" />
        </p>                
        <div style="margin-bottom:10px">
            <asp:Label runat="server" AssociatedControlID="UserName">UserName</asp:Label>
            <div>
                <asp:TextBox runat="server" ID="UserName" />                
            </div>
        </div>
        <div style="margin-bottom:10px">
            <asp:Label runat="server" AssociatedControlID="Password">Password</asp:Label>
            <div>
                <asp:TextBox runat="server" ID="Password" TextMode="Password" />                
            </div>
        </div>
        <div style="margin-bottom:10px">
            <asp:Label runat="server" AssociatedControlID="ConfirmPassword">conferma password</asp:Label>
            <div>
                <asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password" />                
            </div>
        </div>
        <div>
            <div>
                <asp:Button runat="server" OnClick="CreateUser_Click" Text="Registra" />
            </div>
        </div>
    </div>
            </div>
        </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="RightContentPlaceHolder" runat="server">
</asp:Content>
