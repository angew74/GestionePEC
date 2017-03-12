Ext.onReady(function () {   
	if(Ext.isEmpty(configResearch)) return;
	
	new Ext.form.ComboBox({
		id: 'cbxSubType',
		//typeAhead: true,
		width: 390,
		labelWidth: 100,
		fieldLabel: 'Ricerca per',
		editable: false,     
		triggerAction: 'all',
		transform: configResearch.cbxType,
		forceSelection: true,
		emptyText: 'selezionare un tipo...',
		mode: 'local',
		valueField: 'id',
		displayField: 'descr',
		store: new Ext.data.ArrayStore({
			autoDestroy: false,
			fields: [
				{ name: 'id', type: 'int' },
				{ name: 'descr', type: 'string' }
			],
			data: [
				[ 0, 'Mail' ],
				[3, 'Denominazione ente'],
                [4, 'Ufficio']
			]
		}),
		listeners: {
			select: function(){
				var	 cbx = Ext.getCmp('cbxEntita');
				cbx.setDisabled(false).reset();
				cbx.lastQuery = null;
			}
		}
	});

	Ext.define('EntitaModel', {
	    extend: 'Ext.data.Model',
	    fields: [{ name: 'id', type: 'string' },
				{ name: 'title', type: 'string' },
				{ name: 'text', type: 'string' },
				{ name: 'cls', type: 'string' }]
	});
	var reader = new Ext.data.JsonReader({
	    idProperty: 'id',
	    model: 'EntitaModel',
	    messageProperty: 'message',
	    rootProperty: 'Items',
	    totalProperty: 'totale'
	}
		);

	var proxy = new Ext.data.HttpProxy({
		url: configResearch.provider,
		method: 'GET',
		disableCaching: true,
		reader: reader,
		restful: true,
        model:'EntitaModel',
		headers: { 'Content-Type': 'application/json; charset=utf-8' }
	});
	
	var store = new Ext.data.Store({
		proxy: proxy,
		reader: reader,
	    model : 'EntitaModel',
		baseParams: {
		 //   type: '',
			start: 0,
			limit: 10
		},
		listeners: {
			beforeload: function(s, opt){
				var	subType = Ext.getCmp('cbxSubType').getValue();
				var	 type = '';
				switch(subType){
					case -1:
						return false;
						break;
					case 0:
						type = 'MAIL:ALL';
						break;
					case 3:
						type = 'RAGIONE_SOCIALE:PA:PA_SUB:ALL';
						break;
				    case 4:
				        type = 'UFFICIO:ALL';
				        break;
				}
			    //	s.setBaseParam('type',  type );
			//	s.baseParams.type = type;
				s.proxy.url = configResearch.provider + '?type=' + type;
				return true;
			},
			exception: function(){
			}
		}
	});
	
	new Ext.form.ComboBox({
		id: 'cbxEntita',
		disabled: true,
	//	typeAhead: false,
		width: 450,
		model:'EntitaModel',
		pageSize: 10,
		triggerAction: 'all',
		transform: configResearch.cbxEntita,
		forceSelection: false,
		emptyText: 'inserire un testo...',
	    //mode: 'remote',
		//queryMode: 'local',
		valueField: 'id',
		displayField: 'title',
		store: store,
		queryParam: 'text',
		queryDelay: 1000,
		autoSelect: false,
		typeAhead: true,
		minChars: 3,
		tpl: Ext.create('Ext.XTemplate',
'<ul class="x-list-plain"><tpl for=".">',
    '<li role="option" class="x-boundlist-item">{title}</li>',
'</tpl></ul>'),
		listConfig: {
		    loadMask: true,
		    loadingText: 'Attenzione ricerca ente in corso'
		},
		listeners: {
			beforeQuery: function(queryEvent){
				if(queryEvent.query == '') queryEvent.cancel = true;
				var length = queryEvent.query.length;
				queryEvent.query = queryEvent.query.replace('\'', '\'\'');
				queryEvent.query.length = length;
			},
			select: function(cbx, rec, idx){
				configResearch.hfEntitaType.set({ value: Ext.encode(rec.data) });
				ShowValueObject(rec.data, cbx);
			}
		}
	});
});

/* ente selected */
function ShowValueObject(values, obj){
	if(Ext.isEmpty(config) == false){
		var	 node = {
			attributes: {
				itemId: values.id,
				text: values.text,
				cls: values.cls
			}
		};
		if(values.id.indexOf(';') == -1){
			config.tree.valueSelectedAsNode = node;
			config.mappings.enteSelected = node;
		}
		var	 btnShowEntita = config.btnShowEntita;
		if(Ext.isEmpty(btnShowEntita) == false){
			btnShowEntita.on('click', ShowTree.bind(this, [ config.tree ]), this, {
				single: true
			});
			btnShowEntita.on('click', ShowMappings.bind(this, [ config.mappings ]), this, {
				single: true
			});
			btnShowEntita.dom.click();
		}
		
	}
}

/* tree loader */
var	 treeEnt;
function ShowTree(treeConfig) {
	function NodeLoaded(n){
		var	 idEnt = '';
		if(Ext.isEmpty(treeConfig[0].valueSelectedAsNode) == false){
			idEnt = treeConfig[0].valueSelectedAsNode.attributes.itemId;
			if(idEnt.indexOf('@') > -1){
				idEnt = idEnt.split('@')[1];
			}
		}
		if(n.id == 'ROOT') return;
		if(Ext.isEmpty(treeEnt) == false){
			var	node = treeEnt.getRootNode();
			var id = n.attributes.itemId;
			if(id.indexOf('@') > 0){
				id = id.split('@')[1];
			}
			if(id === idEnt){
				var nId = n.getPath().split('/');
				for(var i = 0; i < nId.length; i++){
					var ch = node.findChild('id', nId[i], false);
					if(ch != null){
						node = ch;
						ch.expand(false, true);
					}
				}
			}
		}
	};
	function NodeDblClicked(node, e){
		var id = node.attributes.itemId;
		if(id.indexOf('[') == -1){
			if(node.attributes.cls == 'GRP') return;
			config.tree.valueSelectedAsNode = node;
			config.mappings.enteSelected = node;
			var btn = treeConfig[0].btnShowTreeNode;
			btn.on('click', ShowMappings.bind(this, [ config.mappings ]))
			var	hiddenNode = treeConfig[0].selectedNode;
			if(Ext.isEmpty(hiddenNode) == false){
				var	 nodeSel = {
					id: node.attributes.itemId,
					text: node.attributes.text,
					cls: node.attributes.cls
				};
				hiddenNode.set({ value: Ext.encode(nodeSel) });
				if(Ext.isEmpty(btn) == false){
					btn.dom.click();
				}
			}
		}
	};
	function PreloadNodes(loader, node){
		node.eachChild(loader.doPreload, loader);
		for (var i = 0; i < node.childNodes.length; i++){
			PreloadNodes(loader, node.childNodes[i]);
		}
		NodeLoaded(node);
	};
	function CreateTree(treeConfig){
		//var treeEntLoader = new Ext.tree.TreeLoader({
		//	clearOnLoad: true,
		//	dataUrl: treeConfig.provider,
		//	requestMethod: 'POST',
		//	baseParams: {
		//		idNode: ''
		//	},
		//	listeners: {
		//		beforeload: function(t, n, c){
		//			t.baseParams.idNode = n.attributes.itemId;
		//		},
		//		load: PreloadNodes.bind(this)
		//	}
	    //});

	    if (!Ext.ClassManager.isCreated('TreeEntityModel')) {
	        Ext.define('TreeEntityModel', {
	            extend: 'Ext.data.Model',
	            fields: [
                    { name: 'itemId', type: 'string' },
                    { name: 'leaf', type: 'boolean' },
                    { name: 'text', type: 'string' },
                    { name: 'cls', type: 'string' },
                    { name: 'icon', type: 'string' },
                    { name: 'nodeType', type: 'string' },
	            ]
	        });
	    }

	    var treeEntLoader = Ext.create('Ext.data.TreeStore', {
	        storeId: 'TreeEntityStore',
	        clearOnLoad: true,
	        model: 'TreeEntityModel',
	        autoLoad: false,
	      //  nodeParam: treeConfig[0].valueSelectedAsNode.attributes.itemId,
	        proxy: {
	            type: 'ajax',
	            model: 'TreeEntityModel',
	             url: config.tree.provider,	       
	            reader: {
	                type: 'json',
	                rootProperty: 'JSONTreeNode'
	             //   ,totalProperty: 'TotalCount',
	            //    messageProperty: 'Message'
	            }
	        },
	        root: {
	            draggable: false,
	            text: treeConfig[0].valueSelectedAsNode.attributes.text,
	            id: treeConfig[0].valueSelectedAsNode.attributes.itemId.split('#')[0],
	            leaf: false,
	            expanded: true,
	            children: []
	        }
	    });


		//var rootNode = new Ext.tree.AsyncTreeNode({
		//	text: 'Rubrica',
		//	id: 'ROOT',
		//	draggable: false,
		//	editable: false,
		//	hidden: false,
		//	leaf: false,
		//	expandable: true
		//});
		// albero entità
	    var treeEnt =  Ext.create('Ext.tree.TreePanel',{
			renderTo: config.tree.divTree,
			useArrows: true,
			animate: true,
			enableDD: false,
			border: false,
		//	autoHeight: true,
			//autoScroll: false,
		//	containerScroll: false,
			ddScroll: true,
		//	root: rootNode,
		    //	loader: treeEntLoader,
			store: treeEntLoader,
			listeners: {
				dblclick: NodeDblClicked.bind(this),
				beforeChildrenrendered: function(n){
					for (var i = 0; i < n.childNodes.length; i++){
						n.childNodes[i].setTooltip(n.childNodes[i].text);
					}
				},
				expandnode: function(n){
					if(n.id == 'ROOT') return;
					var	 idEnt = '';
					if(Ext.isEmpty(treeConfig[0].valueSelectedAsNode) == false){
						idEnt = treeConfig[0].valueSelectedAsNode.attributes.itemId;
						if(idEnt.indexOf('@') > -1){
							idEnt = idEnt.split('@')[1];
						}
					}
					if(n.attributes.itemId.split('@')[1] == idEnt){
						n.select();
					}
				}
			}
		});
		//treeEnt.render();
		return treeEnt;
	};
//	if(Ext.isEmpty(treeEnt)){
	//	treeEnt = CreateTree(treeConfig);
    //	}
	treeEnt = CreateTree(treeConfig);
	var root = treeEnt.getRootNode();
	root.removeAll(true);	
	var	 node = treeConfig[0].valueSelectedAsNode;
	var	 idRef = node.attributes.itemId;
	if(idRef.indexOf(';') != -1){
		return; // ritrovamenti multipli
	}
	idRef = idRef.split('#')[0];
	// idEnt = idRef;
	if(idRef.indexOf('@') > 0){
		idEnt = idRef.split('@')[1];
	}
	var root = treeEnt.getRootNode();
	var	ch = root.findChild('itemId', idRef, true);
	if(ch != null){
		root.collapseChildNodes(true);
		NodeLoaded(ch);
	} else {
		root.removeAll(true);
		root.childrenRendered = false;
		root.loaded = false;
		root.data.itemId = idRef;
		root.expand(false, true);
	}
	var	 divStruttura = treeConfig[0].divStruttura;
	if(Ext.isEmpty(divStruttura) == false){
		if(divStruttura.isVisible() == false){
			divStruttura.setStyle('display', 'inline-block');
		}
	}
}
/* mappings */
function ShowMappings(mapConfig){
	if(Ext.isEmpty(mapConfig) || mapConfig.length == 0) return;
	var node = mapConfig[0].enteSelected;
	if(node.attributes.itemId.indexOf(';') != -1){
		return; // ritrovamenti multipli
	}
	
	if(node.attributes.cls != 'PA' && node.attributes.cls != 'PA_SUB'){
		return; // solo per PA e PA_SUB
	}
	
	var tpl = new Ext.XTemplate(
		'<tpl for=".">',
		    '<tpl for="EnteMapping">',
		        '<div class="divTabella">',
		            '<div class="control-header-blue tabellaCaption" style="margin-bottom: 5px;">',
		                '<div class="control-header-title">',
		                    '<div class="control-header-text-left">',
		                        '<h3>Comune di {values.Backend.Name} - Codice {values.Backend.BackendCode}</h3>',
		                        '<input type="hidden" name="hfCode" value="{values.Backend.Id}" />',
		                    '</div>',
		                '</div>',
		        	'</div>',			        
			        '<div style="display: table-header-group; margin-left: 10px;">',
			            '<div class="tabellaRow tabellaHead control-header-title">',
			                '<div class="tabellaCell tabellaFirstCell">',
			                    'Applicazione',
			                '</div>',
			                '<div class="tabellaCell">',
			                    'Mail',
			                '</div>',
			                '<div class="tabellaCell tabellaImgCell">',
			                '</div>',
			            '</div>',
			        '</div>',
			        '<tpl for="Mapping">',
			            '<div style="display: table-row-group">',
			                '<div class="tabellaRow" id="{xindex}">',
				                '<input type="hidden" name="hfMapping" value="{Id}" />',
			                    '<div class="tabellaCell tabellaFirstCell">',
			                        '{values.Titolo.Nome}',
			                        '<input type="hidden" name="hfTitolo" value="{values.Titolo.Id}" />',
			                    '</div>',
			                    '<div class="tabellaCell">',
			                    	'<tpl if="values.Contatto != null">',
			                        	'<span>{values.Contatto.mail}</span>',
			                        	'<input type="hidden" name="hfContatto" value="{values.Contatto.id}" />',
			                        '</tpl>',
			                    '</div>',
			                    '<div class="tabellaCell tabellaImgCell">',
				                    '<tpl if="this.isInRole(values.Titolo.Code)">',
				                        '<img alt="MOD" src="/CrabMail/App_Themes/Delta/images/buttons/modify.png" title="modifica" />',
				                        '<tpl if="values.Contatto != null && values.Contatto.id != null">',
				                        	'<img alt="CAN" src="/CrabMail/App_Themes/Delta/images/buttons/delete.png" title="cancella" />',
				                        '</tpl>',
				                        '<img class="imgHidden" alt="SAL" src="/CrabMail/App_Themes/Delta/images/buttons/done.png" title="salva" />',
				                        '<img class="imgHidden" alt="ANN" src="/CrabMail/App_Themes/Delta/images/buttons/back.png" title="annulla" />',
				                    '</tpl>',
			                	'</div>',
			            	'</div>',
			        	'</div>',
			    	'</tpl>',
				'</div>',
			'</tpl>',
    	'</tpl>', {
        compiled : true,
		disableFormat : true,
		roles : {},
		isInRole : function(appCode) {			
		    if (tpl.roles.dip == 52 || tpl.roles.dip == 152)
		        return true; //per tutte le abilitazioni
		    else		      
				return tpl.roles.roles.indexOf(appCode) != -1;
        }
    });

    var idEnte = node.attributes.itemId.split('#')[0];
	Ext.Ajax.request({
		url : mapConfig[0].provider + "/GetMappingsByEntita",
		method : 'GET',
		disableCaching: false,
		headers : {
			'Content-Type' : 'application/json; charset=utf-8'
		},
		params: {
			idEntita: idEnte
		},
		scope : this,
		success : function(resp, opt) {
			var data = Ext.util.JSON.decode(resp.responseText);
			ShowEnteMappings(data);
		},
		failure : function(resp, opt) {
			
			var	 prevDiv = mapConfig[0].divMappings.prev();
			ShowMessage(prevDiv, 'Error');
		}
	});
	
	function ShowEnteMappings(data){
		/* reset del controllo */
	    mapConfig[0].mappingContainer.clearListeners();
		mapConfig[0].mappingContainer.dom.innerHTML = '';
		mapConfig[0].divMappings.setVisibilityMode(Ext.Element.DISPLAY).setVisible(false, false);
		/* controllo dati */
		if(Ext.isEmpty(data.d)) return;
		tpl.roles = config.roles;
		var el = tpl.overwrite(mapConfig[0].mappingContainer, data.d, true);
		var divMappings = mapConfig[0].divMappings;
		if(divMappings.isVisible() == false){
			divMappings.setVisibilityMode(Ext.Element.DISPLAY).setVisible(true, true);
		}
		// define mapping store to pass to all combo
		var store = new Ext.data.Store({
			autoLoad : false,
			autoSave : false,
			baseParams : {
				start : 0,
				limit : 10
			},
			proxy : new Ext.data.HttpProxy({
				method : 'GET',
				url: config.mappings.provider + "/GetContattiEntita?idEntita=" + idEnte,
				disableCaching: true,
				headers : {
					'Content-Type' : 'application/json; charset=utf-8'
				},
				scope : this
			}),
			reader: new Ext.data.JsonReader({
				idProperty: 'id',
				root: 'd.data',
				totalProperty: 'd.totalCount'},
				[ 
					{ name: 'id', type: 'int' },
					{ name: 'mail', type: 'string' }
				]
			),
			storeId : 'mailStore',
			listeners : {
				beforeload: function(st) {
				//	st.setBaseParam( 'idEntita', idEnte );
				},
				exception : function(m) {
				}
			}
		});
		
		// click sulla div ( --> bubble)
		el.select('div.tabellaImgCell').each(function(el, co, idx) {
			el.on('click', imgClick, this, { store: store });
		}, this);
	}
	
	function imgClick(eve, target, opt) {
		var ele = Ext.get(target);
		var typeT = ele.getAttribute('alt');
		var divMail = ele.parent().prev();
		var	rowId = ele.parent('.tabellaRow').id;
		switch (typeT) {
			case 'MOD' :
				var curMail = '';
				var	curId = '';
				if(divMail.child('span') != null){
					divMail.child('span').setVisibilityMode(Ext.Element.DISPLAY)
						.setVisible(false);
					var curMail = divMail.child('span').dom.innerHTML;
				}
				if(divMail.child('input') != null){
					var curId = divMail.child('input').getValue();
				}
				if(typeof opt.store == 'undefined' || opt.store == null){
					return;
				}
				var cbx = new Ext.form.ComboBox({
					autoScroll : false,
					triggerAction : 'all',
					editable : false,
					forceSelection : true,
					name : 'comboMail',
					mode : 'remote',
					id: 'combo-' + rowId,
					pageSize : 10,
					renderTo : divMail,
					resizable : false,
					store : opt.store,
					queryParam: 'idEntita',
					width : 300,
					valueField : 'id',
					displayField : 'mail',
					listeners : {
						afterrender : function(cbx) {
						},
						expand : function(cbx) {
	
						},
						select: function(cbx, rec, idx){
							ele.next('img[alt="SAL"]').removeClass('imgHidden');
						}
					}
				});
				ele.addClass('imgHidden');
				if(Ext.isEmpty(ele.next('img[alt="CAN"]'))== false){
					ele.next('img[alt="CAN"]').addClass('imgHidden');
				}
				ele.next('img[alt="SAL"]').addClass('imgHidden');
				ele.next('img[alt="ANN"]').removeClass('imgHidden');
				break;
			case 'CAN' :
				var divTab = ele.parent('.divTabella');
				var	divRow = ele.parent('.tabellaRow');
				var mappings = { 
					IdCanale: 1,
					EnteMapping: [{
						Backend: {
							Id: Number(divTab.child('input[name="hfCode"]').getValue())
						},
						Mapping: [{
							Id: Number(divRow.child('input[name="hfMapping"]').getValue()),
							Titolo: {
								Id: Number(divRow.child('input[name="hfTitolo"]').getValue())
							},
							Contatto: {
								id: -1
							}
						}]
					}]
				};
				Ext.Ajax.request({
					url : mapConfig[0].provider + "/PutMapping",
					method : 'POST',
					disableCaching: false,
					headers : {
						'Content-Type' : 'application/json; charset=utf-8'
					},
					jsonData: {
						mapping: mappings
					},
					scope : this,
					success : function(resp, opt) {
						divRow.child('input[name="hfContatto"]').parent().dom.innerHTML = '';
						ele.setVisibilityMode(Ext.Element.DISPLAY).setVisible(false);
						ShowMessage(divRow, Ext.decode(resp.responseText).d.message);
					},
					failure : function(resp, opt) {
						ShowMessage(divRow, resp.responseText);
					}
				});
				break;
			case 'SAL' :
				var divTab = ele.parent('.divTabella');
				var	divRow = ele.parent('.tabellaRow');
				var	idMapping = Number(divRow.child('input[name="hfMapping"]').getValue());
				var url = '';
				if(idMapping == -1){
					url = mapConfig[0].provider + '/CreateMapping';
				} else {
					url = mapConfig[0].provider + "/PutMapping";
				}
				var mappings = { 
					IdCanale: 1,
					EnteMapping: [{
						Backend: {
							Id: Number(divTab.child('input[name="hfCode"]').getValue())
						},
						Mapping: [{
							Id: Number(divRow.child('input[name="hfMapping"]').getValue()),
							Titolo: {
								Id: Number(divRow.child('input[name="hfTitolo"]').getValue())
							},
							Contatto: {
								id: Ext.getCmp('combo-' + rowId).getValue()
							}
						}]
					}]
				};
				Ext.Ajax.request({
					url : url,
					method : 'POST',
					disableCaching: false,
					headers : {
						'Content-Type' : 'application/json; charset=utf-8'
					},
					jsonData: {
						mapping: mappings
					},
					scope : this,
					success : function(resp, opt) {
						var cbx = Ext.getCmp('combo-' + rowId);
						var	 mail = cbx.el.getValue();
						cbx.destroy();
						var	m = Ext.decode(resp.responseText);
						divRow.child('input[name="hfMapping"]').set({value: m.d.data[0].EnteMapping[0].Mapping[0].Id});
						var	div = divRow.select('.tabellaCell').item(1);
						var sp = Ext.DomHelper.append(div, {
							tag: 'span',
							html: mail
						}, true);
						sp.insertSibling({
							tag: 'input',
							type: 'hidden',
							value: m.d.data[0].EnteMapping[0].Mapping[0].Contatto.id,
							name: 'hfContatto'
						}, 'after');
						divRow.child('.tabellaImgCell').select('img').each(function(el){
							switch(el.getAttribute('alt')){
								case 'MOD':
								case 'CAN':
									el.removeClass('imgHidden');
									break;
								case 'SAL':
								case 'ANN':
									el.addClass('imgHidden');
									break;
							}
						});
						var divImg = divRow.child('.tabellaImgCell');
						if(divImg.select('img[alt="CAN"]').getCount() == 0){
							divImg.child('img[alt="MOD"]').insertSibling({
								tag: 'img',
								alt: 'CAN',
								src: '/CrabMail/App_Themes/Delta/images/buttons/delete.png',
								title: 'cancella'
							}, 'after');
						}
						ShowMessage(divRow, Ext.decode(resp.responseText).d.message);
					},
					failure : function(resp, opt) {
						ShowMessage(divRow, resp.responseText);
					}
				});
				break;
			case 'ANN' :
				var	 combo = Ext.getCmp('combo-' + rowId);
				if(typeof combo != 'undefined'){
					combo.destroy();
				}
				ele.parent().select('img').each(function(el){
					switch(el.getAttribute('alt')){
						case 'MOD':
						case 'CAN':
							el.removeClass('imgHidden');
							break;
						case 'SAL':
						case 'ANN':
							el.addClass('imgHidden');
							break;
					}
				});
				if(divMail.child('span') != null){
					divMail.child('span').setVisibilityMode(Ext.Element.DISPLAY)
						.setVisible(true);
				}
				break;
		}
	};
	
	function ShowMessage(divRow, msg){
		var el = Ext.DomHelper.insertAfter(divRow, {
			id: 'msgError',
			tag: 'div',
			html: msg
			}, true)
		.applyStyles({ 
			border: '2px solid #780808', 
			color: '#780808', 
			'font-weight': 'bold', 
			display: 'block'
			})
		.animate({
			opacity: {
				from: 0.5, to: 1
			}},
			3,
			function(){
				el.remove();
			},
			'easeOut'
		);
	}
};

//if (Sys && Sys.Application) {
//	Sys.Application.notifyScriptLoaded();
//};