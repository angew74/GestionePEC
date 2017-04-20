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

            var UsersTextField = Ext.create('Ext.form.TextField', {
                id: 'UsersField',
                fieldLabel: 'Utente',
                labelWidth: 100,
                enforceMaxLength: true,
                width: 400,
                maxlength: 20,
                maxLengthText: 'Massimo 20 carattere',
                margin: '0 0 0 4px',
                emptyText: '',
                // renderTo: 'RichiedenteDiv',
                listeners: {
                    change: function (textField, value, oldvalue) {
                        Ext.getCmp('UsersGrid').getStore().getProxy().extraParams.username = value;
                    }
                }
            });

            var RolesTextField = Ext.create('Ext.form.TextField', {
                id: 'RolesField',
                fieldLabel: 'Ruolo',
                labelWidth: 100,
                maxlength: 20,
                enforceMaxLength: true,
                maxLengthText: 'Massimo 20 caratteri',
                width: 400,
                margin: '0 0 0 4px',
                emptyText: '',
                listeners: {
                    change: function (textField, value, oldvalue) {
                        Ext.getCmp('UsersGrid').getStore().getProxy().extraParams.role = value;
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
                    Ext.getCmp('UsersField').setValue('');
                    Ext.getCmp('RolesField').setValue('');                  
                   
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
                    store.getProxy().setExtraParam("username", Ext.getCmp('UsersField').getValue());
                    store.getProxy().setExtraParam("role", Ext.getCmp('RolesField').getValue());
                    Ext.getCmp("UsersGrid").store.loadPage(1);
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
                        items: [UsersTextField, RolesTextField, buttonFilter, buttonReset]
                    }
                ]
            });

            Ext.define('UtentiModel', {
                extend: 'Ext.data.Model',
                fields: [
                    'Id',
                    'UserName',
                     'Role'                    
                ],
                idProperty: 'UserName'
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
                    dataIndex: 'UserName'                   
                }, {
                    id: 'IdField',
                    text: 'Id',
                    dataIndex: 'Id',
                    hidden: true
                }, {
                    id: "RoleField",
                    text: "Ruolo",
                    dataIndex: 'Role',
                    width: 100,
                    align: 'center',
                    sortable: true
                }, {
                    xtype: 'actioncolumn',
                    width: 50,
                    items: [{                       
                        tooltip: 'Rimuovi Ruolo',
                        iconCls: function (view, rowIndex, colIndex, item, record) {
                            return 'bSearchWhite';
                            // Returns true if 'editable' is false (, null, or undefined)
                            //if (record.get('StatusRichiesta') != "C")
                            //{ return 'x-hide-display'; }
                            //else {
                            //    return 'ViewAttoCls';
                            //}
                        },
                        handler: function (grid, rowIndex, colIndex, item, e, record) {
                            var rec = grid.getStore().getAt(rowIndex);
                            RimuoviRuolo(rec);                         
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
                    url: 'GestionePEC/api/UsersServiceController/RemoveRole?userid=' + rec.data["Id"] + '&roleid=' +rec.data["Role"],
                    type: 'json',
                    method: 'GET',
                    success: function (response, opts) {
                        var obj = response.responseText;
                    },
                    failure: function(response,opts)
                    {}
                });
            }
                    

        });

            </script>
</asp:Content>

 