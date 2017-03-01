<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailTreeViewer.ascx.cs" Inherits="GestionePEC.Controls.MailTreeViewer" %>
<%@ Register Assembly="GestionePEC" Namespace="GestionePEC.Extensions" TagPrefix="inl" %>

<asp:Button ID="butMailViewer" runat="server" OnClick="butMailViewer_Click" />

<inl:InlineScript runat="server">
<script type="text/javascript">
    Ext.onReady(function () {
        var mailTree = Ext.create('Ext.tree.Panel', {
            useArrows: true,
            animate: true,
            autoHeight: false,
            height: 250,
            autoScroll: true,
            border: false,
            enableDD: false,
            loader: new Ext.data.TreeStore({
                clearOnLoad: true,
                dataUrl: '<%= Page.ResolveClientUrl("~/Ashx/MailTreeProvider.ashx") %>',
                requestMethod: 'POST',
                baseParams: {
                    idNode: ''
                }
            }),
            root: {
                draggable: false,
                text: 'Comunicazioni associate',
                id: 'ROOT',
                leaf: false
            },
            listeners: {
                click: function (n, e) {
                    if (n.id == 'ROOT') return false;
                    var idMailField = currentMailHF;
                    if (idMailField == null) return false;
                    if (idMailField.getAttribute('value') != n.id) {
                        idMailField.set({ value: n.id });
                        var but = Ext.get('<%= butMailViewer.ClientID %>');
                        but.dom.click();
                    }
                    modalMailTreeWin.hide();
                },
                expandnode: function (n) {
                    if (typeof currentMailId != 'undefined') {
                        if (n.id == currentMailId) {
                            n.select();
                        }
                    }
                }
            }
        });

        var modalMailTreeWin = new Ext.create('Ext.window.Window', {
            title: 'Albero delle comunicazioni',
            heigth: 250,
            width: 800,
            modal: true,
            layout: 'fit',
            renderTo: Ext.getBody(),
            closeAction: 'hide',
            items: mailTree,
            listeners: {
                beforeshow: function () {                   
                    var root = mailTree.getRootNode();
                    var loader = mailTree.getLoader();
                    var idMail = currentMailId;
                    if (root.isLoaded() == false) {
                        loader.baseParams.idNode = idMail;
                        root.expand(true, false);
                    }
                    else {
                        if (root.hasChildNodes()) {
                            debugger;
                            var nod = root.findChild('id', idMail, true);
                            if (nod == null) {
                                var node = root.item(0);
                                loader.baseParams.idNode = idMail;
                                loader.load(root,
                                    function (n) {
                                        if (n.hasChildNodes()) {
                                            root.replaceChild(n.item(0), node);
                                            root.expand(true, false);
                                        }
                                    });
                            }
                            else {
                                debugger;
                                if (nod.isSelected() == false) {
                                    nod.select();
                                }
                            }
                        }
                    }
                }
            }
        });

        var currentMailId = -1;
        var currentMailHF;
        function ShowMailTree(hfIdMail) {            
            if (typeof hfIdMail == 'undefined' || hfIdMail == null) return false;
            currentMailHF = hfIdMail;
            currentMailId = hfIdMail.getValue(true);
            if (currentMailId < 0) return false;
            modalMailTreeWin.show();
        };
    });
</script>
    </inl:InlineScript>
