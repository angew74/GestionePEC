<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InBox.ascx.cs" Inherits="GestionePEC.Controls.InBox" %>
<%@ Import Namespace="ActiveUp.Net.Common.DeltaExt" %>
<%@ Import Namespace="SendMail.Model" %>
<%@ Register Assembly="GestionePEC" Namespace="GestionePEC.Extensions" TagPrefix="inl" %>
<div class="control-main">
    <div id="topic-grid">
    </div>
</div>
<asp:Button ID="btnPost" runat="server" Text="Dettagli" OnClick="btnPost_Click" />
<asp:HiddenField runat="server" ID="hfSelectedRow" />
<asp:HiddenField runat="server" ID="hfCurrentRating" />
<asp:HiddenField runat="server" ID="hfCurrentFolder" />
<asp:HiddenField runat="server" ID="hfDimension" />
<asp:HiddenField runat="server" ID="hfPFolder" />
<inl:InlineScript runat="server">

    <script type="text/javascript">
var btnPost = document.getElementById('<%= btnPost.ClientID %>');
btnPost.style.display = "none";


var mailAction = 0;
var selectedIds = '';
var parentFolder ="I";
var filterCustom = null;

Ext.Loader.setConfig({ enabled: true });
Ext.require([
    'Ext.grid.*',
    'Ext.data.*',
    'Ext.util.*',
    'Ext.toolbar.Paging',
    'Ext.tip.QuickTipManager'
]);


Ext.onReady(function () {
    Ext.tip.QuickTipManager.init();
    if (!Ext.ClassManager.isCreated('Mails')) {
        Ext.define('Mails', {
            extend: 'Ext.data.Model',    
            fields: ['from',
                     { name: 'date', type: 'date', dateFormat: 'd/m/Y H:i:s' },
                     'subject',
                     'sStatus',
                     'mStatus',
                     'pText',
                     'attach',
                     'utente',
                     'dimen']
        });
    }
    if (Ext.getCmp('StatusFilter')) {
        Ext.getCmp('StatusFilter').destroy();
    }

    var json = eval("<%= this.JsonCartella %>");    

    //ricarcica la griglia da zero considerando la folder scelta dall'utente(il primo caricamento si posiziona sulla Inbox)
    var doSearch = function (Folder_id) {
        if (currentFolder != Folder_id) {
            currentFolder = Folder_id;
            store.load({ params: { start: 0, limit: 25, folder: currentFolder } });
            SetColumnHeaders();
        }
    };

    if (Ext.getCmp('folders')) {
        Ext.getCmp('folders').destroy();
    }


    var Folder = Ext.create('Ext.form.ComboBox', {
        id: 'folders',
        // typeAhead: true,
      //  triggerAction: 'all',
        allowBlank: false,
        //forceSelection: false,
        value: 1,
        editable: false,
      //  lazyRender: true,
        readOnly: true,
        mode: 'local',
        width: 350,
        labelWidth:50,
        mode: 'local',       
        tabIndex: 4,
        store: new Ext.data.ArrayStore({
            fields: ['type', 'displayText'],
            data: json
        }),
        valueField: 'type',
        displayField: 'displayText',
        listeners: {
            // collapse: doSearch.createDelegate(this),
            collapse: doSearch.bind(this),
            //  collapse : Ext.Function.pass(doSearch(this)),
            // specialKey: stopSpecialKey.createDelegate(this)
            specialKey: stopSpecialKey.bind(this)
        }
    });

       var currentFolder = Ext.getCmp('folders').getValue();
        var StatusFilter = Ext.create('Ext.form.ComboBox', {
            // typeAhead: true,
            triggerAction: 'all',
            id: 'StatusFilter',
            disabled: true,
            forceSelection: true,
            lazyRender: true,
            emptyText: 'Status...',
            width: 100,
            mode: 'local',
            tabIndex: 5,
            editable: false,
            //  selectOnFocus: true,
            store: statusArray,
            valueField: 'idItem',
            displayField: 'displayText',
            enableKeyEvents: true,
            listeners: {
                enable: function (cBox) {
                    var selVal = cBox.getValue();
                    if (selVal != '') {
                        // setDisable(FindButton, false);
                        Ext.getCmp('FindButton').disabled = true;
                    }
                },
                disable: function (cBox) {
                    if (Ext.getCmp('FindText').disabled == true) {
                        // setDisable(FindButton, true);
                        Ext.getCmp('FindButton').disabled = true;
                    }
                },
                expand: function (o) {
                    var val = '#' + currentFolder + '#';
                    statusArray.filter('folder', val, true, true);
                },
                collapse: function (o) {
                    if (statusArray.isFiltered() == true) {
                        statusArray.clearFilter(true);
                    }
                },
                select: function (cBox, rec, ind) {
                    var selVal = cBox.getValue();
                    if (selVal == '' || selVal == null) {
                        if (Ext.getCmp('FindText').disabled == true) {
                            setDisable(Ext.getCmp('FindButton'), true);
                        }
                    } else {
                        setDisable(Ext.getCmp('FindButton'), false);
                    }
                },
                // specialkey: stopSpecialKey.createDelegate(this)
                specialkey: stopSpecialKey.bind(this)
            }
        });
    

    var proxy = new Ext.data.proxy.Ajax({
        url: '<%= Page.ResolveClientUrl(MailBoxProvider) %>',
        method: 'POST',
        model: 'Mails',
        reader: {
            type: 'json',
            rootProperty: 'data',
            //callbackKey: 'theCallbackFunction',
            totalProperty: 'TotalCount'
        }
    });

    function GetFilterCustom() {
        var sType = '';
        var filterCustomFunc;
        var folderFilter = Ext.getCmp('folders').getValue();
        var txt = '';
        var status = '';
        if (typeof FilterAction != 'undefined') {
            sType = Ext.getCmp('FilterAction').getValue();
        };
        if (typeof FindText != 'undefined') {
            txt = Ext.getCmp('FindText').getValue();
        };
        if (typeof StatusFilter != 'undefined') {
            status = StatusFilter.getValue();
        };
        if ((sType != '' && sType != '0' && txt.length > 3) || (status != '' && status != null)) {
            var filter = {
                text: {
                    tipo: sType,
                    value: txt
                },
                status: {
                    tipo: '<%= (int)MailIndexedSearch.STATUS_MAIL %>',
                    value: status
                }
            };
            if (typeof store != 'undefined') {
                var filterCustomFunc = JSON.stringify(filter);
            }
        }
        return filterCustomFunc;
    }


    var store = new Ext.data.JsonStore({
        // root: 'Data',
        storeId: 'mailStore',
        model: 'Mails',
        pageSize:25,
        autoLoad: {
            params: {
                start: 0,
                limit: 25,
                folder: currentFolder,
                parFolder: parentFolder,
                mailAction: mailAction,
                mailIds: selectedIds,
                filter: filterCustom 
            }
        }, //per default vado nella inbox
        proxy: proxy,
        //fields: ['from',
        //         { name: 'date', type: 'date', dateFormat: 'd/m/Y H:i:s'},
        //         'subject',
        //         'sStatus',
        //         'mStatus',
        //         'pText',
        //         'attach',
        //         'utente',
        //         'dimen'],
        //totalProperty: 'TotalCount',
        listeners: {
            beforeload: function (store, operation, eOpts) {
                // s.setBaseParam('folder', currentFolder);
                // s.setBaseParam('mailAction', mailAction);
                //  s.setBaseParam('mailIds', selectedIds);
                //  s.setBaseParam('parFolder',parentFolder);  
              //  currentFolder = document.getElementById('ctl00_MainContentPlaceHolder_Inbox1_hfCurrentFolder').value;             
                var filterCustomLoad = GetFilterCustom();
                store.getProxy().extraParams = {}; // clear all previous
                store.getProxy().extraParams.folder = Ext.getCmp('folders').getValue();
                store.getProxy().extraParams.mailAction = mailAction;
                store.getProxy().extraParams.mailIds = selectedIds;
                store.getProxy().extraParams.parFolder = parentFolder;
                store.getProxy().extraParams.filter = filterCustomLoad;
            }
        }
    });   

   

    function SetColumnHeaders() {
        var colMod = Ext.getCmp('mailGrid').getView().getHeaderCt().getGridColumns();
        if (colMod != null) {           
            if (parentFolder == "O" || parentFolder == "AO" || parentFolder == "OA" || parentFolder == "AO") {
                colMod[5].text = "Destinatario";
                colMod[4].text = "Inviato il";
            } else {
                colMod[5].text ="Mittente";
                colMod[4].text = "Ricevuto il";
            }
        }
    };
    var doAction = function (obj) {
        if (obj.getValue() != '' && obj.getValue() != null && obj.getValue() != '0') {
            var sel = Ext.getCmp('mailGrid').selModel.getSelection();
            if (sel.length > 0) {
                mailAction = parseInt(obj.getValue());             
                for (var i = 0; i < sel.length; i++) {
                    selectedIds += sel[i].id + ",";
                };
                Ext.getCmp('mailGrid').store.getProxy().extraParams.mailIds = selectedIds;
                Ext.getCmp('ToolbarGrid').doRefresh();
                resetAction(obj);
            }
        }
        resetAction(obj);
    };
    function resetAction(obj) {
        obj.reset();
        obj.getStore().clearFilter(true);
        mailAction = 0;
        selectedIds = '';
        if (typeof grid != 'undefined') {
            var selMod = grid.getSelectionModel();
            selMod.clearSelections(false);
            var gridStore = Ext.getCmp('mailGrid').getStore();
            var colModel = Ext.getCmp('mailGrid').getView().getHeaderCt().getGridColumns();
           // Ext.getCmp('mailGrid').reconfigure(gridStore, colModel);
        }
    };
    function stopSpecialKey(o, e) {
        e.stopEvent();
    };
    if (!Ext.ClassManager.isCreated('ActionsModel')) {
        Ext.define('ActionsModel', {
            extend: 'Ext.data.Model',
            fields: [
               { name: 'Id', type: 'string' },
                { name: 'NomeAzione', type: 'string' }
            ]
        });
    }

    var readerAction = new Ext.data.JsonReader({
        idProperty: 'Id',
        messageProperty: 'message',
        rootProperty: 'ActionsList',
        totalProperty: 'total',
        model: 'ActionsModel'
    });

    var proxyAction = new Ext.data.HttpProxy({
        url: '<%= Page.ResolveClientUrl("~/api/FolderController/GetActions")%>?idfolder=' + Ext.getCmp('folders').getValue(),
        method: 'GET',
        disableCaching: true,
        reader: readerAction,
        restful: true,
        headers: { 'Content-Type': 'application/json; charset=utf-8' }
    });  
  


    var actionStore = Ext.create('Ext.data.Store', {
        proxy: proxyAction,       
        storeId: 'ActionStore',
        model: 'ActionsModel',
        autoLoad: true,
        baseParams: {
            type: '',
            start: 0,
            limit: 10
        },
        listeners: {
            beforeload: function (s, opt) {
                return true;
            },
            exception: function () {
            }
        }
    });



    if (Ext.getCmp('ComboActions')) {
        Ext.getCmp('ComboActions').destroy();
    }
        var MailActions = Ext.create('Ext.form.ComboBox', {
            // typeAhead: true,
            //  triggerAction: 'all',  
            // lazyRender: true,
            editable: false,
            id: 'ComboActions',
            model: 'ActionsModel',
            disabled: true,
            width: 300,           
            hideLabel: true,
            labelWidth: 10,
            queryMode: 'local',
            hideTrigger: true,
            forceSelection: true,
            tabIndex: 0,
            store: actionStore,
            valueField: 'Id',
            displayField: 'NomeAzione',
            tpl: Ext.create('Ext.XTemplate',
                 '<ul class="x-list-plain"><tpl for=".">',
                     '<li role="option" class="x-boundlist-item">{NomeAzione}</li>',
                 '</tpl></ul>'
             ),
            listeners: {
                //  collapse: doAction.createDelegate(this),
                collapse: doAction.bind(this),
                //  specialKey: stopSpecialKey.createDelegate(this),
                specialKey: stopSpecialKey.bind(this),
                expand: function (c) {
                    // Ext.getCmp('ActionStore').getProxy().url 
                    c.getStore().getProxy().url = '<%= Page.ResolveClientUrl("~/api/FolderController/GetActions")%>?idfolder=' + Ext.getCmp('folders').getValue();
                    //  actionStore.proxy.conn.url =
                    // c.store = actionStore.get;
                    c.getStore().load();
                }
            }
        }); 

   

    var FilterToggleButton = Ext.create('Ext.Button', {
        allowDepress: true,
        enableToggle: true,
        pressed: false,
        text: 'Filtro: Off',       
        tooltip: 'Clic per abilitare il filtro',
        toggleHandler: function (but, pressed) {
            if (pressed) {
                if (typeof FilterAction != 'undefined') {
                    but.setText('Filtro: On');
                    but.setTooltip('Clic per disabilitare il filtro');     
                    setDisable(Ext.getCmp('FilterAction'),false);                   
                    if (Ext.getCmp('StatusFilter'))
                    {setDisable(Ext.getCmp('StatusFilter'),'false'); }
                    filterClick(this, null);
                } else {
                    but.toggle(false);
                }
            } else {
                but.setText('Filtro: Off');
                but.setTooltip('Clic per abilitare il filtro');
                setDisable(Ext.getCmp('FilterAction'), true);               
                setDisable(Ext.getCmp('StatusFilter'), true);
                Ext.getCmp('FindText').setValue('');
                var folderFilter = Ext.getCmp('folders').getValue();
                if (typeof store != 'undefined') {
                   // if (store.getProxy().extraParams.filter != null) {
                     //   store.getProxy().extraParams.filter =  null;
                        store.load({ params: { start: 0, limit: 25, folder: folderFilter, parFolder: parentFolder,filter:null} });
                  //  }
                }
            }
        }
    });

    var FilterResetButton = Ext.create('Ext.Button', {
        allowDepress: false,
        enableToggle: false,
        text: 'Filtro: Reset',
        tooltip: 'Clic per resettare il filtro',
        listeners: {
            click: function (b, e) {
                Ext.getCmp('FilterAction').reset();
                Ext.getCmp('FindText').setValue('');
                Ext.getCmp('StatusFilter').reset();
                setDisable( Ext.getCmp('FindButton'), true);             
                if (typeof store != 'undefined') {
                  //  if (Ext.getCmp('mailGrid').getStore().extraParams.filter != null) {
                       // Ext.getCmp('mailGrid').geStore().filter = null;
                        Ext.getCmp('mailGrid').geStore().load({ params: { start: 0, limit: 25, folder: currentFolder, parFolder: parentFolder,filter:null } });
                  //  }
                }
            }
        }
    });
    

    var storeFilter = Ext.create('Ext.data.Store', {
        fields: [
             { name: 'id', type: 'int' },
             { name: 'displayText', type: 'string' }
        ],
        data: [
       { "id": 1, "displayText": "Mail" },
       { "id": 2, "displayText": "Oggetto" }      
        ]
    });

    if (Ext.getCmp('FilterAction')) {
        Ext.getCmp('FilterAction').destroy();
    }
        var FilterAction = Ext.create('Ext.form.ComboBox', {
            // typeAhead: true,
            //  triggerAction: 'all',
            disabled: true,
            id: 'FilterAction',
            // forceSelection: true,
            //lazyRender: true,
            emptyText: 'Filtra per...',
            width: 150,
            labelWidth: 30,
            // mode: 'local',
            queryMode: 'local',
            // tabIndex: 5,
            editable: false,
            // selectOnFocus: true,
            store: storeFilter,
            valueField: 'id',
            displayField: 'displayText',
            tpl: Ext.create('Ext.XTemplate',
               '<ul class="x-list-plain"><tpl for=".">',
                   '<li role="option" class="x-boundlist-item">{displayText}</li>',
               '</tpl></ul>'
           ),
            listeners: {
                disable: function (cBox) {
                    setDisable(Ext.getCmp('FindText'), true);
                },
                enable: function (cBox) {
                    var selVal = cBox.getValue();
                    if (selVal != '' && selVal !== '0') {
                        setDisable(Ext.getCmp('FindText'), false);
                    }
                },
                blur: function (cBox) {
                    var selVal = cBox.getValue();
                    if (selVal === '' || selVal === '0') {
                        setDisable(Ext.getCmp('FindText'), false);
                    }
                },
                select: function (cBox, rec, ind) {
                    var selVal = cBox.getValue();
                    if (selVal === '' || selVal === '0') {
                        Ext.getCmp('FindText').setValue('');
                        setDisable(Ext.getCmp('FindText'), true);
                    } else {
                        setDisable(Ext.getCmp('FindText'), false);
                    }
                },
                // specialkey: stopSpecialKey.createDelegate(this)
                specialkey: stopSpecialKey.bind(this)
            }
        });
    

        if (Ext.getCmp('FindText')) {
            Ext.getCmp('FindText').destroy();
        }

        var FindText = Ext.create('Ext.form.field.Text', {
            width: 350,
            labelWidth: 20,
            emptyText: 'cerca...',
            validateOnBlur: false,
            disabled: true,
            maxLength: 20,
            id: 'FindText',
            enableKeyEvents: true,
            listeners: {
                disable: function (txt) {
                    // var statusValue = StatusFilter.getValue();
                    var statusValue = Ext.getCmp('StatusFilter').getValue();
                    if (Ext.getCmp('StatusFilter').disabled == true || statusValue == '' || statusValue == null) {
                        setDisable(Ext.getCmp('FindButton'), true);
                    }
                },
                enable: function (txt) {
                    var txtVal = txt.getValue();
                    if (txtVal.length > 3) {
                        setDisable(Ext.getCmp('FindButton'), false);
                    }
                },
                keyup: function (txt, e) {
                    var newVal = txt.getValue();
                    if (newVal.length > 3) {
                        setDisable(Ext.getCmp('FindButton'), false);
                    } else {
                        //   var status = StatusFilter.getValue();
                        var status = Ext.getCmp('StatusFilter').getValue();
                        if (status == '' || status == null) {
                            setDisable(Ext.getCmp('FindButton'), true);
                        }
                    }
                },
                specialkey: function (txt, e) {
                    var k = e.getKey();
                    if (k == e.ENTER) {
                        if (typeof Ext.getCmp('FindButton') != 'undefined') {
                            Ext.getCmp('FindButton').fireEvent('click', null);
                        }
                    } else {
                        if (k == e.BACKSPACE || k == e.DELETE || k == e.CAPS_LOCK ||
                            k == e.SHIFT || k == e.LEFT || k == e.RIGHT || k == e.CTRL) {
                            return;
                        }
                    }
                    e.stopEvent();
                }
            }
        });
    

    var jsonstatus = eval("<%= this.JsonStatus %>");
    var statusArray = Ext.create('Ext.data.ArrayStore', {
        fields: [{ name: 'idItem', type: 'string' }, { name: 'displayText', type: 'string' }, { name: 'folder', type: 'string' }],
        data: jsonstatus
    });

    if (Ext.getCmp('FindButton')) {
        Ext.getCmp('FindButton').destroy();
    }
        var FindButton = Ext.create('Ext.Button', {
            iconCls: 'toolbarFindDisabled',
            autoWidth: true,
            scale: 'large',
            id: 'FindButton',
            disabled: true,
            hideLabel: true,
            tooltip: 'cerca',
            listeners: {
                disable: function (but) {
                    but.setIconCls('toolbarFindDisabled');
                },
                enable: function (but) {
                    but.setIconCls('toolbarFind');
                },
                click: filterClick
            }
        });
    

    function setDisable(object, disable) {
        if (object != null && typeof object.setDisabled != 'undefined') {
            object.setDisabled(disable);
        }
    };

    var toolBar = Ext.create('Ext.panel.Panel', {
        scrollable: true,
        dockedItems: [
            {
                xtype: 'toolbar',
                dock: 'top',
                items: [{
                    xtype: 'tbtext',
                    width: 10,
                    text: 'Cartella:'
                }, Folder, { xtype: 'tbseparator' },
                    {
                        xtype: 'panel',
                       // layout: 'fit',
                        bodyStyle: { 'background-color': 'transparent' },
                        border: false,
                        buttonAlign: 'center',
                        items: FilterToggleButton
                    },
                  FilterAction, FindText, {
                      xtype: 'container',
                      rowspan: 2,
                      items: FindButton
                  },{ xtype: 'tbseparator' }]
            },
       {xtype: 'toolbar',
        dock: 'top',
        items: [{
            xtype: 'tbtext',
           text: 'Azioni:'
                    },
                    MailActions,
                    { xtype: 'tbseparator' },
                    FilterResetButton,
                    {
                        xtype: 'container',
                        layout: 'fit',
                        colspan: 2,
                        items: StatusFilter
                    },
                    { xtype: 'tbseparator' }]
            }
        ]
    });
    
    var selectionModel = Ext.create('Ext.selection.CheckboxModel',{
        checkOnly: true,
        listeners: {
            select: function (t, record, index, eOpts) {
                setDisable(Ext.getCmp('ComboActions'),false);              
                Ext.getCmp('ComboActions').updateLayout();               
            },
            deselect: function (t, record, index, eOpts) {
                if (Ext.getCmp('mailGrid').getSelection().length == 0)
                { setDisable(Ext.getCmp('ComboActions'), true); }
            }
        }
    });

    if (Ext.getCmp('mailGrid')) {
        Ext.getCmp('mailGrid').destroy();
    }

        var grid = Ext.create('Ext.grid.Panel', {
            id: 'mailGrid',
            height: 350,
            forceFit: true,            
            //split: true,
            selModel: selectionModel,
            loadMask: {
                msg: 'Caricamento...'
            },
            tbar: toolBar,
            trackMouseOver: false,
            autoExpandColumn: 'subject',
            // autoExpandMax: 4000, //fa si che la griglia utilizzi tutto lo spazio a sua disposizione
            store: store,
            viewConfig: {
                emptyText: 'Nessun ritrovamento',
                forceFit: true,
                autoFill: true
            },
            enableColumnMove: false,
            colModel: {
                defaults: {
                    sortable: true,
                    width: 100
                }
            },
            columns: [
                //selectionModel,
                {
                    id: 'status',
                    dataIndex: 'sStatus',
                    width: 55,
                    fixed: true,
                    sortable: false,
                    renderer: renderStatus
                }, {
                    id: 'attachments',
                    dataIndex: 'attach',
                    width: 30,
                    fixed: true,
                    sortable: false,
                    renderer: renderAttachments
                }, {
                    id: 'utente',
                    header: 'Utente',
                    dataIndex: 'utente',
                    width: 110,
                    fixed: true,
                    sortable: false,
                    renderer: renderTopic
                }, {
                    id: 'date',
                    header: "Ricevuto il",
                    dataIndex: 'date',
                    width: 150,
                    fixed: true,
                    renderer: renderLast,
                    sortable: true
                },{
                    id: 'from',
                    header: "Mittente",
                    dataIndex: 'from',
                    renderer: renderTopic,
                    width: 350,
                    sortable: true
                }, {
                    id: 'subject',
                    header: "Oggetto",
                    dataIndex: 'subject',
                    renderer: renderTopic,
                    align: 'left',
                    width: 590,
                    sortable: true,
                    hideable: false
                }, {
                    id: 'dimen',
                    hidden: true,
                    dataIndex: 'dimen'
                }],
            bbar: Ext.create('Ext.toolbar.Paging', {
                store: store,
                id: 'ToolbarGrid',
                autoWidth: true,
                // pageSize: 25,
                displayInfo: true,
                displayMsg: 'Visualizzazione righe {0} - {1} of {2}',
                dock: true,
                // autoWidth: true,
                emptyMsg: "Nessun record da visualizzare",
                listeners: {
                    afterLayout: function (asd) {
                        this.container.dom.style.width = "";
                        this.el.dom.style.width = "";
                    }
                }
            })
        });
   // }

    Ext.getCmp('mailGrid').autoWidth = true;
    Ext.getCmp('mailGrid').on("rowdblclick", rowSelected, this);
   
   
 
    Ext.Ajax.on('requestcomplete', UpdateRating); 
    function renderStatus(value, metaData, record, rowIndex, colIndex, store) {
        switch (value) {
            case '0':
                metaData.css = '';
                break;
            case '1':
                metaData.css = 'rating_1';
                break;
            case '2':
                metaData.css = 'rating_2';
                break;
            case '3':
                metaData.css = 'rating_3';
                break;
            case '4':
                metaData.css = 'rating_4';
                break;
            case '20':
            case '21':
                metaData.css = 'semaforo_spento';
                break;
            case '22':
            case '23':
            case '26':
                metaData.css = 'semaforo_yellow';
                break;
            case '28':
                metaData.css = 'semaforo_green';
                break;
            case '24':
            case '25':
            case '27':
            case '29':
                metaData.css = 'semaforo_red';
                break;
        }
        return '';
    };
    function renderAttachments(value, metaData, record, rowIndex, colIndex, store) {
        switch (value) {
            case '0':
                metaData.css = '';
                break;
            case '1':
                metaData.css = 'has_attachment';
                break;
            default:
                metaData.css = '';
                break;
        }
    };
    function rowSelected(gr, rowIndex, e) {
      //   var row_data = gr.store.data.items[rowIndex];
        if (rowIndex != null) {
            var hfSel = Ext.get('<%= hfSelectedRow.ClientID %>');
            var hfCurrRat = Ext.get('<%= hfCurrentRating.ClientID %>');
            var hfCurrFolder = Ext.get('<%= hfCurrentFolder.ClientID %>');
            var hfDimension = Ext.get('<%= hfDimension.ClientID %>');
            var hfPFolder = Ext.get('<%= hfPFolder.ClientID %>');
            if (typeof hfSel != 'undefined' && typeof btnPost != 'undefined') {
                for (var i = 0; i < gr.store.data.items.length; i++) {
                    if (gr.store.data.items[i].id == rowIndex.data.id) {                     
                        gr.getRow(i).style.backgroundColor = "#BAD0EF";                    
                    } else {
                        gr.getRow(i).style.backgroundColor = "";                  
                    }
                }
                hfSel.set({ value: rowIndex.data.id });
                hfCurrRat.set({ value: rowIndex.data.sStatus });
                hfCurrFolder.set({ value: Ext.getCmp('folders').getValue() });
                hfPFolder.set({ value: parentFolder });
                hfDimension.set({ value: rowIndex.data.dimen });
                btnPost.click();
            }
        }
    }; 


    function UpdateRating(c, r, o) {
        if (Ext.decode(r.responseText) == true) {
            var uidMail = o.params.idMail;
            if (typeof uidMail != 'undefined' && uidMail != null) {
                var colModel = Ext.getCmp('mailGrid').getView().getHeaderCt().getGridColumns();
                var gridStore = Ext.getCmp('mailGrid').getStore();
                if (typeof gridStore != 'undefined' && gridStore != null) {
                    var idx = gridStore.indexOfId(uidMail);
                    if (idx === -1) {
                       // HideGridMask();
                        return;
                    };
                    var item = gridStore.data.items[idx];
                    item.data.sStatus = o.params.rating.toString();                   
                    Ext.getCmp('mailGrid').reconfigure(gridStore);
                }
            }
        }
       // HideGridMask();
    };
    // render functions
    function renderTopic(value, p, record, ri, ci) {
        if (record.data.mStatus != '0') {
            return String.format('{0}<br />{1}', value, record.data.pText);
        } else {
            return String.format('<b>{0}</b><br/>{1}', value, record.data.pText);
        }
    };
    function renderLast(value, p, r) {
        var dd = Ext.util.Format.date(value, 'd/m/Y G:i:s');
        if (r.data.mStatus != '0') {
            return dd;
        }
        else
            return String.format('<b>{0}</b>', dd);
    };
    //function filter
    function filterClick(but, e) {
        var sType = '';
        var folderFilter = Ext.getCmp('folders').getValue();
        var txt = '';
        var status = '';
        if (typeof FilterAction != 'undefined') {         
            sType = Ext.getCmp('FilterAction').getValue();
        };
        if (typeof FindText != 'undefined') {
            txt = Ext.getCmp('FindText').getValue();
        };
        if (typeof StatusFilter != 'undefined') {
            status = StatusFilter.getValue();
        };
        if ((sType != '' && sType != '0' && txt.length > 3) || (status != '' && status != null)) {
            var filter = {
                text: {
                    tipo: sType,
                    value: txt
                },
                status: {
                    tipo: '<%= (int)MailIndexedSearch.STATUS_MAIL %>',
                    value: status
                }
            };
            if (typeof store != 'undefined') {               
                var filterCustom = JSON.stringify(filter);
                store.load({ params: { start: 0, limit: 25, folder: folderFilter, parFolder: parentFolder, filter: filterCustom } });
            }
        }
    };

    Ext.create('Ext.panel.Panel', {
        renderTo: 'topic-grid',
        layout: 'anchor',
        // anchor: '100%',
        frame: true,
        autoScroll: true,
        border: false,      
        items: grid
    });

});
    </script>

</inl:InlineScript>