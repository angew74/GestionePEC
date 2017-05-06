<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Mail.Master" AutoEventWireup="true" Theme="Delta" CodeBehind="ManagerUserToRole.aspx.cs" Inherits="GestionePEC.pages.Users.ManagerUserToRole" %>
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
                        <label>Aggiungi Utente Ruolo</label>
                    </div>                  
                </div>
            </div>          
            <div class="control-body-gray">
                  <table align="center" border="0">
                    <tr>
                        <td>
                         <asp:Label runat="server" AssociatedControlID="Users" Width="200px">Utenti:</asp:Label>
                            </td>
                        <td>
                               <asp:DropDownList runat="server" ID="Users">                                       
                                     </asp:DropDownList>       
                        </td>
                        </tr>                      
                        <tr>
                         <td>
                               <asp:Label runat="server" AssociatedControlID="Role" Width="200px">Ruolo:</asp:Label>
                          </td>
                          <td>
                                 <asp:DropDownList runat="server" ID="Role">                                       
                                     </asp:DropDownList>       
                          </td>
                    </tr>
                </table>
              <div style="text-align: left; margin: 10px 0 5px 935px;">
               <asp:Button runat="server" OnClick="CreateUserRole_Click" Text="Associa" />
                </div>
            </div>     
        </div>
            <div id="containerDelete" class="control-main">
            <div id="divparoleDelete" class="control-header-blue">
                <div class="header-title">
                    <div class="header-text-left">
                        <label>Rimuovi Utente Ruolo</label>
                    </div>                  
                </div>
            </div>          
            <div class="control-body-gray">
               <div id="users-grid"></div>
            </div>     
        </div>
    </div>
    <script type="text/javascript">

        Ext.Loader.setConfig({ enabled: true });
        Ext.Loader.setPath('Ext.ux', '../../ScanOnLineExt/Scripts/Extjs');
        Ext.require([
           'Ext.grid.*',
           'Ext.data.*',
           'Ext.util.*',
           'Ext.toolbar.Paging'        
         
        ]);
        Ext.onReady(function () {
            Ext.tip.QuickTipManager.init();


            var requiredMessage = '<div style="color:red;">Campo obbligatorio</div>';

            function ManageError(msg) {
                var summary = Ext.getCmp("labelSummary");
                summary.setValue(msg);
                var panelsummary = Ext.getCmp("panelSummary");
                panelsummary.show();
            };

            function HideError() {
                var summary = Ext.getCmp("labelSummary");
                summary.setValue("");
                var panelsummary = Ext.getCmp("panelSummary");
                panelsummary.hide();
            }

            Ext.define('RolesModel', {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'Id', type: 'string' },
                    { name: 'Name', type: 'string' }
                ]
            });

            var readerRoles = new Ext.data.JsonReader({
                idProperty: 'Id',
                model: 'RolesModel',
                messageProperty: 'Message',
                type: 'json',
                rootProperty: 'RolesList',
                totalProperty: 'Totale'
            });

            var storeRoles = Ext.create('Ext.data.Store', {
                autoLoad: true,
                storeId: 'storeRoles',
                model: 'RolesModel',
                reader: readerRoles,
                proxy:
                   {
                       type: 'ajax',
                       url: '/GestionePEC/api/RolesServiceController/GetAllRoles',
                       reader: readerRoles
                   },
                //  restful: true,
                listeners: {
                    load: function (s, r, o) {
                    },
                    exception: function () {
                    }
                }
            });

            var RolesCombo = Ext.create('Ext.form.field.ComboBox', {
                fieldLabel: 'Ruolo',
                emptyText: 'Selezionare un ruolo',
                //  renderTo: 'TipoCombo',
                id: 'RolesCombo',
                displayField: 'Name',
                name: 'Role',
                valueField: 'Id',
                ctCls: 'LabelBlackBold',
                editable: false,
                tpl: Ext.create('Ext.XTemplate',
              '<ul class="x-list-plain"><tpl for=".">',
                  '<li role="option" class="x-boundlist-item">{Name}</li>',
              '</tpl></ul>'
          ),
                width: 365,
                labelWidth: 60,
                margin: '0 0 0 10px',
                store: storeRoles,               
                queryMode: 'local',
                listeners: {
                    change: function (field, value, oldvalue) {
                        Ext.getCmp('UsersGrid').getStore().getProxy().extraParams.role = value;
                    }
                }
            });

            Ext.define('UsersModel', {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'UserName', type: 'string' },
                    { name: 'UserName', type: 'string' }
                ]
            });


            var readerUsers = new Ext.data.JsonReader({
                idProperty: 'UserName',
                model: 'UsersModel',
                messageProperty: 'Message',
                type: 'json',
                rootProperty: 'ListUtenti',
                totalProperty: 'Totale'
            });

            var storeUsers = Ext.create('Ext.data.Store', {
                autoLoad: true,
                storeId: 'storeUsers',
                model: 'UsersModel',
                reader: readerUsers,
                proxy:
                   {
                       type: 'ajax',
                       url: '/GestionePEC/api/LogsServiceController/getUsers',
                       reader: readerUsers
                   },
                //  restful: true,
                listeners: {
                    load: function (s, r, o) {
                    },
                    exception: function () {
                    }
                }
            });

            var UsersCombo = Ext.create('Ext.form.field.ComboBox', {
                fieldLabel: 'Utente',
                emptyText: 'Selezionare un utente',
                //  renderTo: 'TipoCombo',
                id: 'UsersCombo',
                displayField: 'UserName',
                valueField: 'UserName',
                ctCls: 'LabelBlackBold',
                editable: false,
                tpl: Ext.create('Ext.XTemplate',
              '<ul class="x-list-plain"><tpl for=".">',
                  '<li role="option" class="x-boundlist-item">{UserName}</li>',
              '</tpl></ul>'
          ),
                width: 365,
                labelWidth: 60,
                margin: '0 0 0 4px',
                store: storeUsers,
                queryMode: 'local',
                listeners: {
                    change: function (combo, value) {
                        Ext.getCmp('UsersGrid').getStore().getProxy().extraParams.username = value;
                    }
                }
            });

         
            var buttonReset = Ext.create('Ext.Button', {
                text: 'Riazzera',
                //  renderTo: 'divBottoni',
                cls: 'x-btn-text-icon',
                width: 80,
                iconCls: 'bRefresh',
                handler: function () {
                    // this button will spit out a different number every time you click it.
                    Ext.getCmp('UsersCombo').setValue('');
                    Ext.getCmp('RolesCombo').setValue('');
                   
                }
            });

            var buttonFilter = Ext.create('Ext.Button', {
                // text: 'Riazzera campi ricerca',
                //  renderTo: 'divBottoni',
                cls: 'x-btn-text-icon',
                width: 50,
                iconCls: 'bFind',
                handler: function () {
                    // this button will spit out a different number every time you click it.
                    if ((Ext.getCmp('UsersCombo').getValue() != null && Ext.getCmp('UsersCombo').getValue() != '')
                        && (Ext.getCmp('RolesCombo').getValue() != null && Ext.getCmp('RolesCombo').getValue() != '')) {
                        Ext.Msg.alert('Attenzione', 'Scegliere utente o ruolo');
                    }
                    else if ((Ext.getCmp('UsersCombo').getValue() == null || Ext.getCmp('UsersCombo').getValue() == '')
                        && (Ext.getCmp('RolesCombo').getValue() == null || Ext.getCmp('RolesCombo').getValue() == '')) {
                        Ext.Msg.alert('Attenzione', 'Scegliere almeno un filtro');
                    }
                    else {
                        store.getProxy().setExtraParam("username", Ext.getCmp('UsersCombo').getValue());
                        store.getProxy().setExtraParam("role", Ext.getCmp('RolesCombo').getValue());
                        Ext.getCmp("UsersGrid").store.loadPage(1);
                    }
                }
            });

            var toolBar = Ext.create('Ext.panel.Panel', {
                //   width: 1000,
                //   height: 150,
                //  renderTo: 'toolBarDiv',
                scrollable: true,
                dockedItems: [
                    {
                        xtype: 'toolbar',
                        dock: 'top',
                        items: [UsersCombo, RolesCombo, buttonFilter, buttonReset]
                    }
                ]
            });

            Ext.define('UtentiModel', {
                extend: 'Ext.data.Model',
                fields: [
                    'Id',
                    'UserName',
                     'Role'                    
                ]
              //  ,idProperty: 'UserName'
            });

            var urlApiGetUtenti = '/GestionePEC/api/UsersServiceController/GetUtenti';

               var store = Ext.create('Ext.data.Store', {
                pageSize: 5,
                id: 'StoreUtenti',
                model: 'UtentiModel',
                remoteSort: true,
                beforeload: function () {
                    store.getProxy().extraParams = {}; // clear all previous                   
                },
                proxy: {                  
                    type: 'ajax',
                    url: urlApiGetUtenti,
                    reader: {
                        type: 'json',
                        rootProperty: 'UtentiList',                      
                        totalProperty: 'Totale'
                    },                   
                    simpleSortMode: true
                }
            });

            var grid = Ext.create('Ext.grid.Panel', {
                //width: 900,
                //height: 300,
                store: store,
                id: 'UsersGrid',
                region:'north',
                loadMask: true,               
                viewConfig: {
                    forceFit: true,
                    loadingText: "Caricamento griglia in corso...."
                },
                split: true,
                region: 'north',
                // grid columns
                columns: [
                    {
                    id: 'UserNameField',
                    text: 'UserName',
                    width: 220,
                    flex: 1,
                    dataIndex: 'UserName'                   
                }, {
                    id: 'IdField',
                    text: 'Id',
                    dataIndex: 'Id',
                    width: '20',
                    hidden: true
                }, {
                    id: "RoleField",
                    text: "Ruolo",
                    dataIndex: 'Role',
                    width: 220,
                    flex:2,
                    align: 'center',
                    sortable: true
                }, {
                    xtype: 'actioncolumn',
                    width: 50,
                    items: [{
                        tooltip: 'Rimuovi Ruolo',
                        iconCls: 'bRimRol',                       
                        handler: function (grid, rowIndex, colIndex, item, e, record) {
                            var rec = grid.getStore().getAt(rowIndex);
                            RimuoviRuolo(rec);
                        }
                    }]
                },{
                        xtype: 'actioncolumn',
                        width: 50,
                        items: [{                       
                            tooltip: 'Cancella Utente',
                            iconCls:'bDelUser',                           
                            handler: function (grid, rowIndex, colIndex, item, e, record) {
                                var rec = grid.getStore().getAt(rowIndex);
                                DeleteUser(rec);                         
                            }
                        }]
                }
              ],  
                // paging bar on the bottom
                bbar: Ext.create('Ext.toolbar.Paging', {
                    store: store,
                    displayInfo: true,
                    autoWidth: true,
                    id: 'PagingUtenti',
                    displayMsg: 'Visualizzazione righe {0} - {1} of {2}',
                    emptyMsg: "Nessun record da visualizzare",
                    listeners: {
                        afterLayout: function (asd) {
                            this.container.dom.style.width = "";
                            this.el.dom.style.width = "";
                        }
                    }
                })
            });

            Ext.create('Ext.Panel', {
                renderTo: 'users-grid',
                frame: true,
                resizable: true,
                title: 'Elenco Utenti',
                //  width: 1205,
                // minWidth: 1220,
                height: 590,
                border: false,
                layout: 'anchor',
                items: [
                   toolBar, grid]
            });

            function RimuoviRuolo(rec){
                Ext.Ajax.request({
                    url: '/GestionePEC/api/UsersServiceController/RemoveRole?userid=' + rec.data["Id"] + '&role=' +rec.data["Role"],
        //            type: 'json',
                    method: 'GET',
                    success: function (response, opts) {
                        var obj = Ext.decode(response.responseText).message;
                        if (obj != null) {
                            var message = obj;
                            Ext.Msg.alert('Errore nella rimozione utente', message);
                            // ManageError("Errore nell'aggiornameto dettagli: " + message);
                        }
                        else {
                            Ext.Msg.alert('Complimenti', "Utente rimosso dal ruolo con successo");
                            Ext.getCmp("UsersGrid").store.loadPage(1);
                        }
                    },
                    failure: function(response,opts)
                    {
                        if (Ext.decode(response.responseText) != null) {
                            var message = Ext.decode(response.responseText).message;
                            Ext.Msg.alert('Errore nella cancellazione utente', message);
                        }
                        else {
                            Ext.Msg.alert('Errore nella cancellazione utente', "Errore di sistema");
                        }
                    }
                });
            }

            function DeleteUser(rec) {
                Ext.Ajax.request({
                    url: '/GestionePEC/api/UsersServiceController/DeleteUser?userid=' + rec.data["Id"],
                    type: 'json',
                    method: 'GET',
                    success: function (response, opts) {
                        var obj = Ext.decode(response.responseText).message;
                        if (obj != null) {
                            var message = obj;                         
                            Ext.Msg.alert('Errore nella cancellazione utente', message);
                            // ManageError("Errore nell'aggiornameto dettagli: " + message);
                        }
                        else {
                            Ext.Msg.alert('Complimenti', "Utente cancellato con successo");
                            Ext.getCmp("UsersGrid").store.loadPage(1);
                        }
                    },
                    failure: function (response, opts)
                    {
                        if(Ext.decode(response.responseText) != null)
                        {   
                            var message = Ext.decode(response.responseText).message;
                            Ext.Msg.alert('Errore nella cancellazione utente', "Errore di sistema");   
                        }
                    }
                });
            }
                    

        });

            </script>
</asp:Content>

 