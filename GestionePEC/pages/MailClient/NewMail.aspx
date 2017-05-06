<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Mail.Master" Theme="Delta" EnableEventValidation="false" ValidateRequest="false" AutoEventWireup="true" CodeBehind="NewMail.aspx.cs" Inherits="GestionePEC.pages.MailClient.NewMail" %>

<%@ Register Src="~/Controls/MailBoxLogin.ascx" TagName="MailBoxLogin" TagPrefix="mail" %>
<%@ Register Src="~/Controls/HeaderNewMail.ascx" TagName="UCHeader" TagPrefix="uc4" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">   
    <asp:HiddenField runat="server" ID="hfIdTitolo" />
    <asp:HiddenField runat="server" ID="hfIdReferral" />
    <div id="mainDivNascita" class="content-panel-borderless">
        <div class="header-panel-blue">
            <div class="header-title">
                <div class="header-text-left">
                    <label>
                        Invio Nuova Comunicazione</label>
                </div>
            </div>
        </div>
        <div id="container" class="body-panel">
            <div id="Div1" class="content-panel">
                <div class="header-panel-gray" id="Div2">
                    <div class="header-title">
                        <div class="header-text-left">
                            <asp:Label runat="server" ID="lblTitle" SkinID="lab_blue_bold_10">Comunicazione</asp:Label>
                        </div>
                    </div>
                </div>
                <div class="body-panel-gray">
                    <asp:UpdatePanel runat="server" ID="pnlLogin" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="Login" />
                        </Triggers>
                        <ContentTemplate>
                            <mail:MailBoxLogin ID="Login" runat="server" OnNewMail="Login_OnNewMail" OnChangeStatus="Login_OnChangeStatus"></mail:MailBoxLogin>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <uc4:UCHeader ID="HeaderNew" runat="server"></uc4:UCHeader>
                     <asp:Button ID="btnSend" runat="server" ToolTip="Invia la comunicazione" Text="Invia"
                         Style="margin-left:1000px"
                            OnClientClick="return GetValuesHtml();" ValidationGroup="email" OnClick="btnSend_OnClick" />
                    <div id="divTextArea" onkeyup="return GetValuesHtml();">
                    </div>
                    <div id="divUpload" />  
                            <asp:HiddenField ID="HidHtml" runat="server" />                 
                </div>
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
    'Ext.ux.upload.header.Base64FilenameEncoder',
    'Ext.ux.editor.myeditor'
            ]);
            Ext.tip.QuickTipManager.init();
       
            var appPanel = new Ext.panel.Panel({
                height: 250,              
                title: 'Gestione Allegati',
                renderTo: 'divUpload',
                scrollable:'true',
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

              new Ext.form.Panel ({             
             id: 'htmlEditor',
             title: 'Testo Comunicazione',
             renderTo: 'divTextArea',
             width: 1050,           
            height: 300,
            border:false,
                    //   frame: false,   
            bodyStyle: {
                padding: '5px'
            },
            layout: 'fit',
            header :{
                border:false
            },
            items: {
                xtype: 'htmleditor',
                enableColors: true,              
                enableAlignments: true,
                enableSourceEdit: true,
                enableFont: true,
                enableFontSize: true,
                enableFormat: true,
                frame:false,
                id:'TextHtml',
                width: 600,
                maxWidth: 1030,              
                padding: '0 0 0 -100',
                enabled:true,
                enableLinks: true,
                enableLists: true,
                enabled:true,
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
             

            Ext.getCmp('TextHtml').setReadOnly(false);
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



