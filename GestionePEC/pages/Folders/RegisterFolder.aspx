<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Mail.Master" Theme="Delta" AutoEventWireup="true" CodeBehind="RegisterFolder.aspx.cs" Inherits="GestionePEC.pages.Folders.RegisterFolder" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div id="pnlMain" class="content-panel-borderless">
        <div class="header-panel-blue">
            <div class="header-title">
                <div class="header-text-left">
                    <label>
                        REGISTRAZIONE CARTELLA</label>
                </div>
            </div>
        </div>
        <div id="container" class="control-main">
          <div class="control-body-gray">
          <div id="divNuovoFolder"></div>
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
         var formNuovo = Ext.create('Ext.form.Panel',
               {
                   frame: true,
                   title: 'Profilo Cartella',
                   bodyPadding: 10,
                   scrollable: true,
                   width: '100%',
                   id: 'formNuovaCartella',
                   //   layout: 'column',
                  // height: 250,
                   // xtype: 'fieldset',
                   autoWidth: false,
                   renderTo: 'divNuovoFolder',
                   items: [
                       {
                           fieldLabel: 'Nome Folder/Cartella',
                           xtype: 'textfield',
                           emptyText: 'Digitare il nome',
                           id: 'FolderText',
                           name: 'NomeFolder',
                           width: 700,
                           labelWidth: 130,
                           allowBlank: false,
                           blankText: requiredMessage,
                           msgTarget: 'under',
                           maxLength: 120,
                           enforceMaxLength: true,
                           maxLengthText: 'cartella campo massimo 120 caratteri',
                           padding: '5 0 0 0',
                           minLength: 3,
                           minLengthText: 'minimo 3 caratteri'
                       }],
                   buttons: [
                  {
                      text: 'Conferma creazione Cartella',
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
             var form = Ext.getCmp('formNuovaCartella');
             form.getForm().submit({
                 clientValidation: false,
                 // standardSubmit:true,
                 url: '/GestionePEC/api/FolderController/RegisterFolder',
                 waitTitle: 'Attendere prego',
                 method: 'POST',
                 success: function (form, action) {                    
                     if (action.result.message != null) {
                         var message = ' ';
                         message += action.result.message;
                         Ext.Msg.alert('Cartella creata correttamente', message);
                        // ManageError("Errore nell'aggiornameto dettagli: " + message);
                     }
                     else {
                         Ext.Msg.alert('Cartella creata correttamente', 'Collegare cartella/folder ad email');
                     }
                 },
                 failure: function (form, action) {
                     var form = Ext.getCmp('formNuovaCartella');
                     if (action.response.responseText != null) {
                         var message = Ext.decode(action.response.responseText).message;
                         if (message != null) {
                             Ext.Msg.alert('Errore creazione cartella', message);
                         }
                         else {
                             Ext.Msg.alert('Cartella creata', 'Collegare cartella/folder ad email');
                         }
                     }
                     else {
                         if (action.result != null) {
                             var message;                             
                                 message += action.result.errormessage[i];                            
                                 Ext.Msg.alert('Errore creazione cartella', message);
                         }
                     }                    
                 }
             });
             form.el.unmask();
         };       
     });
            </script>

</asp:Content>
