<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Mail.Master" AutoEventWireup="true" Theme="Delta" CodeBehind="ManageLogs.aspx.cs" Inherits="GestionePEC.pages.Logs.ManageLogs" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
       <div id="pnlMain" class="content-panel-borderless">
        <div class="header-panel-blue">
            <div class="header-title">
                <div class="header-text-left">
                    <label>
                        GESTIONE LOGS </label>
                </div>
            </div>
        </div>
        <div id="container" class="control-main">
            <div id="divparole" class="control-header-blue">
                <div class="header-title">
                    <div class="header-text-left">
                        <label>Visualizza Logs Attività</label>
                    </div>                  
                </div>
            </div>          
            <div class="control-body-gray">
                   <div id="toolBarDiv"></div>
                        <div id="logs-grid"></div>
                        <div id="summary"></div>
                </div>
            </div>
           </div>
    <script type="text/javascript">
        Ext.Loader.setConfig({ enabled: true });
        Ext.require([
        'Ext.form.field.ComboBox',
        'Ext.form.FieldSet',
        'Ext.tip.QuickTipManager',
        'Ext.data.*'
        ]);
        Ext.onReady(function () {
            Ext.tip.QuickTipManager.init();

            // Model tipo
            Ext.define('LogCodeModel', {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'LOG_CODE', type: 'string' },
                    { name: 'DESCR', type: 'string' }
                ]
            });

            // Model tipo
            Ext.define('UsersModel', {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'UserName', type: 'string' },
                    { name: 'UserName', type: 'string' }
                ]
            });

            Ext.define('MailsModel', {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'emailAddress', type: 'string' },
                    { name: 'emailAddress', type: 'string' }
                ]
            });

            

            var requiredMessage = '<div style="color:red;">Campo obbligatorio</div>';

            // Model tipo
            Ext.define('AppCodeModel', {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'APP_CODE', type: 'string' },
                    { name: 'DESCR', type: 'string' }
                ]
            });

          
            var readerAppCode = new Ext.data.JsonReader({
                idProperty: 'APP_CODE',
                model: 'AppCodeModel',
                messageProperty: 'Message',
                type: 'json',
                rootProperty: 'ElencoAppCodes',
                totalProperty: 'Totale'
            });

            var readerMails = new Ext.data.JsonReader({
                idProperty: 'emailAddress',
                model: 'MailsModel',
                messageProperty: 'Message',
                type: 'json',
                rootProperty: 'ElencoMails',
                totalProperty: 'Totale'
            });

            var readerUsers = new Ext.data.JsonReader({
                idProperty: 'UserName',
                model: 'UsersModel',
                messageProperty: 'Message',
                type: 'json',
                rootProperty: 'ListUtenti',
                totalProperty: 'Totale'
            });

             var storeAppCode = Ext.create('Ext.data.Store', {
                autoLoad: true,
                storeId: 'storeAppCode',
                model: 'AppCodeModel',
                reader: readerAppCode,
                proxy:
                   {
                       type: 'ajax',
                       url: '/GestionePEC/api/LogsServiceController/getAppCodes',                       
                       reader: readerAppCode
                   },
                //  restful: true,
                listeners: {
                    load: function (s, r, o) {
                    },
                    exception: function () {
                    }
                }
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

             var storeMails = Ext.create('Ext.data.Store', {
                 autoLoad: true,
                 storeId: 'storeMails',
                 model: 'MailsModel',
                 reader: readerUsers,
                 proxy:
                    {
                        type: 'ajax',
                        url: '/GestionePEC/api/LogsServiceController/getMails',
                        reader: readerMails
                    },
                 //  restful: true,
                 listeners: {
                     load: function (s, r, o) {
                     },
                     exception: function () {
                     }
                 }
             });


             var readerLogCode = new Ext.data.JsonReader({
                 idProperty: 'LOG_CODE',
                 model: 'LogCodeModel',
                 messageProperty: 'Message',
                 type: 'json',
                 rootProperty: 'ElencoLogCodes',
                 totalProperty: 'Totale'
             });

             var storeLogCode = Ext.create('Ext.data.Store', {
                 autoLoad: true,
                 storeId: 'storeLogCode',
                 model: 'LogCodeModel',
                 reader: readerLogCode,
                 proxy:
                    {
                        type: 'ajax',
                        url: '/GestionePEC/api/LogsServiceController/getLogCodes',
                        reader: readerLogCode
                    },
                 //  restful: true,
                 listeners: {
                     load: function (s, r, o) {
                     },
                     exception: function () {
                     }
                 }
             });


            // Combo tipo
            var LogCodeCombo = Ext.create('Ext.form.field.ComboBox', {
                fieldLabel: 'Codice Log',
                emptyText: 'Selezionare un codice di messaggio',
                //  renderTo: 'TipoCombo',
                id: 'CodiceLogCombo',
                displayField: 'DESCR',
                valueField: 'LOG_CODE',
                ctCls: 'LabelBlackBold',
                editable: false,
                tpl: Ext.create('Ext.XTemplate',
              '<ul class="x-list-plain"><tpl for=".">',
                  '<li role="option" class="x-boundlist-item">{DESCR}</li>',
              '</tpl></ul>'
          ),
                width: 365,
                labelWidth: 80,
                margin: '0 0 0 4px',
                store: storeLogCode,
                queryMode: 'local',
                listeners: {
                    change: function (combo, value) {
                        Ext.getCmp('LogsGrid').getStore().getProxy().extraParams.logcode = value;
                    }
                }
            });


            // Combo Utenti
            var UsersCombo = Ext.create('Ext.form.field.ComboBox', {
                fieldLabel: 'Utente',
                emptyText: 'Selezionare un utente di messaggio',
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
                labelWidth: 80,
                margin: '0 0 0 4px',
                store: storeUsers,
                queryMode: 'local',
                listeners: {
                    change: function (combo, value) {
                        Ext.getCmp('LogsGrid').getStore().getProxy().extraParams.user = value;
                    }
                }
            });

            var MailsCombo = Ext.create('Ext.form.field.ComboBox', {
                fieldLabel: 'Casella Mail',
                emptyText: 'Selezionare una casella di posta',
                //  renderTo: 'TipoCombo',
                id: 'MailsCombo',
                displayField: 'emailAddress',
                valueField: 'emailAddress',
                ctCls: 'LabelBlackBold',
                editable: false,
                tpl: Ext.create('Ext.XTemplate',
              '<ul class="x-list-plain"><tpl for=".">',
                  '<li role="option" class="x-boundlist-item">{emailAddress}</li>',
              '</tpl></ul>'
          ),
                width: 365,
                labelWidth: 80,
                margin: '0 0 0 4px',
                store: storeMails,
                queryMode: 'local',
                listeners: {
                    change: function (combo, value) {
                        Ext.getCmp('LogsGrid').getStore().getProxy().extraParams.usermail = value;
                    }
                }
            });


            var AppCodeCombo = Ext.create('Ext.form.field.ComboBox', {
                fieldLabel: 'Codice Appl.',
                emptyText: 'Selezionare un codice applicazione',
                //  renderTo: 'TipoCombo',
                id: 'CodiceAppCombo',
                displayField: 'DESCR',
                valueField: 'APP_CODE',
                ctCls: 'LabelBlackBold',
                editable: false,
                tpl: Ext.create('Ext.XTemplate',
              '<ul class="x-list-plain"><tpl for=".">',
                  '<li role="option" class="x-boundlist-item">{DESCR}</li>',
              '</tpl></ul>'
          ),
                width: 405,
                labelWidth: 120,
                margin: '0 0 0 4px',
                store: storeAppCode,
                queryMode: 'local',
                listeners: {
                    change: function (combo, value) {
                        Ext.getCmp('LogsGrid').getStore().getProxy().extraParams.appcode = value;
                    }
                }
            });


            // cognome field
            //var userTextField = Ext.create('Ext.form.TextField', {
            //    id: 'UserField',
            //    fieldLabel: 'Utente',
            //    // renderTo: 'CognomeDiv',
            //    labelWidth: 60,
            //    emptyText: 'Indicare un utente',
            //    width: 350,
            //    padding: '0 0 0 10',
            //    listeners: {
            //        change: function (textField, value, oldvalue) {
            //            Ext.getCmp('LogsGrid').getStore().getProxy().extraParams.user = value;
            //        }
            //    }
            //});      
           

           

            //var UserMailTextField = Ext.create('Ext.form.TextField', {
            //    id: 'UserMailField',
            //    fieldLabel: 'Casella Mail',
            //    labelWidth: 80,
            //    maxlength: 50,
            //    enforceMaxLength: true,
            //    maxLengthText: 'Massimo 50 caratteri',
            //    width: 365,
            //    margin: '0 0 0 4px',
            //    emptyText: '',
            //    listeners: {
            //        change: function (textField, value, oldvalue) {
            //            Ext.getCmp('LogsGrid').getStore().getProxy().extraParams.usermail = value;
            //        }
            //    }
            //});

            var dataInizioTextField = Ext.create('Ext.form.DateField', {
                id: 'DataInizioField',
                format: 'd/m/Y',
                fieldLabel: 'Data Inizio',
                value: '01/10/2010',
                labelWidth: 70,
                margin: '0 0 0 4px',
                width: 190,
                // renderTo: 'DataInizioDiv',
                listeners: {
                    change: function (dateField, value) {
                        Ext.getCmp('LogsGrid').getStore().getProxy().extraParams.datainizio = value;
                    }
                }
            });
            var dataFineTextField = Ext.create('Ext.form.DateField', {
                id: 'DataFineField',
                format: 'd/m/Y',
                fieldLabel: 'Data Fine',
                labelWidth: 70,
                width: 190,
                margin: '0 0 0 4px',
                // renderTo: 'DataFineDiv',
                change: function (dateField, value) {
                    Ext.getCmp('LogsGrid').getStore().getProxy().extraParams.datafine = value;
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
                    Ext.getCmp('DataInizioField').setValue('');
                    Ext.getCmp('DataFineField').setValue('');
                    Ext.getCmp('MailsCombo').setValue('');
                    Ext.getCmp('UsersCombo').setValue('');
                    Ext.getCmp('TipoCombo').setValue('');
                    Ext.getCmp('CodiceLogCombo').setValue('');
                    Ext.getCmp('CodiceAppCombo').setValue('');
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
                    store.getProxy().setExtraParam("usermail", Ext.getCmp('MailsCombo').getValue());
                    store.getProxy().setExtraParam("user", Ext.getCmp('UsersCombo').getValue());
                    store.getProxy().setExtraParam("codiceapp", Ext.getCmp('CodiceAppCombo').getValue());
                    store.getProxy().setExtraParam("codicelog", Ext.getCmp('CodiceLogCombo').getValue());                  
                    store.getProxy().setExtraParam("datainizio", Ext.getCmp('DataInizioField').getValue());
                    store.getProxy().setExtraParam("datafine", Ext.getCmp('DataFineField').getValue());
                    Ext.getCmp("LogsGrid").store.loadPage(1);
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
                        items: [LogCodeCombo, AppCodeCombo, UsersCombo]
                    }, {
                        xtype: 'toolbar',
                        dock: 'top',
                        items: [
                MailsCombo, dataInizioTextField, dataFineTextField, buttonFilter, buttonReset]
                    }
                ]
            });

            Ext.define('LogsModel', {
                extend: 'Ext.data.Model',
                fields: [
                    'ID',
                     { name: 'IdLog'}, 'LOG_UID', 'APP_CODE', 'LOG_CODE', 'USER_ID', 'USER_MAIL',
                     'LOG_DATE', 'LOG_DETAILS', 'LOG_LEVEL', 'OBJECT_ID'                    
                ],
                idProperty: 'IdLog'
            });

            var urlApiGetLogs = '/GestionePEC/api/LogsServiceController/GetLogs';           


            function changeLogCode(value,p)
            {
                var descr = 'Non codificato';
                switch (value) {
                    case 'CMN_SVC_001':
                        descr = 'SALVATAGGIO EMAIL IN ARCHIVIO';
                        break;
                    case 'CRB_ARK':
                        descr = 'ARCHIVIAZIONE EMAIL E SPOSTAMENTO EMAIL IN ARCHIVIO';
                        break;
                    case 'CRB_DEL':
                        descr = 'CANCELLAZIONE EMAIL E SPOSTAMENTO NEL CESTINO';
                        break;
                    case 'CRB_LOG':
                        descr = 'AUTENTICAZIONE';
                        break;
                    case 'CRB_MOV':
                        descr = 'SPOSTAMENTO EMAIL IN FOLDER ';
                        break;
                    case 'CRB_RIP':
                        descr = 'RIPRISTINO EMAIL IN OUTBOX DA CESTINO O ARCHIVIO';
                        break;
                    case 'CRB_RIPC':
                        descr = 'RIPRISTINO EMAIL DA CESTINO';
                        break;
                    case 'CRB_RIPK':
                        descr = 'RIPRISTINO EMAIL DA ARCHIVIO';
                        break;
                   default:
                        descr = '' + value;
                        break;                
                }
                return descr;

            }    
            
            
         

         
            var store = Ext.create('Ext.data.Store', {
                pageSize: 10,
                id: 'StoreLogs',
                model: 'LogsModel',
                remoteSort: true,
                beforeload: function () {
                    store.getProxy().extraParams = {}; // clear all previous                   
                },
                proxy: {
                    // load using script tags for cross domain, if the data in on the same domain as
                    // this page, an HttpProxy would be better
                    type: 'ajax',
                    url: urlApiGetLogs,
                    reader: {
                        type: 'json',
                        rootProperty: 'LogsList',
                        //callbackKey: 'theCallbackFunction',
                        totalProperty: 'Totale'
                    },
                    // sends single sort as multi parameter
                    simpleSortMode: true
                },
                sorters: [{
                    property: 'LOG_DATE',
                    direction: 'DESC'
                }]
            });

       

            var grid = Ext.create('Ext.grid.Panel', {
                //width: 900,
                //height: 300,
                store: store,
                id: 'LogsGrid',
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
                    id: 'IdLog',
                    text: 'Id',
                    dataIndex: 'IdLog',
                    hidden: true
                }, {
                    id: 'LOG_UID',
                    text: 'LOG_UID',
                    dataIndex: 'LOG_UID',
                    hidden: true
                }, {
                    id: "APP_CODE",
                    text: "Codice App",
                    dataIndex: 'APP_CODE',
                    width: 150,
                    align: 'center',
                    sortable: true
                },{                   
                    id: 'LOG_CODE',
                    text: "Codice Log",
                    dataIndex: 'LOG_CODE',
                    width: 150,
                    sortable: false,
                    width:135,
                    hidden: false,
                },{
                    text: "Utente",
                    dataIndex: 'USER_ID',
                    width: 225,
                    align: 'center',
                    hidden:false,
                    sortable: true
                }, {
                    text: "Mail",
                    dataIndex: 'USER_MAIL',
                    width: 300,
                    align: 'center',
                    sortable: true
                }, {
                    text: "Data",
                    dataIndex: 'LOG_DATE',
                    width: 200,
                    align: 'center',
                    sortable: true
                }, {
                    text: "Dettagli",
                    dataIndex: 'LOG_DETAILS',
                    width: 400,
                    align: 'center',
                    hidden: false,
                    sortable: true
                },{
                    text: "Livello",
                    dataIndex: 'LOG_LEVEL',
                    width: 60,
                    align: 'center',
                    sortable: true,
                    hidden:true
                },{
                    text: "Oggetto",
                    dataIndex: 'OBJECT_ID',
                    width: 60,
                    align: 'center',
                    sortable: true,
                    hidden:true
                }],
                listeners: {                   
                    selectionchange: function (model, records) {                        
                        HideError();
                        if (rec) {   
                        }
                    }
                },
                // paging bar on the bottom
                bbar: Ext.create('Ext.toolbar.Paging', {
                    store: store,
                    displayInfo: true,
                    autoWidth: true,
                    id: 'PagingRichieste',
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
                renderTo: 'logs-grid',
                frame: true,
                resizable: true,
                title: 'Elenco Logs',
                //  width: 1205,
                // minWidth: 1220,
                height: 590,
                border: false,
                layout: 'anchor',
                items: [
                   toolBar, grid, summaryPanel]
            });

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


            store.getProxy().setExtraParam("usermail", Ext.getCmp('MailsCombo').getValue());
            store.getProxy().setExtraParam("user", Ext.getCmp('UsersCombo').getValue());
            store.getProxy().setExtraParam("codiceapp", Ext.getCmp('CodiceAppCombo').getValue());
            store.getProxy().setExtraParam("codicelog", Ext.getCmp('CodiceLogCombo').getValue());
            store.getProxy().setExtraParam("datainizio", Ext.getCmp('DataInizioField').getValue());
            store.getProxy().setExtraParam("datafine", Ext.getCmp('DataFineField').getValue());
            // trigger the data store load           
            store.loadPage(1);

            var summaryPanel = Ext.create('Ext.panel.Panel', {
                bodyPadding: 5,  // Don't want content to crunch against the borders
                // minWidth: 1210,
                collapsible: true,
                id: 'panelSummary',
                title: 'Messaggi',
                items: [{
                    xtype: 'displayfield',
                    fieldLabel: '',
                    id: 'labelSummary'
                }],
                renderTo: 'summary'
            });

        });
    </script>
</asp:Content>

