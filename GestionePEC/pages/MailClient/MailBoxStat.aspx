<%@ Page Title="Statistiche" Language="C#" Theme="Delta" MasterPageFile="~/Master/Mail.Master" AutoEventWireup="true" CodeBehind="MailBoxStat.aspx.cs" Inherits="GestionePEC.pages.MailClient.MailBoxStat" %>
<%@ Register TagPrefix="uc" TagName="Ricerca" Src="~/controls/RicercaStat.ascx" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
 <div class="content-panel-borderless">
        <div class="header-panel-blue">
            <div class="header-title">
                <div class="header-text-left">
                    <label>
                        Statische Lavorazioni Mail</label>
                </div>
                <div class="header-text-right">
                </div>
            </div>
        </div>
            <div id="divStatPanel"></div>
     </div>
 <script type="text/javascript">

        Ext.Loader.setConfig({ enabled: true });
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

            var readerUsers = new Ext.data.JsonReader({
                idProperty: 'UserName',
                model: 'UsersModel',
                messageProperty: 'Message',
                type: 'json',
                rootProperty: 'ListUtenti',
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

            Ext.Ajax.setTimeout(150000); // 150 seconds

            var msgBox = Ext.MessageBox.show({
                msg: 'Caricamento accounts in corso, attendere prego...',
                progressText: 'Caricamento...',
                id: 'MessageLoading',
                width: 350,
                wait: true,
                waitConfig:
                {
                    duration: 50000,
                    increment: 15,
                    text: 'Caricamento...',
                    scope: this
                }
            });

            var storeMails = Ext.create('Ext.data.Store', {
                autoLoad: true,
                storeId: 'storeMails',
                model: 'MailsModel',
                reader: readerMails,
                proxy:
                   {
                       type: 'ajax',
                       url: '/GestionePEC/api/LogsServiceController/getMails',
                       reader: readerMails
                   },
                //  restful: true,
                listeners: {
                    load: function (store, records, successfull, eOpts) {
                        if (successfull) {
                            msgBox.hide();
                        }
                        else {
                            msgBox.hide();
                            Ext.MessageBox.alert('Errore elaborazine', 'Problema caricamento accounts posta elettronica.');
                        }
                    },
                    exception: function () {
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
                width: 550,
                labelWidth: 120,
                allowBlank: false,
                margin: '5 0 0 4px',
                store: storeMails,
                queryMode: 'local',
                listeners: {
                    change: function (combo, value) {
                      //  Ext.getCmp('GridStat').getStore().getProxy().extraParams.mail = value;
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
                allowBlank: true,
                tpl: Ext.create('Ext.XTemplate',
              '<ul class="x-list-plain"><tpl for=".">',
                  '<li role="option" class="x-boundlist-item">{UserName}</li>',
              '</tpl></ul>'
          ),
                width: 550,
                labelWidth: 120,
                margin: '5 0 0 4px',
                store: storeUsers,
                queryMode: 'local',
                listeners: {
                    change: function (combo, value) {
                     //   Ext.getCmp('GridStat').getStore().getProxy().extraParams.utente = value;
                    }
                }
            });

            var dataInizioTextField = Ext.create('Ext.form.DateField', {
                id: 'DataInizioField',
                format: 'd/m/Y',
                fieldLabel: 'Data Inizio',
                value: '01/10/2010',
                labelWidth: 120,
                margin: '5 0 0 4px',
                width: 240,
                // renderTo: 'DataInizioDiv',
                listeners: {
                    change: function (dateField, value) {
                     //   Ext.getCmp('GridStat').getStore().getProxy().extraParams.datainizio = value;
                    }
                }
            });
            var dataFineTextField = Ext.create('Ext.form.DateField', {
                id: 'DataFineField',
                format: 'd/m/Y',
                fieldLabel: 'Data Fine',
                labelWidth: 120,
                width: 240,
                margin: '5 0 0 4px',
                value: new Date(),
                maxDate: new Date(),
                // renderTo: 'DataFineDiv',
                change: function (dateField, value) {
                   // Ext.getCmp('GridStat').getStore().getProxy().extraParams.datafine = value;
                }
            });


            var tabStatLavorazioni = Ext.create('Ext.form.Panel',
           {
               title: 'Statistica Lavorazioni',
               layout: 'vbox',
               id: 'formStatLavorazioni',
               autoHeight: false,
               width:800,
               height: 250,
               items: [
                   UsersCombo, MailsCombo, dataInizioTextField, dataFineTextField
               ], buttons: [{
                   text: 'Ricerca',
                   id: 'btnRicercaStatLavorazioni',
                   formBind: true,
                   xtype: 'button',
                   listeners:
                             {
                                 click: function () {
                                     ajaxHandlerStatLavorazioni();
                                 }
                             }
               }, {
                   text: 'Stampa',
                   id: 'btnStampatatLavorazioni',
                   formBind: true,
                   xtype: 'button',
                   listeners:
                             {
                                 click: function () {
                                     ajaxHandlerStampaLavorazioni();
                                 }
                             }
               }]
           });

            var urlGetStatLavorazioni = '/GestionePEC/api/StatServiceController/getStatByUserMail';
            function ajaxHandlerStatLavorazioni() {
                var form = Ext.getCmp('formStatLavorazioni');
                HideError();
                form.getForm().submit({
                    clientValidation: true,
                    url: urlGetStatLavorazioni,
                    params: {
                        utente: Ext.getCmp('formStatLavorazioni').items.get(0).value, mail: Ext.getCmp('formStatLavorazioni').items.get(1).value,
                        dt: Ext.getCmp('formStatLavorazioni').items.get(2).value, df: Ext.getCmp('formStatLavorazioni').items.get(3).value,
                        page: 1, start: 1, limit: 5, sort: '',dir: ''
                    },
                    waitTitle: 'Attendere prego',
                    method: 'GET',
                    success: function (form, action) {
                        var form = Ext.getCmp('formStatLavorazioni');
                        var grid = Ext.getCmp('GridStat');
                        var jsonDataTotale = action.result.Totale;
                        if (action.result.success == "false") {
                            var form = Ext.getCmp('formStatLavorazioni');
                            // form.el.unmask();
                            ManageError("Errore nell'interrogazione dei sistemi centrali dettagli :" + action.result.Message);
                        }
                        else {
                            if (jsonDataTotale == "0") {
                                grid.setStyle("display", "none");
                                ManageError("Nessun ritrovamento");
                            }
                            else if (jsonDataTotale != "0") {
                                grid.store.loadData(action.result.ElencoStat);
                                grid.store.pageSize = 5;
                                grid.store.totalCount = jsonDataTotale;
                                Ext.ComponentQuery.query('pagingtoolbar')[0].onLoad();
                                grid.show();
                                HideError();
                            }
                            // form.el.unmask();
                        }
                    },
                    failure: function (form, action) {
                        var form = Ext.getCmp('formStatLavorazioni');
                        //  form.el.unmask();
                        Ext.Msg.alert('Errore', Ext.decode(action.response.responseText).Message);
                    }
                });
            }

            var urlGetStampaLavorazioni = '/GestionePEC/api/StatServiceController/getStampaByUserMail';
            function ajaxHandlerStampaLavorazioni() {
                var form = Ext.getCmp('formStatLavorazioni');
                var formData = form.getForm().getFieldValues();
                form.standardSubmit = true;
                form.getForm().submit({
                    clientValidation: true,
                    url: urlGetStampaLavorazioni,
                    target: '_blank',
                    params: {
                        utente: Ext.getCmp('formStatLavorazioni').items.get(0).value, mail: Ext.getCmp('formStatLavorazioni').items.get(1).value,
                        dt: Ext.getCmp('formStatLavorazioni').items.get(2).value, df: Ext.getCmp('formStatLavorazioni').items.get(3).value
                    },
                    waitTitle: 'Attendere prego',
                    failure: function (form, action) {
                        var form = Ext.getCmp('formStatLavorazioni');
                        //  form.el.unmask();
                        Ext.Msg.alert('Errore', Ext.decode(action.response.responseText).Message);
                    }
                })
            };


            var tabContainer = Ext.create('Ext.tab.Panel', {
                id: 'StatPanel',
                activeTab: 0,
                height: 290,
                width:1480,
                autoHeight: true,
                header: false,
                bodyStyle: 'padding:5px',
                items: [
                  tabStatLavorazioni
                ]
            });

            var summaryPanel = Ext.create('Ext.panel.Panel', {
                bodyPadding: 5,  // Don't want content to crunch against the borders
                // width: 1205,
                collapsible: true,
                hidden: true,
                //   region: south,
                id: 'panelSummary',
                title: 'Messaggi',
                items: [{
                    xtype: 'displayfield',
                    fieldLabel: '',
                    id: 'labelSummary'
                }]
            });

            Ext.define('statGridModel', {
                extend: 'Ext.data.Model',
                fields: [
                    'User', 'Operazioni', 'Account']
            });

            var store = Ext.create('Ext.data.Store', {
                model: 'statGridModel',
                pageSize: 5,
                fields: [
                   { name: 'User' },
                   { name: 'Operazioni' },
                   { name: 'Account' }
                ],
                proxy: {
                    type: 'ajax',
                    url: urlGetStatLavorazioni,
                    reader: {
                        type: 'json',
                        rootProperty: 'ElencoStat',
                        enablePaging: true,
                        params: {
                            utente: Ext.getCmp('formStatLavorazioni').items.get(0).value,
                            mail: Ext.getCmp('formStatLavorazioni').items.get(1).value,
                            dt: Ext.getCmp('formStatLavorazioni').items.get(2).value,
                            df: Ext.getCmp('formStatLavorazioni').items.get(3).value
                        },
                        pageSize: 5,
                        totalProperty: 'Totale'
                    }
                },
                //autoLoad: true,
            });

            // Prima griglia risultato
            var gridStat = Ext.create('Ext.grid.Panel', {
                //width: 900,
                //height: 300,
                store: store,
                id: 'GridStat',
                hidden: 'true',
                // region: center,
                loadMask: true,
                viewConfig: {
                    forceFit: true,
                    loadingText: "Caricamento griglia in corso...."
                },
                split: true,
                //  region: 'north',
                // grid columns
                columns: [{
                    id: 'UtenteText',
                    text: 'Casella Posta',
                    dataIndex: 'User',
                    align: 'center',
                    width: 490,
                    sortable: true
                },{
                    id: 'AccountText',
                    text: "Account",
                    dataIndex: 'Account',
                    sortable: true,
                    width: 550,
                    align: 'center'
                }, {
                    id: 'OperazioniText',
                    text: 'Operazioni',
                    dataIndex: 'Operazioni',
                    align: 'center',
                    width: 260
                }
                ],
                listeners: {
                    scope: this,
                    selectionchange: function (model, records) {
                        var rec = records[0];
                        if (rec) {
                            // var secondGrid = Ext.getCmp('secondGrid');
                        }
                    }
                },
                // paging bar on the bottom
                bbar: Ext.create('Ext.toolbar.Paging', {
                    store: store,
                    displayInfo: true,
                    autoWidth: true,
                    id: 'PagingfirstGrid',
                    displayMsg: 'Visualizzazione righe {0} - {1} of {2}',
                    emptyMsg: "Nessun record da visualizzare",
                    listeners: {
                        afterLayout: function (asd) {
                            this.container.dom.style.width = "";
                            this.el.dom.style.widt3h = "";
                        },
                        beforechange: function (page, currentPage) {
                            //--- Get Proxy ------//
                            var myProxy = this.store.getProxy();
                            //--- Define Your Parameter for send to server ----//
                            myProxy.params = {
                                sesso: '',
                                nome: '',
                                cognome: '',
                                annoda: '',
                                annoa: ''

                            };
                            //--- Set value to your parameter  ----//
                            myProxy.setExtraParam('utente', Ext.getCmp('formStatLavorazioni').items.get(0).value);
                            myProxy.setExtraParam('mail', Ext.getCmp('formStatLavorazioni').items.get(1).value);
                            myProxy.setExtraParam('dt', Ext.getCmp('formStatLavorazioni').items.get(2).value);
                            myProxy.setExtraParam('df', Ext.getCmp('formStatLavorazioni').items.get(3).value);
                        }

                    }
                })
            });

            Ext.create('Ext.Panel', {
                renderTo: 'divStatPanel',
                title: '',
                // width: 1225,
                border: false,
                autoHeight: true,
               // height: 630,
                layout: {
                    pack: 'start',
                    align: 'stretch'
                },
                items: [
                   tabContainer,
                   gridStat,
                   summaryPanel
                ]
            });

            
        });
     </script>
    </asp:Content>