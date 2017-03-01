<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailBoxLogin.ascx.cs" Inherits="GestionePEC.Controls.MailBoxLogin" %>
<%@ Register Assembly="GestionePEC" Namespace="GestionePEC.Extensions" TagPrefix="inl" %>
<div class="control-main">
    <div class="control-header-blue">
        <div class="header-title">
            <div class="header-text-left">
                <label>
                    <%= this.CurrentMail%></label>
            </div>
            <div class="header-text-right">
                <asp:Button ID="IbLogOff" SkinID="ToolboxButton" CssClass="bClose" runat="server"
                    OnClick="lbtnLogOff_Click" />
            </div>
        </div>
    </div>
    <asp:Panel ID="panLogin" runat="server" CssClass="control-body-gray">
        <asp:CheckBox runat="server" ID="ManagedAccount" Checked="true" TextAlign="left"
            Text="Account Gestito" AutoPostBack="false" onclick="checkChange(this)" />
        <div id="<%= this.ClientID+"_man" %>">
            <asp:DropDownList runat="server" ID="ddlManagedAccounts" ToolTip="Accounts gestiti"
                AutoPostBack="true" OnDataBinding="ddlManagedAccounts_DataBinding" OnDataBound="ddlManagedAccounts_DataBound"
                OnSelectedIndexChanged="ddlManagedAccounts_SelectedIndexChanged" />
        </div>
        <div id="<%= this.ClientID+"_unman" %>">
            <label class="LabelBlack" style="min-width: 11em;">
                Server:</label>
            <asp:DropDownList ID="ddlServer" runat="server" ToolTip="Server di Posta configurati"
                OnDataBinding="ddlServer_DataBinding">
            </asp:DropDownList>
            <asp:HiddenField ID="hidDominus" runat="server" Value="comune.roma.it" />
            <label class="LabelBlack">
                Mail:</label>
            <asp:TextBox ID="txtName" runat="server" Width="200px" CssClass="formtext">portaapplicativa</asp:TextBox>
            <label class="LabelBlack">
                Password:</label>
            <asp:TextBox ID="txtPassword" runat="server" Width="100px" TextMode="Password" CssClass="formtext"></asp:TextBox>
            <asp:Button ID="btnLogin" runat="server" Text="Login" Width="100px" OnClick="btnLogin_Click" />
        </div>
    </asp:Panel>
</div>
<inl:InlineScript runat="server">

    <script type="text/javascript">

    var chkMng = document.getElementById('<%= this.ManagedAccount.ClientID %>');
    var managedPanel = document.getElementById('<%= this.ClientID+"_man" %>');
    var unmanagedPanel = document.getElementById('<%= this.ClientID+"_unman" %>');
    if(typeof chkMng != 'undefined' && chkMng != null){
        if(chkMng.checked){
            managedPanel.style.display = 'inline';
            unmanagedPanel.style.display = 'none';
        }
        else {
            managedPanel.style.display = 'none';
            unmanagedPanel.style.display = 'inline';
        }
    }
    
    function checkChange(obj) {
        if (obj.checked) {
            managedPanel.style.display = 'inline';
            unmanagedPanel.style.display = 'none';
        } else {
            managedPanel.style.display = 'none';
            unmanagedPanel.style.display = 'inline';
        }
    }
    </script>

</inl:InlineScript>
