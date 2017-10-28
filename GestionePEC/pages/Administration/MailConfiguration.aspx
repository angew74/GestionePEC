<%@ Page Title="Configurazione Caselle mail" Language="C#" Theme="Delta" MasterPageFile="~/Master/Mail.Master" AutoEventWireup="true" CodeBehind="MailConfiguration.aspx.cs" Inherits="GestionePEC.pages.Administration.MailConfiguration" %>

<%@ Register Src="~/Controls/MailBoxNavigator.ascx" TagName="MailNav" TagPrefix="mail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="WestContentPlaceHolder" runat="server">
    <mail:MailNav ID="Navigator" runat="server" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div class="content-panel-borderless">
        <div class="header-panel-blue">
            <div class="header-title">
                <div class="header-text-left">
                    <label>
                        Configurazione Caselle</label>
                </div>
                <div class="header-text-right">
                </div>
            </div>
        </div>
        <div id="container" class="control-main">
            <div class="control-body-gray">
                <div id="GridUtentiMail"></div>
                <div id="summary"></div>
            </div>
            <div class="control-body-gray">
                <div id="EmailField"></div>
                <div id="IdEmailField"></div>
            </div>

            <div id="pnlElencoUtenti" runat="server" style="display: none">
                <div class="body-panel">
                    <div id="UsersManagement"></div>
                </div>
            </div>
            <div id="pnlAdmin" runat="server" style="display: none">
                <div class="body-panel">
                    <div id="AdminsManagement"></div>
                </div>
            </div>
            <div id="pnlGestioneFolders" runat="server" style="display: none">
                <div class="body-panel">
                    <div id="FoldersManagement"></div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function check(txt, id) {
            Ext.getCmp('emailField').setValue(txt);
            Ext.getCmp('IdemailField').setValue(id);
            //   LoadFolders(txt);
            document.getElementById('MainContentPlaceHolder_pnlGestioneFolders').style.display = 'block';
            document.getElementById('MainContentPlaceHolder_pnlAdmin').style.display = 'none';
            document.getElementById('MainContentPlaceHolder_pnlElencoUtenti').style.display = 'none';
            //  Ext.getCmp('folderselection-field').store.load();
            Ext.getCmp('folderselection-field').hide();
            Ext.getCmp('folderselection-field').show();
        }
        function checkUser(txt, id) {
            Ext.getCmp('emailField').setValue(txt);
            Ext.getCmp('IdemailField').setValue(id);
            //   LoadFolders(txt);
            document.getElementById('MainContentPlaceHolder_pnlGestioneFolders').style.display = 'none';
            document.getElementById('MainContentPlaceHolder_pnlAdmin').style.display = 'none';
            document.getElementById('MainContentPlaceHolder_pnlElencoUtenti').style.display = 'block';
            //  Ext.getCmp('folderselection-field').store.load();
            Ext.getCmp('userselection-field').hide();
            Ext.getCmp('userselection-field').show();
        }

        function checkAdmin(txt, id) {
            Ext.getCmp('emailField').setValue(txt);
            Ext.getCmp('IdemailField').setValue(id);
            //   LoadFolders(txt);
            document.getElementById('MainContentPlaceHolder_pnlGestioneFolders').style.display = 'none';
            document.getElementById('MainContentPlaceHolder_pnlAdmin').style.display = 'block';
            document.getElementById('MainContentPlaceHolder_pnlElencoUtenti').style.display = 'none';
            //  Ext.getCmp('folderselection-field').store.load();
            Ext.getCmp('adminselection-field').hide();
            Ext.getCmp('adminselection-field').show();
        }

    </script>
    <script type="text/javascript">
        Ext.Loader.setPath('Ext.ux', '../../ExtJS6/ux');
        Ext.require([
       'Ext.form.Panel',
       'Ext.ux.form.MultiSelect',
       'Ext.ux.form.ItemSelector',
       'Ext.tip.QuickTipManager'
        ]);


        Ext.onReady(function () {


            Ext.tip.QuickTipManager.init();

            Ext.create('Ext.form.field.Display', {
                fieldLabel: 'Indirizzo email',
                name: 'email',
                id: 'emailField',
                style: {
                    color: 'maroon',
                    fontstyle: 'italic',
                    fontweight:'bold'
                },
                vtype: 'email', // applies email validation rules to this field
                renderTo: 'EmailField'
            });
            Ext.create('Ext.form.field.Display', {
                fieldLabel: 'Id email',
                name: 'idmail',
                id: 'IdemailField',
                vtype: 'email', // applies email validation rules to this field
                renderTo: 'IdEmailField',
                hidden: true
            });

            var data = [];

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


            // gestione abilitazione cartelle
            Ext.define('FoldersModel', {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'IdNome', type: 'int' },
                    { name: 'Nome', type: 'string' }
                ]
            });

            var readerFolders = new Ext.data.JsonReader({
                idProperty: 'IdNome',
                model: 'FoldersModel',
                messageProperty: 'Message',
                type: 'array',
                rootProperty: 'FoldersList',
                totalProperty: 'Totale'
            });

            var storeFolders = Ext.create('Ext.data.ArrayStore', {
                autoLoad: true,
                storeId: 'storeFolders',
                model: 'FoldersModel',
                reader: readerFolders,
                proxy:
                   {
                       type: 'ajax',
                       url: '/GestionePEC/api/FolderController/GetAllFolders',
                       reader: readerFolders
                   },
                //  restful: true,
                listeners: {
                    load: function (s, r, o) {
                        // Ext.getCmp('folderselection-field').data = s.data.items;
                        Ext.getCmp("folderselection-field").bindStore(s.data);
                    },
                    exception: function () {
                    }
                }
            });

            var storeSelectedFolders = Ext.create('Ext.data.ArrayStore', {
                autoLoad: false,
                storeId: 'storeSelectedFolders',
                model: 'FoldersModel',
                reader: readerFolders,
                fields: [
                    { name: 'IdNome', type: 'int' },
                    { name: 'Nome', type: 'string' }
                ],
                proxy:
                   {
                       type: 'ajax',
                       url: '/GestionePEC/api/FolderController/GetFoldersAbilitate?mail=',
                       reader: readerFolders
                   },
                //  restful: true,
                listeners: {
                    load: function (s, r, o) {
                        data = s.data;
                        var a = [];
                        if (data != null & data.length > 0) {
                            for (var key = 0; key < data.items.length; key++) {
                                a.push(data.items[key].data["IdNome"]);
                            }
                            // Ext.getCmp('folderselection-field').toField.fieldDefaults = a;
                            Ext.getCmp('folderselection-field').toField.bindStore(a);
                        }
                        Ext.getCmp('folderselection-field').setValue(a);
                    },
                    exception: function () {
                    }
                }
            });

            // tool bar e docked items

            function createDockedItemsFolders(fieldId) {
                return [
                {
                    xtype: 'toolbar',
                    dock: 'top',
                    id: 'DockedItemsFolders',
                    items: {
                        text: 'Opzioni',
                        menu: [{
                            text: 'Cartelle di sistema',
                            handler: function () {
                                Ext.getCmp(fieldId).setValue(['1', '2', '3']);
                            }
                        }, {
                            text: 'Toggle enabled',
                            checked: true,
                            checkHandler: function (item, checked) {
                                Ext.getCmp(fieldId).setDisabled(!checked);
                            }
                        }]
                    }
                }, {
                    xtype: 'toolbar',
                    dock: 'bottom',
                    ui: 'footer',
                    defaults: {
                        minWidth: 75
                    },
                    items: ['->', {
                        text: 'Rimuovi',
                        handler: function () {
                            var field = Ext.getCmp(fieldId);
                            if (!field.disabled) {
                                field.clearValue();
                            }
                        }
                    }, {
                        text: 'Reset',
                        handler: function () {
                            Ext.getCmp(fieldId).up('form').getForm().reset();
                        }
                    }, {
                        text: 'Salva',
                        handler: function () {
                            var form = Ext.getCmp(fieldId).up('form').getForm();
                            form.getValues(true);
                            if (form.isValid()) {
                                var values = form.getValues(true);
                                Ext.Ajax.request({
                                    url: "/GestionePEC/api/FolderController/AbilitaFolders?idemail=" + Ext.getCmp('IdemailField').getValue() + "&" + values,
                                    method: 'GET',
                                    headers: { 'Content-Type': 'application/json' },
                                    success: function (conn, response, options, eOpts) {
                                        var result = Ext.decode(conn.responseText);
                                        if (result.success) {
                                            Ext.Msg.alert('Complimenti!', 'Cartelle abilitate.');
                                        } else {
                                            ManageError(result.msg);
                                        }
                                    },
                                    failure: function (conn, response, options, eOpts) {
                                        // TODO get the 'msg' from the json and display it                                        
                                        ManageError(conn.responseText);
                                    }
                                });
                            }
                        }
                    }]
                }];
            }

            // Box Abilitazione e disabilitazione

            var isForm = Ext.widget('form', {
                title: 'Gestione Cartelle',
                // width:00,
                bodyPadding: 10,
                height: 400,
                renderTo: 'FoldersManagement',
                layout: 'fit',
                id: 'SelectorCartelle',
                items: [{
                    xtype: 'itemselector',
                    name: 'itemselector',
                    id: 'folderselection-field',
                    anchor: '100%',
                    delimiter: ';',
                    width: 1000,
                    fieldLabel: 'Cartelle',
                    imagePath: '../../App_Themes/Delta/images/itemselector/',
                    store: storeFolders,
                    displayField: 'Nome',
                    valueField: 'IdNome',
                    value: [],
                    allowBlank: false,
                    hidden: true,
                    maxSelections: 10,
                    msgTarget: 'side',
                    fromTitle: 'Non abilitate',
                    toTitle: 'Abilitate',
                    selectionModel: 'single',
                    listeners: {
                        show: function () {
                            if (Ext.getCmp('emailField').rawValue != '') {
                                storeSelectedFolders.getProxy().setExtraParam("email", Ext.getCmp('emailField').getValue());
                                storeSelectedFolders.load();
                            }
                        },
                        itemclick: 'onItemsSelect'
                    }
                }],
                dockedItems: createDockedItemsFolders('folderselection-field')
            });


            // Gestione abilitazione utenti

            Ext.define('UsersModel', {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'UserId', type: 'int' },
                    { name: 'UserName', type: 'string' }
                ]
            });

            var readerUsers = new Ext.data.JsonReader({
                idProperty: 'UserId',
                model: 'UsersModel',
                messageProperty: 'Message',
                type: 'array',
                rootProperty: 'UsersList',
                totalProperty: 'Totale'
            });

            var storeUsers = Ext.create('Ext.data.ArrayStore', {
                autoLoad: true,
                storeId: 'storeUsers',
                model: 'UsersModel',
                reader: readerUsers,
                proxy:
                   {
                       type: 'ajax',
                       url: '/GestionePEC/api/UsersServiceController/GetAllUsers',
                       reader: readerUsers
                   },
                //  restful: true,
                listeners: {
                    load: function (s, r, o) {
                        // Ext.getCmp('folderselection-field').data = s.data.items;
                        Ext.getCmp("userselection-field").bindStore(s.data);
                    },
                    exception: function () {
                    }
                }
            });

            var storeSelectedUsers = Ext.create('Ext.data.ArrayStore', {
                autoLoad: false,
                storeId: 'storeSelectedUsers',
                model: 'UsersModel',
                reader: readerUsers,
                fields: [
                    { name: 'UserId', type: 'int' },
                    { name: 'UserName', type: 'string' }
                ],
                proxy:
                   {
                       type: 'ajax',
                       url: '/GestionePEC/api/UsersServiceController/GetUsersAbilitati?idmail=',
                       reader: readerUsers
                   },
                //  restful: true,
                listeners: {
                    load: function (s, r, o) {
                        data = s.data;
                        var a = [];
                        if (data != null & data.length > 0) {
                            for (var key = 0; key < data.items.length; key++) {
                                a.push(data.items[key].data["UserId"]);
                            }
                            // Ext.getCmp('folderselection-field').toField.fieldDefaults = a;
                            Ext.getCmp('userselection-field').toField.bindStore(a);
                        }
                        Ext.getCmp('userselection-field').setValue(a);
                    },
                    exception: function () {
                    }
                }
            });

            // tool bar e docked items

            function createDockedItemsUtenti(fieldId) {
                return [
                {
                    xtype: 'toolbar',
                    dock: 'top',
                    id: 'DockedItemsUtenti',
                    items: {
                        text: 'Opzioni',
                        menu: [{
                            text: 'Utenti di sistema',
                            handler: function () {
                                Ext.getCmp(fieldId).setValue(['1', '2', '3']);
                            }
                        }, {
                            text: 'Toggle enabled',
                            checked: true,
                            checkHandler: function (item, checked) {
                                Ext.getCmp(fieldId).setDisabled(!checked);
                            }
                        }]
                    }
                }, {
                    xtype: 'toolbar',
                    dock: 'bottom',
                    ui: 'footer',
                    defaults: {
                        minWidth: 75
                    },
                    items: ['->', {
                        text: 'Rimuovi',
                        handler: function () {
                            var field = Ext.getCmp(fieldId);
                            if (!field.disabled) {
                                field.clearValue();
                            }
                        }
                    }, {
                        text: 'Reset',
                        handler: function () {
                            Ext.getCmp(fieldId).up('form').getForm().reset();
                        }
                    }, {
                        text: 'Salva',
                        handler: function () {
                            var form = Ext.getCmp(fieldId).up('form').getForm();
                            form.getValues(true);
                            if (form.isValid()) {
                                var values = form.getValues(true);
                                Ext.Ajax.request({
                                    url: "/GestionePEC/api/UsersServiceController/AbilitaUsers?idemail=" + Ext.getCmp('IdemailField').getValue() + "&" + values,
                                    method: 'GET',
                                    headers: { 'Content-Type': 'application/json' },
                                    success: function (conn, response, options, eOpts) {
                                        var result = Ext.decode(conn.responseText);
                                        if (result.success) {
                                            Ext.Msg.alert('Complimenti!', 'Utenti abilitati.');
                                        } else {
                                            ManageError(result.msg);
                                        }
                                    },
                                    failure: function (conn, response, options, eOpts) {
                                        // TODO get the 'msg' from the json and display it                                        
                                        ManageError(conn.responseText);
                                    }
                                });
                            }
                        }
                    }]
                }];
            }

            // Box Abilitaazione e disabilitazione

            var isForm = Ext.widget('form', {
                title: 'Gestione Utenti',
                // width:00,
                bodyPadding: 10,
                height: 400,
                renderTo: 'UsersManagement',
                layout: 'fit',
                id: 'SelectorUtenti',
                items: [{
                    xtype: 'itemselector',
                    name: 'itemselector',
                    id: 'userselection-field',
                    anchor: '100%',
                    delimiter: ';',
                    width: 1000,
                    fieldLabel: 'Utenti',
                    imagePath: '../../App_Themes/Delta/images/itemselector/',
                    store: storeFolders,
                    displayField: 'UserName',
                    valueField: 'UserId',
                    value: [],
                    allowBlank: false,
                    hidden: true,
                    maxSelections: 10,
                    msgTarget: 'side',
                    fromTitle: 'Non abilitati',
                    toTitle: 'Abilitati',
                    selectionModel: 'multi',
                    listeners: {
                        show: function () {
                            if (Ext.getCmp('IdemailField').rawValue != '') {
                                storeSelectedUsers.getProxy().setExtraParam("idsender", Ext.getCmp('IdemailField').getValue());
                                storeSelectedUsers.load();
                            }
                        },
                        itemclick: 'onItemsSelect'
                    }
                }],
                dockedItems: createDockedItemsUtenti('userselection-field')
            });


            // Gestione abilitazione Amministratori

            Ext.define('AdminsModel', {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'UserId', type: 'int' },
                    { name: 'UserName', type: 'string' }
                ]
            });

            var readerAdmins = new Ext.data.JsonReader({
                idProperty: 'UserId',
                model: 'AdminsModel',
                messageProperty: 'Message',
                type: 'array',
                rootProperty: 'UsersList',
                totalProperty: 'Totale'
            });

            var storeAdmins = Ext.create('Ext.data.ArrayStore', {
                autoLoad: true,
                storeId: 'storeAdmins',
                model: 'AdminsModel',
                reader: readerAdmins,
                proxy:
                   {
                       type: 'ajax',
                       url: '/GestionePEC/api/UsersServiceController/GetAllUsers',
                       reader: readerUsers
                   },
                //  restful: true,
                listeners: {
                    load: function (s, r, o) {
                        // Ext.getCmp('folderselection-field').data = s.data.items;
                        Ext.getCmp("adminselection-field").bindStore(s.data);
                    },
                    exception: function () {
                    }
                }
            });

            var storeSelectedAdmins = Ext.create('Ext.data.ArrayStore', {
                autoLoad: false,
                storeId: 'storeSelectedAdmins',
                model: 'AdminsModel',
                reader: readerAdmins,
                fields: [
                    { name: 'UserId', type: 'int' },
                    { name: 'UserName', type: 'string' }
                ],
                proxy:
                   {
                       type: 'ajax',
                       url: '/GestionePEC/api/UsersServiceController/GetAdminsAbilitati?idmail=',
                       reader: readerUsers
                   },
                //  restful: true,
                listeners: {
                    load: function (s, r, o) {
                        data = s.data;
                        var a = [];
                        if (data != null & data.length > 0) {
                            for (var key = 0; key < data.items.length; key++) {
                                a.push(data.items[key].data["UserId"]);
                            }
                            // Ext.getCmp('folderselection-field').toField.fieldDefaults = a;
                            Ext.getCmp('adminselection-field').toField.bindStore(a);
                        }
                        Ext.getCmp('adminselection-field').setValue(a);
                    },
                    exception: function () {
                    }
                }
            });

            // tool bar e docked items

            function createDockedItemsAdmins(fieldId) {
                return [
                {
                    xtype: 'toolbar',
                    id: 'DockedItemsAdmin',
                    dock: 'top',
                    items: {
                        text: 'Opzioni',
                        menu: [{
                            text: 'Amministratori di sistema',
                            handler: function () {
                                Ext.getCmp(fieldId).setValue(['1', '2', '3']);
                            }
                        }, {
                            text: 'Toggle enabled',
                            checked: true,
                            checkHandler: function (item, checked) {
                                Ext.getCmp(fieldId).setDisabled(!checked);
                            }
                        }]
                    }
                }, {
                    xtype: 'toolbar',
                    dock: 'bottom',
                    ui: 'footer',
                    defaults: {
                        minWidth: 75
                    },
                    items: ['->', {
                        text: 'Rimuovi',
                        handler: function () {
                            var field = Ext.getCmp(fieldId);
                            if (!field.disabled) {
                                field.clearValue();
                            }
                        }
                    }, {
                        text: 'Reset',
                        handler: function () {
                            Ext.getCmp(fieldId).up('form').getForm().reset();
                        }
                    }, {
                        text: 'Salva',
                        handler: function () {
                            var form = Ext.getCmp(fieldId).up('form').getForm();
                            form.getValues(true);
                            if (form.isValid()) {
                                var values = form.getValues(true);
                                form.submit({
                                    url: '/GestionePEC/api/UsersServiceController/AbilitaAdmins',
                                    method: 'POST',
                                    headers: { 'Content-Type': 'application/json' },
                                    params: { idemail: Ext.getCmp('IdemailField').getValue() },
                                    //  data: form.getValues(true),
                                    //  params: Ext.JSON.encode(form.getValues(true)),
                                    success: function (conn, response, options, eOpts) {
                                        var result = Ext.decode(conn.responseText);
                                        if (result.success) {
                                            Ext.Msg.alert('Complimenti!', 'Amministratori abilitati.');
                                        } else {
                                            ManageError(result.msg);
                                        }
                                    },
                                    failure: function (conn, response, options, eOpts) {
                                        // TODO get the 'msg' from the json and display it                                        
                                        ManageError(conn.responseText);
                                    }
                                });
                            }
                        }
                    }]
                }];
            }

            // Box Abilitaazione e disabilitazione

            var isForm = Ext.widget('form', {
                title: 'Gestione Amministratori',
                // width:00,
                bodyPadding: 10,
                height: 400,
                renderTo: 'AdminsManagement',
                layout: 'fit',
                id: 'SelectorAdmin',
                items: [{
                    xtype: 'itemselector',
                    name: 'itemselector',
                    id: 'adminselection-field',
                    anchor: '100%',
                    delimiter: ';',
                    width: 1000,
                    fieldLabel: 'Amministratori',
                    imagePath: '../../App_Themes/Delta/images/itemselector/',
                    store: storeFolders,
                    displayField: 'UserName',
                    valueField: 'UserId',
                    value: [],
                    allowBlank: false,
                    hidden: true,
                    maxSelections: 10,
                    msgTarget: 'side',
                    fromTitle: 'Non abilitati',
                    toTitle: 'Abilitati',
                    selectionModel: 'multi',
                    listeners: {
                        show: function () {
                            if (Ext.getCmp('IdemailField').rawValue != '') {
                                storeSelectedUsers.getProxy().setExtraParam("idsender", Ext.getCmp('IdemailField').getValue());
                                storeSelectedUsers.load();
                            }
                        },
                        itemclick: 'onItemsSelect'
                    }
                }],
                dockedItems: createDockedItemsAdmins('adminselection-field')
            });


            // model backend user gestione griglia
            Ext.define('BackendUsersModel', {
                extend: 'Ext.data.Model',
                fields: [
                    'MailSenderId',
                     'MailAccessLevel', 'Casella', 'DisplayName', 'EmailAddress', 'IsPec',
                     'Id', 'FlgManaged', 'UserId'
                ],
                idProperty: 'UserId'
            });

            var urlApiGetUsers = '/GestionePEC/api/EmailsController/GetMails';


            // gestione store utenti backend
            var store = Ext.create('Ext.data.Store', {
                pageSize: 5,
                id: 'StoreBackendUsers',
                model: 'BackendUsersModel',
                autoLoad: true,
                remoteSort: true,
                beforeload: function () {
                    store.getProxy().extraParams = {}; // clear all previous                   
                },
                proxy: {
                    // load using script tags for cross domain, if the data in on the same domain as
                    // this page, an HttpProxy would be better
                    type: 'ajax',
                    url: urlApiGetUsers,
                    reader: {
                        type: 'json',
                        rootProperty: 'ListBackendUsers',
                        //callbackKey: 'theCallbackFunction',
                        totalProperty: 'Totale'
                    },
                    // sends single sort as multi parameter
                    simpleSortMode: true
                },
                sorters: [{
                    property: 'EmailAddress',
                    direction: 'DESC'
                }]
            });

            function rendererISPEC(value, p, record) {
                var descr = 'Non codificato';
                switch (value) {
                    case true:
                        descr = 'SI';
                        break;
                    case false:
                        descr = 'NO';
                        break;
                }
                return descr;
            }

            function rendererAccessLevel(value, p, record) {
                var descr = 'Non codificato';
                switch (value) {
                    case 1:
                    case 2:
                        descr = 'SI';
                        break;
                    case 0:
                        descr = 'NO';
                        break;
                }
                return descr;
            }

            function rendererManaged(value, p, record) {
                var descr = 'Non codificato';
                switch (value) {
                    case 1:
                        descr = 'Parziale';
                        break;
                    case 0:
                        descr = 'NO';
                        break;
                    case 2:
                        descr = 'Completa';
                        break;
                }
                return descr;
            }

            var grid = Ext.create('Ext.grid.Panel', {

                //height: 300,
                store: store,
                id: 'BackendUsersGrid',
                region: 'north',
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
                        id: 'MailSenderIdField',
                        text: 'MailSenderId',
                        dataIndex: 'MailSenderId',
                        hidden: true
                    }, {
                        id: 'UserIdField',
                        text: 'UserId',
                        dataIndex: 'UserId',
                        hidden: true
                    }, {
                        id: "Pec",
                        text: "PEC",
                        dataIndex: 'IsPec',
                        width: 150,
                        align: 'center',
                        sortable: true,
                        renderer: rendererISPEC,
                    }, {
                        id: 'EmailAddressField',
                        text: "Email",
                        dataIndex: 'EmailAddress',
                        width: 450,
                        sortable: true,
                        width: 135,
                        hidden: false,
                        align: 'center',
                        flex: 1
                    }, {
                        text: "Amministratore",
                        dataIndex: 'MailAccessLevel',
                        width: 150,
                        align: 'center',
                        hidden: false,
                        sortable: true,
                        renderer: rendererAccessLevel
                    }, {
                        text: "Gestita",
                        dataIndex: 'FlgManaged',
                        width: 300,
                        align: 'center',
                        sortable: true,
                        renderer: rendererManaged
                    }, {
                        xtype: 'actioncolumn',
                        width: 50,
                        items: [{
                            tooltip: 'Gestisci Cartella',
                            getClass: function (v, metadata, r, rowIndex, colIndex, store) {
                                var isFilter = r.get('MailAccessLevel') == 0;
                                if (isFilter) {
                                    return "x-hidden-display";
                                } else {
                                    return "x-grid-center-icon";
                                }
                            },
                            icon: '../../App_Themes/Delta/Images/buttons/folder.png',
                            handler: function (grid, rowIndex, colIndex, item, e, record) {
                                var rec = grid.getStore().getAt(rowIndex);
                                check(rec.get('EmailAddress'), rec.get('UserId'));
                            }
                        }, {
                            tooltip: 'Gestisci Utenti',
                            getClass: function (v, metadata, r, rowIndex, colIndex, store) {
                                var isFilter = r.get('MailAccessLevel') == 0;
                                if (isFilter) {
                                    return "x-hidden-display";
                                } else {
                                    return "x-grid-center-icon";
                                }
                            },
                            icon: '../../App_Themes/Delta/Images/buttons/user.png',
                            handler: function (grid, rowIndex, colIndex, item, e, record) {
                                var rec = grid.getStore().getAt(rowIndex);
                                checkUser(rec.get('EmailAddress'), rec.get('UserId'));
                            }
                        }, {
                            tooltip: 'Gestisci Amministratori',
                            getClass: function (v, metadata, r, rowIndex, colIndex, store) {
                                var isFilter = r.get('MailAccessLevel') == 2;
                                if (!isFilter) {
                                    return "x-hidden-display";
                                } else {
                                    return "x-grid-center-icon";
                                }
                            },
                            icon: '../../App_Themes/Delta/Images/buttons/administrator.png',
                            handler: function (grid, rowIndex, colIndex, item, e, record) {
                                var rec = grid.getStore().getAt(rowIndex);
                                checkAdmin(rec.get('EmailAddress'), rec.get('UserId'));
                            }
                        }]
                    }],
                listeners: {
                    selectionchange: function (model, records) {
                        HideError();
                        if (records) {
                        }
                    }
                },
                // paging bar on the bottom
                bbar: Ext.create('Ext.toolbar.Paging', {
                    store: store,
                    displayInfo: true,
                    autoWidth: true,
                    pageSize: 5,
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
                //renderTo: 'summary'
            });

            Ext.create('Ext.Panel', {
                renderTo: 'GridUtentiMail',
                frame: true,
                resizable: true,
                title: 'Elenco Emails',
                //  width: 1205,
                // minWidth: 1220,
                maxWidth: 1650,
                height: 340,
                border: false,
                layout: 'anchor',
                items: [
                   grid, summaryPanel]
            });

        });

    </script>

</asp:Content>

