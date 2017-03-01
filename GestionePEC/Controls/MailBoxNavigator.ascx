<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailBoxNavigator.ascx.cs" Inherits="GestionePEC.Controls.MailBoxNavigator" %>
<%@ Import Namespace="ActiveUp.Net.Common.DeltaExt" %>
<%@ Register Assembly="GestionePEC" Namespace="GestionePEC.Extensions" TagPrefix="inl" %>
<div id="<%= this.ClientID +"_Nav"%>">
</div>
<inl:InlineScript runat="server">

    <script type="text/javascript">
   Ext.onReady(function() {
       var json = eval("<%= this.Json %>");

       var Treestore = Ext.create('Ext.data.TreeStore', {
           id:'TreeNavigator',
           root: {
               expanded: true,
               children: [{
                   'text': '<%= this.Account %>',
                   'id': 'account',                
                   'href':'MailBoxInBox.aspx',
                   'expanded': true,       
                   'children': json
               },  {
                   'text': 'Configurazione Mail',
                   'id': 'config',
                   'href':'MailAdmin.aspx',
                   'leaf': true,
                   'icon': '<%= this.ResolveClientUrl("~/App_Themes/unisys/images/default/tree/folder.gif") %>'
               }]
           }
       });


       function SetColumnHeadersNavigator() {
           var colMod = Ext.getCmp('mailGrid').getView().getHeaderCt().getGridColumns();
           if (colMod != null) {              
               if (parentFolder == "O" || parentFolder == "AO" || Ext.getCmp('folders').getValue()== "6") {                  
                   Ext.getCmp('mailGrid').columns[4].setText('Destinatario');
                   Ext.getCmp('mailGrid').columns[3].setText('Inviato il');                  
               } else {
                   Ext.getCmp('mailGrid').columns[4].setText('Mittente');
                   Ext.getCmp('mailGrid').columns[3].setText('Ricevuto il');                  
               }
           }
       };

       function doSearchFunction(Folder_id) {
           var currentFolder = Ext.get('folders').getValue();
           if (currentFolder != Folder_id) {
               currentFolder = Folder_id;             
               var store = Ext.data.StoreManager.lookup('mailStore');                  
               store.load({ params: { start: 0, limit: 25, folder: currentFolder } });
               SetColumnHeadersNavigator();
           }
       };

            window['<%= this.ClientID %>_tree'] = new Ext.create('Ext.tree.Panel', {
                renderTo: '<%= this.ClientID +"_Nav"%>',
                useArrows: true,
                animate: true,
                enableDD: true,                      
                rootVisible: false,
                store:Treestore,
                listeners: {
                    itemclick: function (n, record, item, index, e, eOpts) {
                        var gridPanel = Ext.getCmp('mailGrid');  
                        if (typeof gridPanel != 'undefined' && gridPanel != null) {
                        if(record.id != "I" && record.id != "C" && record.id != "O" && record.id != "A")
                        {                          
                            //  var act = gridPanel.getTopToolbar().find('id', 'folders')[0];
                            var act = Ext.getCmp('folders').getValue();
                            var val = '0';
                            var pa = '0';
                            val = record.id;   
                            if(record.parentNode.id != 'account')
                           {                            
                           // if(record.parentNode.id == 'C' || record.parentNode.id == 'A')
                           // {pa = record.attributes.parentid;}
                                // else {
                                pa = record.parentNode.id;
                            //}                      
                           }      
                           parentFolder = pa;  
                           if (typeof act != 'undefined' && act != null && val != '0') {                               
                               Ext.getCmp('folders').setValue(val);
                               currentFolder = val;
                               doSearchFunction(val);
                              // Ext.getCmp('folders').collapse();
                            //   Ext.getCmp('folders').fireEvent('collapse', act);
                            }
                           }
                        }
                    }
                }
            });
            var LeftTree = eval('<%= this.ClientID %>_tree');
           // LeftTree.expandAll();
        });
    </script>
</inl:InlineScript>
