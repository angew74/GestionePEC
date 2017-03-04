<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Paging.ascx.cs" Inherits="GestionePEC.Controls.Paging" %>
<div style="z-index: 0" class="tabella">
    <div class="colonna">
        <div style="margin-top:4px; padding-top:3px;">
            <asp:Label ID="labPager" ForeColor="#15428B" Text="Seleziona una pagina:" runat="server" />
        </div>
    </div>
    <div class="colonna">
        <asp:DropDownList ID="ddlPagerPages" CausesValidation="false" AutoPostBack="true"
            runat="server" OnSelectedIndexChanged="OnPagerIndexChanged" />
    </div>
    <div class="colonna">
        <div style="margin-top:4px; padding-top:3px;">
            <asp:Label ID="labPagerPages" ForeColor="#15428B" runat="server" />
        </div>
    </div>
    <div class="clear">
    </div>
    <asp:HiddenField ID="hfPagingValue" Value="0" runat="server" />
</div>