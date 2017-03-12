<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntitaSearch.ascx.cs" Inherits="GestionePEC.Controls.EntitaSearch" %>
<div class="control-header-blue" id="pnlHeader">
    <div class="control-header-title">
        <div class="control-header-text-left">            
                Ricerche Enti           
        </div>
    </div>
</div>
<div class="body-panel-gray">
    <div class="content-panel-borderless">       
        <div style="display: inline-block">
            <select id="slcSearchType" name="slcSearchType">
                <option />
            </select>
        </div>
        <div style="display: inline-block">
            <select id="slcEntita" name="slcEntita">
                <option />
            </select>
            <asp:HiddenField runat="server" ID="hfSubType" />
        </div>
    </div>

    <script type="text/javascript">
        var configResearch;
        Ext.onReady(function(){
            configResearch = {
                cbxType: Ext.get('slcSearchType'),
                cbxEntita: Ext.get('slcEntita'),
                hfEntitaType: Ext.get('<%= hfSubType.ClientID %>'),
                provider: '<%= Page.ResolveClientUrl("~/api/GeneralSearchController/")%>/GetEntitaByPartialName'
            };
        });
        Ext.onReady(function(){
        //chiamata per forzare l'istanza dell'host service
            Ext.Ajax.request({
                url: configResearch.provider,
                params: { text: '', type: '', start: 0, limit: 0 },
                method: 'GET',
                disableCaching: false
            });
        }, this, { single: true });
    </script>

</div>
