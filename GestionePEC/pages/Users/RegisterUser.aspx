<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Mail.Master" AutoEventWireup="true" Theme="Delta" CodeBehind="RegisterUser.aspx.cs" Inherits="GestionePEC.pages.Users.RegisterUser" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div id="pnlMain" class="content-panel-borderless">
        <div class="header-panel-blue">
            <div class="header-title">
                <div class="header-text-left">
                    <label>
                        REGISTRAZIONE UTENTE</label>
                </div>
            </div>
        </div>
        <div id="container" class="control-main">
            <div class="control-body-gray">
          <div id="divNuovoUtente"></div>
            </div>     
        </div>
    </div>
   
      <script type="text/javascript">
     Ext.require([
           'Ext.grid.*',
           'Ext.data.*',
           'Ext.util.*',
           'Ext.toolbar.Paging',          
           'Ext.tip.QuickTipManager'
        ]);
     Ext.onReady(function () {
         Ext.tip.QuickTipManager.init();
         function submitOnEnter(field, event) {
             if (event.getKey() == event.ENTER) {
                 var form = field.up('form').getForm();
                 form.submit();
             }
         }

         var requiredMessage = '<div style="color:red;">Campo obbligatorio</div>';
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
             name:'Role',
             valueField: 'Id',
             ctCls: 'LabelBlackBold',
             editable: false,
             tpl: Ext.create('Ext.XTemplate',
           '<ul class="x-list-plain"><tpl for=".">',
               '<li role="option" class="x-boundlist-item">{Name}</li>',
           '</tpl></ul>'
       ),
             width: 365,
             labelWidth: 130,
             margin: '0 0 0 0px',
             store: storeRoles,
             allowBlank: false,            
             blankText: requiredMessage,
             msgTarget: 'under',
             queryMode: 'local'            
         });

         Ext.apply(Ext.form.field.VTypes, {

             password: function (val, field) {
                 if (field.initialPassField) {
                     var pwd = field.up('form').down('#' + field.initialPassField);
                     return (val == pwd.getValue());
                 }
                 return true;
             },

             passwordText: 'Le Password non coincidono'
         });


         var formNuovo = Ext.create('Ext.form.Panel',
               {
                   frame: true,
                   title: 'Profilo Utente',
                   bodyPadding: 10,
                   scrollable: true,
                   width: '100%',
                   id: 'formNuovoUtente',
                   //   layout: 'column',
                   height: 440,
                   // xtype: 'fieldset',
                   autoWidth: false,
                   renderTo:'divNuovoUtente',
                   items: [
                       {
                           fieldLabel: 'Cognome',
                           xtype: 'textfield',
                           emptyText: 'Digitare il cognome',
                           id: 'CognomeText',
                           name: 'Cognome',
                           width: 700,
                           labelWidth: 130,
                           allowBlank: false,
                           blankText: requiredMessage,
                           msgTarget: 'under',
                           maxLength: 120,
                           enforceMaxLength: true,
                           maxLengthText: 'cognome campo massimo 120 caratteri',
                           padding: '5 0 0 0',
                           minLength: 3,
                           minLengthText: 'minimo 3 caratteri'
                       }, {
                           fieldLabel: 'Nome',
                           name: 'Nome',
                           id: 'NomeText',
                           xtype: 'textfield',
                           emptyText: 'Digitare il nome',
                           width: 700,
                           labelWidth: 130,
                           allowBlank: false,
                           padding: '5 0 0 0',
                           blankText: requiredMessage,
                           msgTarget: 'under',
                           maxLength: 120,
                           enforceMaxLength: true,
                           maxLength: 'nome campo massimo 120 caratteri',
                           minLengthText: 'minimo 3 caratteri',
                           minLength: 3
                       }, {
                           fieldLabel: 'User Name',
                           name: 'UserName',
                           id: 'UserNameText',
                           xtype: 'textfield',
                           emptyText: 'Digitare username',
                           width: 700,
                           labelWidth: 130,
                           allowBlank: false,
                           padding: '5 0 0 0',
                           blankText: requiredMessage,
                           msgTarget: 'under',
                           maxLength: 16,
                           enforceMaxLength: true,
                           maxLength: 'username campo massimo 16 caratteri',
                           minLengthText: 'minimo 3 caratteri',
                           minLength: 3
                       }, {
                           fieldLabel: 'Codice Fiscale',
                           name: 'CodiceFiscale',
                           id: 'CodiceFiscaleText',
                           xtype: 'textfield',
                           emptyText: 'Digitare il codice fiscale',
                           width: 700,
                           labelWidth: 130,
                           allowBlank: false,
                           padding: '5 0 0 0',
                           blankText: requiredMessage,
                           msgTarget: 'under',
                           maxLength: 16,
                           enforceMaxLength: true,
                           maxLength: 'username  campo massimo 16 caratteri',
                           minLengthText: 'minimo 16 caratteri',
                           minLength: 16
                       }, {
                           fieldLabel: 'Password',
                           name: 'Password',
                           id: 'PasswordText',
                           xtype: 'textfield',
                           inputType: 'password',
                           emptyText: 'Digitare la password',
                           width: 700,
                           labelWidth: 130,
                           allowBlank: false,
                           padding: '5 0 0 0',
                           blankText: requiredMessage,
                           msgTarget: 'under',
                           maxLength: 12,
                           enforceMaxLength: true,
                           maxLength: 'nome campo massimo 12 caratteri',
                           minLengthText: 'minimo 8 caratteri',
                           minLength: 8,
                           listeners: {
                               change: function (field) {
                                   var confirmField = field.up('form').down('[name=ConfirmPassword]');
                                   confirmField.validate();
                               }
                           }                          
                       }, {
                           fieldLabel: 'Conferma Password',
                           name: 'ConfirmPassword',
                           id: 'CPasswordText',
                           xtype: 'textfield',
                           inputType: 'password',
                           emptyText: 'conferma la password',
                           width: 700,
                           labelWidth: 130,
                           allowBlank: false,
                           padding: '5 0 0 0',
                           blankText: requiredMessage,
                           initialPassField: 'PasswordText',
                           msgTarget: 'under',
                           maxLength: 120,
                           enforceMaxLength: true,
                           maxLength: 'campo massimo 12 caratteri',
                           minLengthText: 'minimo 8 caratteri',
                           minLength: 8,
                           validator: function () {
                               var formPanel = Ext.getCmp("formNuovoUtente").getForm();
                               // Save the fields we are going to insert values into
                               var pass1 = Ext.getCmp('PasswordText').getValue();
                               var pass2 = Ext.getCmp('CPasswordText').getValue();
                               if (pass1 == pass2)
                                   return true;

                               else
                                   return "Passwords non coincidono!";
                           }
                       }, RolesCombo],
                   buttons: [
                  {
                      text: 'Conferma creazione Utente',
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
             var form = Ext.getCmp('formNuovoUtente');
             form.getForm().submit({
                 clientValidation: false,
                 // standardSubmit:true,
                 url: '/GestionePEC/api/UsersServiceController/RegisterUser',
                 waitTitle: 'Attendere prego',
                 method: 'POST',
                 success: function (form, action) {                    
                     if (action.result.message != null) {
                         var message = ' ';
                         message += action.result.message;
                         Ext.Msg.alert('Utente creato correttamente', message);
                        // ManageError("Errore nell'aggiornameto dettagli: " + message);
                     }
                     else {
                         Ext.Msg.alert('Utente creato correttamente', 'Abilitare utente ad email');
                     }
                 },
                 failure: function (form, action) {
                     var form = Ext.getCmp('formNuovoUtente');
                     if (action.response.responseText != null) {
                         var message = Ext.decode(action.response.responseText).Message;
                         if (messge != 'OK') {
                             Ext.Msg.alert('Errore creazione utente', message);
                         }
                         else {
                             Ext.Msg.alert('Utente creato', 'Complimenti utente creato correttamente');
                         }
                     }
                     else {
                         if (action.result != null) {
                             var message;                             
                                 message += action.result.errormessage[i];                            
                                 Ext.Msg.alert('Errore creazione utente', message);
                         }
                     }                    
                 }
             });
             form.el.unmask();
         };       
     });
            </script>
</asp:Content>


