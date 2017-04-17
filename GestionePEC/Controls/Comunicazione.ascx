<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Comunicazione.ascx.cs" Inherits="GestionePEC.Controls.Comunicazione" %>
<script type="text/jscript">
 
Ext.override(Ext.form.HtmlEditor, { 
  getDocMarkup : function(){ 
    var h = Ext.fly(this.iframe).getHeight() - this.iframePad * 2; 
    return '<html><head><style type="text/css">body{border: 0; margin: 0;' + String.format('padding: {0}px; height: {1}px;', this.iframePad, h) + ' cursor: text}</style></head><body></body></html>'; 
  },     
});  
   Ext.onReady(function () {  
   Ext.QuickTips.init();    
     new Ext.Panel({ 
        title: 'Testo Comunicazione',
          renderTo: '<%= this.ClientID +"_divTextArea"%>' ,    
        width: 1200,
        height: 250,  
        frame:false,      
        layout: 'fit',
        items: { 
            xtype: 'htmleditor', 
            enableColors: false,
            enableAlignments: false,
            name:'editor',
             value:'<%= TestoComunicazione %>'         
        } 
    });    
});
 
</script>

<div runat="server" id="divTextArea">
</div>
