<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Mail.Master" AutoEventWireup="true" Theme="Delta" CodeBehind="ChangeSettings.aspx.cs" Inherits="GestionePEC.pages.Users.ChangeSettings" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
        <div id="pnlMain" class="content-panel-borderless">
        <div class="header-panel-blue">
            <div class="header-title">
                <div class="header-text-left">
                    <label>
                        CAMBIA IMPOSTAZIONI</label>
                </div>
            </div>
        </div>
        <div id="container" class="control-main">
            <div class="control-body-gray">
          <div id="divUtente"></div>
            <div id="p3"></div>
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

              var rolesUser = {
                  xtype: 'dataview',
                  store: {
                      fields: ['Id', 'Name']
                  },
                  id: 'RolesList',
                  itemTpl: '<div>{Name}</div>',
                  title: 'Ruoli abilitati',
                  frame: true,
                  collapsible: true,
                  emptyText: 'Nessuna ruolo da mostrare'
              };

              var mailsUser = {
                  xtype: 'dataview',
                  store: {
                      fields: ['emailAddress', 'idMail']
                  },
                  id: 'MailsList',
                  itemTpl: '<div>{emailAddress}</div>',
                  title: 'Caselle abilitate',
                  frame: true,
                  collapsible: true,
                  emptyText: 'Nessuna casella da mostrare'
              };

              var progBar = Ext.create('Ext.ProgressBar');
              new Ext.form.Panel({
                  renderTo: 'divUtente',
                //  width: 1000,
                //  height: 670,
                  title: 'Profilo Utente',
                  id:'formUtente',
                  defaultType: 'textfield',
                  bodyPadding: 10,
                  defaults: {
                      labelWidth: 100
                  },
                  items: [{
                      fieldLabel: 'Cognome',
                      xtype: 'textfield',
                      emptyText: 'Digitare il cognome',
                      id: 'CognomeText',
                      name: 'Cognome',
                      width: 700,
                      labelWidth: 170,
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
                      labelWidth: 170,
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
                      xtype: 'displayfield',
                      emptyText: 'Digitare username',
                      width: 700,
                      labelWidth: 170,
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
                      labelWidth: 170,
                      allowBlank: true,
                      padding: '5 0 0 0',
                      blankText: requiredMessage,
                      msgTarget: 'under',
                      maxLength: 16,
                      enforceMaxLength: true,
                      maxLength: 'username  campo massimo 16 caratteri',
                      minLengthText: 'minimo 16 caratteri',
                      minLength: 16
                  }, {
                      fieldLabel: 'Nuova Password',
                      name: 'Password',
                      id: 'PasswordText',
                      xtype: 'textfield',
                      inputType: 'password',
                      emptyText: 'Digitare la password',
                      width: 700,
                      labelWidth: 170,
                      allowBlank: true,
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
                      fieldLabel: 'Conferma Nuova Password',
                      name: 'ConfirmPassword',
                      id: 'CPasswordText',
                      xtype: 'textfield',
                      inputType: 'password',
                      emptyText: 'conferma la password',
                      width: 700,
                      labelWidth: 170,
                      allowBlank: true,
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
                          var formPanel = Ext.getCmp("formUtente").getForm();
                          // Save the fields we are going to insert values into
                          var pass1 = Ext.getCmp('PasswordText').getValue();
                          var pass2 = Ext.getCmp('CPasswordText').getValue();
                          if (pass1 == pass2)
                              return true;

                          else
                              return "Passwords non coincidono!";
                      }
                  }, {
                      fieldLabel: 'Ruolo utente Mail',
                      name: 'RoleMailDesription',
                      width: 700,
                      labelWidth: 170,
                      xtype:'displayfield',
                      editable:'false',
                      padding: '5 0 0 0',
                  },{
                      name: 'Domain',
                      fieldLabel: 'Dominio attività',
                      width: 700,
                      labelWidth: 170,
                      padding: '5 0 0 0',
                  },
                  {
                      name: 'innerPanel',
                      xtype: 'panel',
                      title: 'Email abilitate',
                      border: false,
                      // width:750,
                      padding: '0 0 0 0',
                      items: [
                          mailsUser]
                  }, {
                      name: 'innerPanelRole',
                      xtype: 'panel',
                      title: 'Ruoli applicativi abilitati',
                      border: false,
                      // width:750,
                      padding: '5 0 25 0',
                      items: [rolesUser]
                  }],
                  buttons:[ {
                      text: 'Conferma',
                      id: 'btnConferma',
                      scope: this,
                      formBind: true,
                      xtype: 'button',
                      padding: '0 0 0 0',
                      listeners: {
                          click: function () {
                              // this == the button, as we are in the local scope
                              sendUser();
                          }
                      }
                  }
                  ]
              });
              var urlSendUser = '/GestionePEC/api/UsersServiceController/UpdateOwnProfile';
              var UrlUserExt = '/GestionePEC/api/UsersServiceController/GetOwnProfile';
              function sendUser()
              {
                  var form = Ext.getCmp('formUtente');
                  form.el.mask('Attendere prego...', 'x-mask-loading');
                  var formData = form.getForm().getFieldValues();
                  /* Normally we would submit the form to the server here and handle the response...*/
                  form.getForm().submit({
                      clientValidation: false,
                      data: formData,
                      url: urlSendUser,
                      waitTitle: 'Attendere prego',
                      method: 'POST',
                      success: function (form, action) {
                          var button = Ext.getCmp('btnConferma');
                          button.disabled = true;
                          var form = Ext.getCmp('formUtente');
                          if (Ext.decode(action.response.responseText).ErrorMessage != null) {
                              Ext.Msg.alert('Errore Elaborazione', Ext.decode(action.response.responseText).ErrorMessage);
                          }
                          else if (Ext.decode(action.response.responseText).Message != null) {
                              Ext.Msg.alert('Errore Elaborazione', (Ext.decode(action.response.responseText).Message));
                          }
                          else {
                              form.el.unmask();
                              Ext.Msg.alert('Profilo modificata', 'Le variazioni sono state registrate sulla banca dati');
                          }
                      },
                      failure: function (form, action) {
                          var form = Ext.getCmp('formUtente');
                          if (Ext.decode(action.response.responseText).ErrorMessage != null) {
                              Ext.Msg.alert('Errore Elaborazione', "Errore nell'aggiornameto contattare servizio tecnico dettagli: " + Ext.decode(action.response.responseText).ErrorMessage);
                          }
                          form.el.unmask();
                      }
                  });
              }
           
            


              Ext.Ajax.setTimeout(120000); // 120 seconds
        
            var msgBox = Ext.MessageBox.show({
                  msg: 'Caricamento profilo utente, attendere prego...',
                  progressText: 'Caricamento...',
                  id:'MessageLoading',
                  width: 300,
                  wait: true,
                  waitConfig:
                  {
                      duration: 50000,
                      increment: 15,
                      text: 'Caricamento...',
                      scope: this
                  }
              });
        
              Ext.Ajax.request({
                  url: UrlUserExt,
                  loadMask: {msg: 'Attender prego...'},
                  success: function (response, opts) {
                      var objUser = Ext.decode(response.responseText);
                      Ext.getCmp('formUtente').getForm().setValues(objUser.OwnProfiles[0]);
                      var mails = objUser.OwnProfiles[0].MappedMails;
                      var roles = objUser.OwnProfiles[0].Roles;
                      for (var i = 0; i < mails.length; i++) {
                          var emailaddress = mails[i].emailAddress;
                          var idmail = mails[i].idMail;
                          Ext.getCmp('MailsList').getStore().add({
                              emailAddress: emailaddress,
                              idMail: idmail
                          });
                      }
                      for (var k = 0; k < roles.length; k++) {
                          var nome = roles[k].Name;
                          var id = roles[k].Id;
                          Ext.getCmp('RolesList').getStore().add({
                              Name: nome,
                              Id: id
                          });
                      }
                      msgBox.hide();
                  },
                  failure: function (response, opts) {
                   //   Ext.Viewport.setMasked(false);  //hide the mask  loaded ...
                      Ext.Msg.alert('Errore Elaborazione', "Errore nel caricamento contattare servizio tecnico dettagli: " + Ext.decode(action.response.responseText).ErrorMessage);
                  }
              });
          });
          </script>
</asp:Content>
