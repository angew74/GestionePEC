<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Mail.Master" Theme="Delta" EnableEventValidation="false" ValidateRequest="false" AutoEventWireup="true" CodeBehind="NewMailExt.aspx.cs" Inherits="GestionePEC.pages.MailClient.NewMailExt" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:HiddenField runat="server" ID="hfIdTitolo" />
    <asp:HiddenField runat="server" ID="hfIdReferral" />
    <div class="content-panel-borderless">
        <div class="header-panel-blue">
            <div class="header-title">
                <div class="header-text-left">
                    <label>
                        Creazione nuova Comunicazione</label>
                </div>
                <div class="header-text-right">
                </div>
            </div>
        </div>
        <div class="control-main">
            <div class="body-panel-gray">
            <div id="divFormNuova"></div>
            <asp:HiddenField ID="HidHtml" runat="server" />
        </div>
    </div>
    </div>
    <script type="text/javascript">
        Ext.onReady(function () {
            Ext.Loader.setPath({
                'Ext.ux': '../../ExtJS6/ux'
            });
            Ext.require([
    'Ext.ux.upload.Dialog',
    'Ext.ux.upload.Panel',
    'Ext.ux.upload.uploader.FormDataUploader',
    'Ext.ux.upload.uploader.ExtJsUploader',
    'Ext.ux.upload.header.Base64FilenameEncoder'
            ]);
            Ext.tip.QuickTipManager.init();

            var requiredMessage = '<div style="color:red;">Campo obbligatorio</div>';
            Ext.define('TitoliModel', {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'Id', type: 'string' },
                    { name: 'Nome', type: 'string' }
                ]
            });

            Ext.define('UserMailsModel', {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'UserId', type: 'string' },
                    { name: 'EmailAddress', type: 'string' }
                ]
            });


            var readerTitoli = new Ext.data.JsonReader({
                idProperty: 'Id',
                model: 'TitoliModel',
                messageProperty: 'Message',
                type: 'json',
                rootProperty: 'TitoliList',
                totalProperty: 'Totale'
            });

            var readerUserMails = new Ext.data.JsonReader({
                idProperty: 'Id',
                model: 'UserMailsModel',
                messageProperty: 'Message',
                type: 'json',
                rootProperty: 'MailUsers',
                totalProperty: 'Totale'
            });

            var readerSottoTitoli = new Ext.data.JsonReader({
                idProperty: 'Id',
                model: 'TitoliModel',
                messageProperty: 'Message',
                type: 'json',
                rootProperty: 'SottoTitoliList',
                totalProperty: 'Totale'
            });


            var storeUserMails = Ext.create('Ext.data.Store', {
                autoLoad: true,
                storeId: 'storeUserMails',
                model: 'UserMailsModel',
                reader: readerUserMails,
                proxy:
                   {
                       type: 'ajax',
                       url: '/GestionePEC/api/EmailsController/GetMailSendersByUserMails',
                       reader: readerUserMails
                   },
                //  restful: true,
                listeners: {
                    load: function (s, r, o) {
                    },
                    exception: function () {
                    }
                }
            });

            var storeTitoli = Ext.create('Ext.data.Store', {
                autoLoad: true,
                storeId: 'storeTitoli',
                model: 'TitoliModel',
                reader: readerTitoli,
                proxy:
                   {
                       type: 'ajax',
                       url: '/GestionePEC/api/TitolarioController/GetTitoli',
                       reader: readerTitoli
                   },
                //  restful: true,
                listeners: {
                    load: function (s, r, o) {
                    },
                    exception: function () {
                    }
                }
            });

            var storeSottoTitoli = Ext.create('Ext.data.Store', {
                autoLoad: true,
                storeId: 'storeSottoTitoli',
                model: 'TitoliModel',
                reader: readerSottoTitoli,
                proxy:
                   {
                       type: 'ajax',
                       url: '/GestionePEC/api/TitolarioController/GetSottoTitoli',
                       reader: readerSottoTitoli
                   },
                //  restful: true,
                listeners: {
                    load: function (s, r, o) {
                    },
                    exception: function () {
                    }
                }
            });

            function ShowValueObject(values, obj) {
                var idRef = values.Id;
                storeSottoTitoli.getProxy().extraParams.idtitolo = idRef;
                Ext.getCmp('SottoTitoliCombo').enable();
                Ext.getCmp('SottoTitoliCombo').getStore().load();
            };


            var UserMailsCombo = Ext.create('Ext.form.field.ComboBox', {
                fieldLabel: 'Caselle',
                emptyText: 'Selezionare una mail mittente',
                //  renderTo: 'TipoCombo',
                id: 'UserMailsCombo',
                displayField: 'EmailAddress',
                name: 'SenderMail',
                valueField: 'UserId',
                ctCls: 'LabelBlackBold',

                editable: false,
                listeners: {
                    select: function (cbx, rec, idx) {
                    //    ShowValueObject(rec.data, cbx);
                    }
                },
                tpl: Ext.create('Ext.XTemplate',
              '<ul class="x-list-plain"><tpl for=".">',
                  '<li role="option" class="x-boundlist-item">{EmailAddress}</li>',
              '</tpl></ul>'
          ),
                width: 750,
                labelWidth: 170,
                margin: '0 0 0 0px',
                store: storeUserMails,
                allowBlank: false,
                blankText: requiredMessage,
                msgTarget: 'under',
                queryMode: 'local'
            });

            var TitoliCombo = Ext.create('Ext.form.field.ComboBox', {
                fieldLabel: 'Titoli',
                emptyText: 'Selezionare un titolo',
                //  renderTo: 'TipoCombo',
                id: 'TitoliCombo',
                displayField: 'Nome',
                name: 'Titolo',
                valueField: 'Id',
                ctCls: 'LabelBlackBold',
                editable: false,
                listeners: {
                    select: function (cbx, rec, idx) {
                        ShowValueObject(rec.data, cbx);
                    }
                },
                tpl: Ext.create('Ext.XTemplate',
              '<ul class="x-list-plain"><tpl for=".">',
                  '<li role="option" class="x-boundlist-item">{Nome}</li>',
              '</tpl></ul>'
          ),
                width: 750,
                labelWidth: 170,
                margin: '0 0 0 0px',
                store: storeTitoli,
                allowBlank: false,
                blankText: requiredMessage,
                msgTarget: 'under',
                queryMode: 'local'
            });

            var SottoTitoliCombo = Ext.create('Ext.form.field.ComboBox', {
                fieldLabel: 'SottoTitoli',
                emptyText: 'Selezionare un Sottotitolo',
                //  renderTo: 'TipoCombo',
                id: 'SottoTitoliCombo',
                displayField: 'Nome',
                name: 'SottoTitolo',
                valueField: 'Id',
                ctCls: 'LabelBlackBold',
                editable: false,
                disabled:true,
                tpl: Ext.create('Ext.XTemplate',
              '<ul class="x-list-plain"><tpl for=".">',
                  '<li role="option" class="x-boundlist-item">{Nome}</li>',
              '</tpl></ul>'
          ),
                width: 750,
                labelWidth: 170,
                margin: '5 0 0 0px',
                store: storeSottoTitoli,
                allowBlank: false,
                blankText: requiredMessage,
                msgTarget: 'under',
                queryMode: 'local'
            });
            function submitOnEnter(field, event) {
                if (event.getKey() == event.ENTER) {
                    var form = field.up('form').getForm();
                    form.submit();
                }
            }

            var requiredMessage = '<div style="color:red;">Campo obbligatorio</div>';

            var appPanel = new Ext.panel.Panel({
                height: 250,
                title: 'Gestione Allegati',
               // renderTo: 'divUpload',
                scrollable: 'true',
                width: 1050,
                uploadComplete: function (items) {
                    var output = 'Caricamento files: <br>';
                    Ext.Array.each(items, function (item) {
                        output += item.getFilename() + ' (' + item.getType() + ', '
                            + Ext.util.Format.fileSize(item.getSize()) + ')' + '<br>';
                    });

                    this.update(output);
                }
            });

            appPanel.syncCheckbox = Ext.create('Ext.form.field.Checkbox', {
                inputValue: true,
                checked: true
            });

            appPanel.addDocked({
                xtype: 'toolbar',
                dock: 'top',
                items: [
                    {
                        xtype: 'button',
                        text: 'Caricamento Files',
                        scope: appPanel,
                        handler: function () {

                            var uploadPanel = Ext.create('Ext.ux.upload.Panel', {
                                uploader: 'Ext.ux.upload.uploader.FormDataUploader',
                                uploaderOptions: {
                                    url: '../../Ashx/FileUploadHandler.ashx'
                                },
                                synchronous: appPanel.syncCheckbox.getValue()
                            });

                            var uploadDialog = Ext.create('Ext.ux.upload.Dialog', {
                                dialogTitle: 'Finestra di scelta Files',
                                panel: uploadPanel
                            });

                            this.mon(uploadDialog, 'uploadcomplete', function (uploadPanel, manager, items, errorCount) {
                                this.uploadComplete(items);
                                if (!errorCount) {
                                    uploadDialog.close();
                                }
                            }, this);

                            uploadDialog.show();
                        }
                    }
                ]
            });

           <%-- var HtmlPanel = new Ext.form.Panel({
                id: 'htmlEditor',
                title: 'Testo Comunicazione',
                renderTo: 'divTextArea',
                width: 1050,
                height: 300,
                border: false,
                //   frame: false,   
                bodyStyle: {
                    padding: '5px'
                },
                layout: 'fit',
                header: {
                    border: false
                },
                items: {
                    xtype: 'htmleditor',
                    enableColors: true,
                    enableAlignments: true,
                    enableSourceEdit: true,
                    enableFont: true,
                    enableFontSize: true,
                    enableFormat: true,
                    frame: false,
                    id: 'TextHtml',
                    width: 600,
                    maxWidth: 1030,
                    padding: '0 0 0 -100',
                    enabled: true,
                    enableLinks: true,
                    enableLists: true,
                    enabled: true,
                    width: 600,
                    //  width: 350,
                    //  height: 290,
                    listeners: {
                        change: function () {
                            on_editor_blur();
                        }
                    }
                   , value: '<%= Comunicazione %>',
                syncValue: function () {
                    var me = this,
                        body, changed, html, bodyStyle, match, textElDom;
                    if (me.initialized) {
                        body = me.getEditorBody();
                        html = body.innerHTML;
                        textElDom = me.textareaEl.dom;
                        if (Ext.isWebKit) {
                            bodyStyle = body.getAttribute('style');
                            if (bodyStyle !== null) { // ***** THIS IS THE FIX *****
                                match = bodyStyle.match(me.textAlignRE);
                                if (match && match[1]) {
                                    html = '<div style="' + match[0] + '">' + html + '</div>';
                                }
                            }
                        }
                        html = me.cleanHtml(html);
                        if (me.fireEvent('beforesync', me, html) !== false) {

                            if (Ext.isGecko && textElDom.value === '' && html === '<br>') {
                                html = '';
                            }
                            if (textElDom.value !== html) {
                                textElDom.value = html;
                                changed = true;
                            }
                            me.fireEvent('sync', me, html);
                            if (changed) {
                                me.checkChange();
                            }
                        }
                    }
                }
            }
          });


            Ext.getCmp('TextHtml').setReadOnly(false);--%>

            var formNuovo = Ext.create('Ext.form.Panel',
               {
                   frame: true,
                   title: 'Comunicazione',
                   bodyPadding: 10,
                   scrollable: true,
                   width: '100%',
                   id: 'formNuovaComunicazione',
                   //   layout: 'column',
                 //  height: 440,
                   // xtype: 'fieldset',
                   autoWidth: false,
                   renderTo: 'divFormNuova',
                   items: [
                       UserMailsCombo,
                       {
                           fieldLabel: 'Destinatario',
                           xtype: 'textfield',
                           emptyText: 'Digitare il destinatario/i destinatari',
                           id: 'DestinatarioText',
                           name: 'Destinatario',
                           width: 750,
                           labelWidth: 170,
                           allowBlank: false,
                           blankText: requiredMessage,
                           msgTarget: 'under',
                           maxLength: 120,
                           enforceMaxLength: true,
                           maxLengthText: 'campo massimo 120 caratteri',
                           regex: /^(([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+([;,.](([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+)*$/,
                           regexText:'Il campo deve contenere uno o più indirizzi email separati da punto e virgola (;)',
                           minLength: 5,
                           padding: '5 0 0 0',
                          // vtype: 'email',
                           minLengthText: 'minimo 5 caratteri'
                       }, {
                           fieldLabel: 'Conoscenza',
                           name: 'Conoscenza',
                           id: 'ConoscenzaText',
                           xtype: 'textfield',
                           emptyText: 'Digitare destinatario in conoscenza',
                           submitEmptyText: false,
                           width: 750,
                           labelWidth: 170,
                           allowBlank: true,
                           padding: '5 0 0 0',
                           msgTarget: 'under',
                           maxLength: 120,
                           enforceMaxLength: true,
                           maxLength: 'campo massimo 120 caratteri',
                           minLengthText: 'minimo 5 caratteri',
                           regex: /^(([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+([;,.](([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+)*$/,
                           regexText: 'Il campo deve contenere uno o più indirizzi email separati da punto e virgola (;)',
                           minLength: 5
                       }, {
                           fieldLabel: 'Destinatario nascosto',
                           name: 'BCC',
                           id: 'BCCText',
                           xtype: 'textfield',
                           emptyText: 'Digitare destinatario nascosto',
                           submitEmptyText: false,
                           width: 750,
                           labelWidth: 170,
                           allowBlank: true,
                           padding: '5 0 0 0',
                           msgTarget: 'under',
                           maxLength: 120,
                           enforceMaxLength: true,
                           maxLength: 'campo massimo 120 caratteri',
                           minLengthText: 'minimo 5 caratteri',
                           regex: /^(([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+([;,.](([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+)*$/,
                           regexText: 'Il campo deve contenere uno o più indirizzi email separati da punto e virgola (;)',
                           minLength: 5
                       }, {
                           fieldLabel: 'Oggetto Comunicazione',
                           name: 'Oggetto',
                           id: 'OggettoText',
                           xtype: 'textfield',
                           emptyText: 'Digitare oggetto comunicazione',
                           width: 750,
                           labelWidth: 170,
                           allowBlank: false,
                           padding: '5 0 0 0',
                           blankText: requiredMessage,
                           msgTarget: 'under',
                           maxLength: 200,
                           enforceMaxLength: true,
                           maxLength: 'campo massimo 200 caratteri',
                           minLengthText: 'minimo 1 carattere',
                           minLength: 1
                       }, TitoliCombo, SottoTitoliCombo,
                   {
                       fieldLabel: 'Testo Mail',
                       name: 'TestoMail',
                       id: 'testomail',
                       xtype: 'textareafield',
                       padding: '5 0 0 0',
                       height: 100,
                       width: 750,
                       labelWidth: 170,
                       maxlength: 500,
                       maxLengthText: 'Massimo 500 caratteri',
                       enforceMaxLength: true
                   }, appPanel],
                   buttons: [
                  {
                      text: 'Salva Comunicazione',
                      id: 'btnConferma',
                      scope: this,
                      formBind: true,
                      xtype: 'button',
                      listeners: {
                          click: function () {
                              // this == the button, as we are in the local scope
                              submitHandlerNuovoConferma();
                          }
                      }
                  }]
               });

            function submitHandlerNuovoConferma() {
                var form = Ext.getCmp('formNuovaComunicazione');
                form.getForm().submit({
                    clientValidation: false,
                    // standardSubmit:true,
                    url: '/GestionePEC/api/PECController/Comunicazione',
                    waitTitle: 'Attendere prego',
                    submitEmptyText: false,
                    waitMsg:'Elaborazione in corso',
                    method: 'POST',
                    success: function (form, action) {
                        if (action.result.message != null) {
                            var message = ' ';
                            message += action.result.message;
                            Ext.Msg.alert('Comunicazione salvata correttamente', message);
                            // ManageError("Errore nell'aggiornameto dettagli: " + message);
                        }
                        else {
                            Ext.Msg.alert('Comunicazione salvata correttamente', 'Mail in partenza');
                        }
                    },
                    failure: function (form, action) {
                        var form = Ext.getCmp('formNuovaComunicazione');
                        if (action.response.responseText != null) {
                            var message = Ext.decode(action.response.responseText).Message;
                            if (messge != 'OK') {
                                Ext.Msg.alert('Errore salvataggio comunicazione', message);
                            }
                            else {
                                Ext.Msg.alert('Comunicazione salvata', 'Complimenti mail in partenza');
                            }
                        }
                        else {
                            if (action.result != null) {
                                var message;
                                message += action.result.errormessage[i];
                                Ext.Msg.alert('Errore salvataggio comunicazione', message);
                            }
                        }
                    }
                });
                form.el.unmask();
            };
        });


    </script>

    <script type="text/javascript">
        function on_editor_blur() {
            var appo = Ext.getCmp('TextHtml').getValue().replace("'", "\x27");
            var bappo = appo.replace(/'/g, '"');
            var myHidden = document.getElementById('<%= HidHtml.ClientID %>');
            if (myHidden)//checking whether it is found on DOM, but not necessary
            {
                myHidden.value = bappo;
            }
        }

        function GetValuesHtml() {

            // var appo = extHtml.items.get('textEditor').getValue();
            var appo = Ext.getCmp('TextHtml').getValue();
            var myHidden = document.getElementById('<%= HidHtml.ClientID %>');
    if (myHidden)//checking whether it is found on DOM, but not necessary
    {
        myHidden.value = appo;
    }
}
    </script>

</asp:Content>
