<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Mail.Master" Theme="Delta" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="GestionePEC.Login" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="WestContentPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
      <div id="LoginPanel" style="position: absolute; top: 155px; left:500px; display: block"></div>       
  <script type="text/javascript">
      Ext.require([
    'Ext.form.Panel',
    'Ext.layout.container.Anchor'
      ]);

      Ext.onReady(function () {
          Ext.create('Ext.form.Panel', {
            //  xtype: 'form-login',
              renderTo: 'LoginPanel',
              iconCls: 'key',
              //<example>
              title: 'Autenticazione Utente',
              labelWidth: 100,
              //</example>             
              frame: true,
              width: 420,
              id:'LoginForm',
             // height:275,
              bodyPadding: 10,
              defaultType: 'textfield',                   
            // layout: 'absolute',
              items: [{
                  allowBlank: false,
                  fieldLabel: 'UserName',
                  name: 'username',
                  id: 'username',
                  blankText: 'I CAMPI SONO TUTTI OBBLIGATORI',
                  maxLength: 11,
                  minLength: 2,
                  msgTarget: 'under',
                  enforceMaxLength: true,
                  emptyText: 'inserire nome utente',
                  maxLengthText: 'utente campo massimo 11 caratteri',
                  minLengthText: 'utente campo minimo 2 caratteri',
              },{
                  allowBlank: false,
                  fieldLabel: 'Password',
                  minLength: 8,
                  maxLength: 16,               
                  enforceMaxLength: true,
                  blankText: 'I CAMPI SONO TUTTI OBBLIGATORI',
                  name: 'password',
                  id:'password',
                  msgTarget: 'under',
                  emptyText: 'inserire la password',
                  maxLengthText:'password campo MASSIMO di 16 caratteri',
                  minLengthText:'password campo MINIMO di 8 caratteri',
                  inputType: 'password'
              }],
              buttons: [
                  {
                      text: 'Accedi',
                      formBind: true,                    
                      listeners:
                      {
                          click: function () {
                              loginLogic();
                          }
                      }
                  },
                  {
                      text: 'Azzera Campi',
                      handler:function()
                      {
                          button.up('form').getForm().reset();
                      }
                  }
              ]              
          });

          Ext.getCmp('username').focus();

          function loginLogic() {
              var form = Ext.getCmp('LoginForm');
              var user =Ext.getCmp('LoginForm').items.get(0).value;            
              var pass = Ext.getCmp('LoginForm').items.get(1).value;
              var urlApi  = '/GestionePEC/api/LoginServiceController/DoLogin/'+user+'/'+pass;
              form.el.mask('Attendere prego...', 'x-mask-loading');
              if (form.isValid()) {
                  form.getForm().submit({                     
                      url: urlApi,
                      waitMsg: 'Autenticazione in corso',
                      params: {
                          username: user,                         
                          password: pass
                      },
                      method:'GET',                     
                      success: function (form, action) {                          
                          if (Ext.decode(action.response.responseText).success == "true")
                          {
                              location.href = Ext.decode(action.response.responseText).ResponseUrl;
                          } 
                         
                      },
                      failure: function (form, action) {                        
                          if (Ext.decode(action.response.responseText).success == "false") {
                              Ext.Msg.alert('Error',
                                  'Dettaglio: ' + Ext.decode(action.response.responseText).Error)
                          }                                               
                      }
                  })
              }
              form.el.unmask();
          };
      });
     
  </script> 
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="RightContentPlaceHolder" runat="server">
</asp:Content>
