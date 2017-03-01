
Ext.namespace('Ext.ux.plugins');

Ext.ux.plugins.FitToParent = Ext.extend(Object, {
    constructor: function (parent) {
        this.parent = parent;
    },
    init: function (c) {
        c.on('render', function (c) {
            c.fitToElement = Ext.get(this.parent || c.getDomPositionEl().dom.parentNode);
            if (!c.doLayout) {
                this.fitSizeToParent();
                Ext.on('resize', this.fitSizeToParent);
            }
        }, this, { single: true });
        if (c.doLayout) {
            c.monitorResize = true;
            c.doLayout = c.doLayout.createInterceptor(this.fitSizeToParent);
        }
    },
    fitSizeToParent: function () {
        var pos = this.getPosition(true), size = this.fitToElement.getViewSize();
        this.setSize(size.width - pos[0], size.height - pos[1]);
    }
});

Ext.define('Ext.FormViewport', {
    extend: 'Ext.container.Viewport',
    initComponent: function () {
        Ext.container.Viewport.superclass.initComponent.call(this);
        document.getElementsByTagName('html')[0].className += ' x-viewport';
        this.el = Ext.get(document.forms[0]);
        this.el.setHeight = Ext.emptyFn;
        this.el.setWidth = Ext.emptyFn;
        this.el.setSize = Ext.emptyFn;
        this.el.dom.scroll = 'no';
        this.allowDomMove = false;
        this.autoWidth = true;
        this.autoHeight = true;
        this.renderTo = this.el;
        Ext.on('resize', this.fireResize);
    },
    fireResize: function (w, h) {
        this.fireEvent('resize', this, w, h, w, h);
    }
});


function CreateViewPort(titleCenter, iconCenter, callFromLoginPage, showmenu, showRightPanel) {
    Ext.onReady(function () {
        Ext.tip.QuickTipManager.init();
        Ext.state.Manager.setProvider(new Ext.state.CookieProvider());
        //  Ext.define('Viewport', Ext.FormViewport);
        //  Ext.reg('FormViewport', Ext.FormViewport);
        var _pnlSouth = null;
        var Height_SouthMessage = 300;
        var Height_PanelSouth = 20;

        var pnlNorth = CreatePanelNorth();
        var pnlCenter = CreatePanelCenter(titleCenter, iconCenter, showmenu);
       // var pnlSouth = CreatePanelSouthMessage();
        var viewport;
        if (showmenu && !showRightPanel) {
            var pnlWest = CreatePanelWest();
            viewport = Ext.create('Ext.FormViewport', {
                layout: 'border',
                id: 'vmain',
                monitorResize: true,
                border: false,
                items: [pnlNorth, pnlWest, pnlCenter]
            });
        }
        else if (showmenu && showRightPanel) {
            var pnlWest = CreatePanelWest();
            var pnlEast = CreatePanelEast();
            viewport = Ext.create('Ext.FormViewport', {
                layout: 'border',
                id: 'vmain',
                monitorResize: true,
                border: false,
                items: [pnlNorth, pnlWest, pnlEast, pnlCenter]
            });
        }
        else {
            var pnlWestHidden = CreatePanelWestHidden();
            viewport = Ext.create('Ext.FormViewport', {
                layout: 'border',
                id: 'vmain',
                monitorResize: true,
                border: false,
                items: [pnlNorth, pnlCenter]
            });
        }
        // viewport.removeCls('x-border-box');
        document.body.style.visibility = "visible";
    });
}

function CreatePanelNorth() {
    var pnlNorth = Ext.create('Ext.panel.Panel', {
        region: 'north',
        title: 'North',
        header: false,
        id: 'nord-region',
        contentEl: 'north',
        collapsible: false,
      //  border: false,
        split: false,
        layout: 'fit',
        minSize: 180,
        maxSize: 180,
        margin: '0 0 6 0',
        bodyStyle: 'background-color:#CECECE;'
    });

    return pnlNorth;

}

function CreatePanelWestHidden() {

    var pnlWestHidden = Ext.create('Ext.panel.Panel', {
        region: 'west',
        title: 'Menu',
        id: 'west-region-hidden',
        contentEl: 'west',
        split: false,
        width: 0,
        minSize: 0,
        autoScroll: false,
        collapsible: false,
        margin: '0 0 0 0',
        layout: 'fit',
        layoutConfig: {
            animate: false
        }

    });

    return pnlWestHidden;
}


function CreatePanelWest() {
    var pnlWest = Ext.create('Ext.panel.Panel', {
        region: 'west',
        title: 'Menu',
       // stateId: 'navigation-panel',
        id: 'west-region',
        contentEl: 'west',
        split: true,
        autoWidth: true, //ciro 21/05/2016
        width: 200,
        minSize: 175,
        maxSize: 400,
        //width: 250,
       // minWidth: 175,
        maxWidth: 400,
        collapsible: true,
      //  animCollapse: true,
       // border: true,
        autoScroll: true,
        margins: '0 0 6 6',
        defaults: {
            layout: 'fit',
            animate: false
        }
    });

    return pnlWest;
}

function CreatePanelWestHidden() {

    var pnlWestHidden = new Ext.Panel({
        region: 'west',
        title: 'Menu',
        id: 'west-region-hidden',
        contentEl: 'west',
        split: false,
        width: 0,
        minSize: 0,
        autoScroll: false,
        collapsible: false,
        margins: '0 0 6 6',
        layout: 'fit',
        layoutConfig: {
            animate: false
        }

    });

    return pnlWestHidden;
}


function CreatePanelSouthMessage() {
    var pnlSouth = new Ext.panel.Panel({
        region: 'south',
        title: 'Messaggi...',
        id: 'south-region',
        contentEl: 'south',
        split: true,
        collapsed: true,
        height: 100,
        minSize: 300,
        maxSize: 300,
        collapsible: true,
        margin: '0 0 0 0',
        iconCls: 'icon-alert',
        layout: 'fit',
        layoutConfig: {
            animate: true
        }
        //cmargins:'5px 0px 5px 5px'
    });

    return pnlSouth;

}

function CreatePanelEast() {

    var pnlEast = Ext.create('Ext.panel.Panel', {
        region: 'east',
        title: 'Richiesta',
        id: 'east-region',
        contentEl: 'east',
        collapsible: true,
        split: true,
        width: 225,
        minSize: 175,
        maxSize: 400,
        layout: 'fit',
        margin: '0 6 6 -6',
        layoutConfig: {
            animate: true
        }
    });

    return pnlEast;

}

function CreatePanelCenter(titleCenter, icon, showmenu) {
    var autoScroll = true;
    if (Ext.isIE6 || Ext.isIE7)
        autoScroll = false;
    var hideMargin = Boolean(showmenu);
    var margini = '0 6 6 0';

    if (!hideMargin)
        margini = '0 6 6 6';


    var pnlCenter = Ext.create('Ext.panel.Panel', {
        region: 'center',
        title: titleCenter,
        //header:showTitle,
        header: false,
        id: 'center-region',
        contentEl: 'center',
        layout: 'fit',
        monitorResize: true,
        iconCls: icon,
        border: false,
        autoScroll: true, //Boolean(autoScroll),
        //collapsible: true,
        //width: 180,
        //minSize: 180,
        //maxSize: 180,
        //bodyStyle: 'width:100%; overflow:auto;',
        margin: margini
        //cmargins:'2px 2px 2px 2px'
    });

    return pnlCenter;

}