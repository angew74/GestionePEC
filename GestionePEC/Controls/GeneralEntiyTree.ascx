<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GeneralEntiyTree.ascx.cs" Inherits="GestionePEC.Controls.GeneralEntiyTree" %>
<div id="tree-div">
</div>

<script type="text/javascript">
       
    Ext.onReady(function () {
        Ext.tip.QuickTipManager.init();  


        Ext.define('TreeAlberoModel', {
            extend: 'Ext.data.TreeModel',
            fields: [
                { name: 'text', type: 'string' },
                { name: 'itemId', type: 'string' },
                { name: 'cls', type: 'string' }
            ]
        });

        var AlberoStore = Ext.create('Ext.data.TreeStore', {
            storeId: 'TreeStore',
            clearOnLoad: true,
            model: 'TreeAlberoModel',
            autoLoad: true,
            proxy: {
                type: 'ajax',              
                model: 'TreeAlberoModel',
                url: '<%= Page.ResolveClientUrl(TreeViewDataSource) %>',
                reader: {
                    type: 'json',
                    //model: 'TreeAlberoModel',
                  //  idProperty: 'itemId',   
                    rootProperty: 'Items'
                 //   totalProperty: 'totale'
                    //  requestMethod:'POST'
                },
                headers: { 'Content-Type': 'application/json; charset=utf-8' }
            },
            listeners: {
                beforeload: function (t, n, cb) {
                    //   t.baseParams.idNode = n.attributes.itemId;
                    t.url = '<%= Page.ResolveClientUrl(TreeViewDataSource) %>' + '?node=' + n.node.id;
                   // t.baseParams.idNode = n.node.id;
                }
            },
            root: {
                text: 'Rubrica',
                draggable: false,
                itemId: 'RUBR@0',
                expanded: true,
                leaf: false,
                expanded: true,
                children: []
            }
        });


        var tree = Ext.create('Ext.tree.Panel', {
            renderTo: 'tree-div',
            useArrows: true,
            animate: true,
            height:'150',
            enableDD: false,
            store: AlberoStore,
            border: true,
            // rootProperty: {
            // nodeType: 'async',
            //    text: 'Rubrica',
            //  draggable: false,
            //  itemId: 'ROOT'
            // },         
            listeners: {
                // dblclick: TreeNodeClicked.createDelegate(this),
                dblclick: function (node, e) {
                    if (node.attributes.itemId.indexOf('[') == -1) {
                        if (node.attributes.cls == 'GRP') return;
                        if (Ext.isEmpty(config) == false) {
                            var nodeSel = {
                                id: node.attributes.itemId,
                                cls: node.attributes.cls,
                                text: node.attributes.text
                            };
                            config.tree.selectedNode.set({ value: Ext.encode(nodeSel) });
                            config.tree.valueSelectedAsNode = node;
                            config.mappings.enteSelected = node;
                            var btn = config.tree.btnShowTreeNode;
                            if (Ext.isEmpty(ShowTree) == false) {
                                btn.on('click', ShowTree.createDelegate(this, [config.tree]));
                                btn.on('click', ShowMappings.createDelegate(this, [config.mappings]));
                                btn.dom.click();
                            }
                        }
                    }
                },
                beforechildrenrendered: function (n) {
                    for (var i = 0; i < n.childNodes.length; i++) {
                        n.childNodes[i].setTooltip(n.childNodes[i].text);
                    }
                }
            }
        });

        tree.getRootNode().expand();

        function TreeNodeClicked(node, e) {
            if (node.attributes.itemId.indexOf('[') == -1) {
                if (node.attributes.cls == 'GRP') return;
                if (Ext.isEmpty(config) == false) {
                    var nodeSel = {
                        id: node.attributes.itemId,
                        cls: node.attributes.cls,
                        text: node.attributes.text
                    };
                    config.tree.selectedNode.set({ value: Ext.encode(nodeSel) });
                    config.tree.valueSelectedAsNode = node;
                    config.mappings.enteSelected = node;
                    var btn = config.tree.btnShowTreeNode;
                    if (Ext.isEmpty(ShowTree) == false) {
                        btn.on('click', ShowTree.createDelegate(this, [config.tree]));
                        btn.on('click', ShowMappings.createDelegate(this, [config.mappings]));
                        btn.dom.click();
                    }
                }
            }
        };
    });
</script>