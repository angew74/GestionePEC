<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailViewer.ascx.cs" Inherits="GestionePEC.Controls.MailViewer" %>
<%@ Register Assembly="GestionePEC" Namespace="GestionePEC.Extensions" TagPrefix="inl" %>
<%@ Import Namespace="ActiveUp.Net.Common.DeltaExt" %>
<asp:Button ID="butMailViewer" runat="server" OnClick="butMailViewer_Click" />
<div id="MailViewer" class="control-main">
    <div class="control-header-blue">
        <div class="header-title">
            <div class="header-text-left">
                <asp:HiddenField runat="server" ID="hfIdMail" />
                <asp:HiddenField runat="server" ID="hfUIDMail" />
                <asp:HiddenField runat="server" ID="hfAccount" />
                <asp:HiddenField runat="server" ID="hfCurrFolder" />
                <asp:HiddenField runat="server" ID="hfParFolder" />
                <% if (EnableReplyAll)
                   { %>
                <asp:LinkButton ID="ReplyAllButton" runat="server" OnCommand="Action_WorkOn" CommandName="Reply_All"
                    CssClass="mailReplyAll" Style="margin-right: 5px" ToolTip="Rispondi a tutti"
                    Visible="<%# ButtonVisible[HeaderButtons.ReplyAll] %>">
                </asp:LinkButton>
                <% } %>
                <% if (EnableReplyTo)
                   { %>
                <asp:LinkButton ID="ReplyButton" runat="server" OnCommand="Action_WorkOn" CommandName="Reply_To"
                    CssClass="mailReplyTo" Style="margin-right: 5px" ToolTip="Rispondi" Visible="<%# ButtonVisible[HeaderButtons.ReplyTo] %>">
                </asp:LinkButton>
                <% } %>
                <% if (EnableForward)
                   { %>
                <asp:LinkButton ID="ForwardButton" runat="server" OnCommand="Action_WorkOn" CommandName="Forward"
                    CssClass="mailForward" Style="margin-right: 5px" ToolTip="Inoltra" Visible="<%# ButtonVisible[HeaderButtons.Forward]%>">
                </asp:LinkButton>
                <% } %>
                <% if (EnableRating)
                   { %>
                <a runat="server" id="txtRating" style="float: left; margin-left: 5px; cursor: pointer"
                    visible="<%# ButtonVisible[HeaderButtons.Rating]%>" onclick="vote(0); return false;">Status:</a>
                <ul class='star-rating' id="imgRating" runat="server" visible="<%# ButtonVisible[HeaderButtons.Rating]%>">
                    <li class="current-rating" id="rating" style="width: 0px" runat="server">0</li>
                    <li><a href="#" onclick="vote(1); return false;" title='1 di 4' class='one-star'>1</a></li>
                    <li><a href="#" onclick="vote(2); return false;" title='2 di 4' class='two-stars'>2</a></li>
                    <li><a href="#" onclick="vote(3); return false;" title='3 di 4' class='three-stars'>3</a></li>
                    <li><a href="#" onclick="vote(4); return false;" title='4 di 4' class='four-stars'>4</a></li>
                </ul>
                <% } %>
            </div>
            <div class="header-text-right">
                <asp:UpdatePanel ID="pnlStampa" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                    <Triggers>
                        <asp:PostBackTrigger ControlID="lbStampaPdf" />
                        <asp:PostBackTrigger ControlID="lbDownload" />
                        <asp:PostBackTrigger ControlID="lbStampaPDU" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:LinkButton ID="lbDownload" runat="server" CssClass="download-mail" ToolTip="Download della mail"
                            OnClick="lbDownload_Click" Visible="false" />
                        <asp:LinkButton ID="lbStampaPdf" runat="server" CssClass="PrintBodyMail" ToolTip="Stampa la mail"
                            OnClick="lbStampaPdf_Click" />
                        <asp:LinkButton ID="lbStampaPDU" runat="server" CssClass="PrintPDUBodyMail" ToolTip="Stampa la mail"
                            OnClick="lbStampaPDU_Click" />
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:LinkButton ID="lnbSMBack" runat="server" OnClick="SubMessage_Back_Click" CssClass="fatherMail"
                    ToolTip="Torna alla Mail Iniziale" />
                <% if (EnableAcquire)
                   { %>
                <asp:LinkButton ID="libuAction" runat="server" OnCommand="Action_WorkOn" CommandName="Acquire"
                    CssClass="mailAcquire" ToolTip="Acquisisci Mail" Visible="<%# ButtonVisible[HeaderButtons.Acquire] && (String.IsNullOrEmpty(this.rating.InnerHtml) || (int.Parse(this.rating.InnerHtml) == 0)) %>"></asp:LinkButton>
                <% } %>
                <asp:ImageButton ID="ibReSend" runat="server" OnCommand="Action_WorkOn" CommandName="Re_Send"
                    ImageUrl="~/App_Themes/unisys/images/mail_set/send-again.png" ImageAlign="Middle"
                    ToolTip="Rinvia la mail" Visible='<%# ButtonVisible[HeaderButtons.ReSend] && !String.IsNullOrEmpty(this.rating.InnerHtml) && (int.Parse(this.rating.InnerHtml) > 20) %>' />
                <% if (EnableMailTree)
                   { %>
                <asp:ImageButton ID="ibShowMailTree" runat="server" ImageUrl="~/App_Themes/unisys/images/tree/view-tree.png"
                    ImageAlign="Middle" ToolTip="Visualizza l'albero delle comunicazioni" Visible="<%# ButtonVisible[HeaderButtons.MailTree]%>" />
                <% } %>
            </div>
        </div>
    </div>
    <div class="control-body-gray">
        <% if (CurrentMessage != null)
           { %>
        <div style="display: table; padding-bottom: 3px">
            <div style="display: table-row">
                <div style="display: table-cell">
                    <label class="labelBlack">
                        Mittente:</label>
                </div>
                <div style="display: table-cell">
                    <label class="LabelBlack">
                        <%= HttpUtility.HtmlEncode(CurrentMessage.From.Merged)%></label>
                </div>
            </div>
            <div style="display: table-row">
                <div style="display: table-cell">
                    <label class="labelBlack">
                        Destinatario:</label>
                </div>
                <div style="display: table-cell">
                    <label class="LabelBlack">
                        <%= HttpUtility.HtmlEncode(CurrentMessage.To.Merged)%></label>
                </div>
            </div>
            <div style="display: table-row">
                <div style="display: table-cell">
                    <label class="labelBlack">
                        CC:</label>
                </div>
                <div style="display: table-cell">
                    <label class="LabelBlack">
                        <%= HttpUtility.HtmlEncode(CurrentMessage.Cc.Merged)%></label>
                </div>
            </div>
            <div style="display: table-row">
                <div style="display: table-cell">
                    <label class="labelBlack">
                        Oggetto:</label>
                </div>
                <div style="display: table-row">
                    <label class="LabelBlack">
                        <%= HttpUtility.HtmlEncode(CurrentMessage.Subject)%></label>
                </div>
            </div>
        </div>
        <%} %>
        <asp:Panel ID="PnlAttachment" runat="server" class="Expanding-Blue-Header">
            <asp:UpdatePanel runat="server" ID="pnlAllegati" RenderMode="Block" UpdateMode="Conditional">
                <Triggers>
                    <asp:PostBackTrigger ControlID="Repeater1" />
                </Triggers>
                <ContentTemplate>
                    <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater_ItemDataBound"
                        OnItemCommand="Repeater1_ItemCommand">
                        <ItemTemplate>
                            <div class="AttchmentItem">
                                <asp:LinkButton ID="test" CommandArgument='<%# Eval("FileName") %>' runat="server"
                                    CommandName="Download"><%# Eval("FileName") %></asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
        <asp:Panel ID="PnlEmbeddedElements" runat="server" class="Expanding-Blue-Header">
            <asp:UpdatePanel runat="server" ID="pnlEmbed" RenderMode="Block" UpdateMode="Conditional">
                <Triggers>
                    <asp:PostBackTrigger ControlID="Repeater2" />
                </Triggers>
                <ContentTemplate>
                    <asp:Repeater ID="Repeater2" runat="server" OnItemDataBound="Repeater_ItemDataBound"
                        OnItemCommand="Repeater2_ItemCommand">
                        <ItemTemplate>
                            <div class="AttchmentItem">
                                <asp:LinkButton ID="test" CommandArgument='<%# Eval("FileName") %>' runat="server"
                                    CommandName="Download" Text='<%# Eval("FileName") %>' />
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
        <asp:Panel ID="PnlInnerMail" runat="server" class="Expanding-Blue-Header">
            <asp:Repeater ID="rptSubMessage" runat="server" OnItemCommand="rptSubMessage_ItemCommand"
                OnItemDataBound="rptSubMessage_ItemDataBound">
                <ItemTemplate>
                    <div class="InnerMailItem">
                        <asp:LinkButton ID="rptsubMsg" runat="server" CommandName="Enter" Text='<%# "from:"+ Eval("From.Merged") + " - " + Eval("Subject") %>'></asp:LinkButton>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </asp:Panel>
        <div id="MailContent" runat="server" visible="true" class="control-body">
        </div>
    </div>
</div>
<inl:InlineScript runat="server">
    <script type="text/javascript">
        var currentMailId = -1;
        var currentMailHF;

        if (!Ext.ClassManager.isCreated('TreeAlberoModel')) {
            Ext.define('TreeAlberoModel', {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'leaf', type: 'boolean' },
                    { name: 'text', type: 'string' },
                    { name: 'icon', type: 'string' },
                    { name: 'source', type: 'string' },
                    { name: 'id', type: 'string' },
                ]
            });
        }

        var AlberoStore = Ext.create('Ext.data.TreeStore', {
            storeId: 'TreeStore',
            clearOnLoad: true,
            model: 'TreeAlberoModel',
            autoLoad: false,
            proxy: {
                type: 'ajax',
                model: 'TreeAlberoModel',
                url: '<%= Page.ResolveClientUrl("~/api/Mail/GetTreeStructure") %>' + '?idM=' +  document.getElementById('<%= hfIdMail.ClientID %>').value,
                reader: {
                    type: 'json',
                    rootProperty: 'data',                  
                    totalProperty: 'TotalCount',
                    messageProperty: 'Message'
                }
            },
            root: {
                draggable: false,
                text: 'Comunicazioni associate',
                id: 'id',
                leaf: false,
                expanded: true,
                children: []
            }
        });  

        function ShowMailTree(hfIdMail) {          
            if (typeof hfIdMail == 'undefined' || hfIdMail == null) return false;
            currentMailHF = hfIdMail;
            currentMailId = hfIdMail.getValue(true);
            if (currentMailId < 0) return false;      

            var modalMailTreeWin = new Ext.create('Ext.window.Window', {
                title: 'Albero delle comunicazioni',
                heigth: 280,
                width: 800,
                modal: true,
                id: 'WindowTree',
                layout: 'fit',
                renderTo: Ext.getBody(),
                closeAction: 'hide',
                items: {
                    xtype: 'treepanel',
                    useArrows: true,
                    animate: true,
                    autoHeight: false,
                    height: 250,
                    autoScroll: true,
                    id: 'MailAlbero',
                    store: AlberoStore,
                    border: false,
                    enableDD: false,
                    //  loader: AlberoStore,
                    listeners: {
                        itemclick: function (treeModel, record, item, index, e, eOpts) {
                            if (record.id == 'ROOT') return false;
                            var idMailField = document.getElementById('<%= hfIdMail.ClientID %>').value;                           
                            if (idMailField == null) return false;
                            if (idMailField != record.id) {
                                document.getElementById('<%= hfIdMail.ClientID %>').value = record.id;
                                var but = Ext.get('<%= butMailViewer.ClientID %>');
                                but.dom.click();
                                Ext.getCmp('MailAlbero').destroy();
                                Ext.getCmp('WindowTree').destroy();
                            }                                             
                        }
                    }},
                    listeners:
                    {
                        close: function () {
                            Ext.getCmp('MailAlbero').destroy();
                            Ext.getCmp('WindowTree').destroy();
                        }
                    }                   
            });

          <%--  Ext.getCmp('MailAlbero').store.proxy.url = '<%= Page.ResolveClientUrl("~/api/Mail/GetTreeStructure") %>' + '?idM=' + currentMailId;
            Ext.getCmp('MailAlbero').store.load();
            Ext.getCmp('MailAlbero').getStore().on('load', function (store, eOpts) {
                debugger;
            });--%>
            Ext.getCmp('MailAlbero').updateLayout();  
            modalMailTreeWin.show();
        };

        function vote(amnt) {           
            var currFolder = document.getElementById('<%= hfCurrFolder.ClientID %>');
            if (typeof currFolder == 'undefined') return false;
            if (currFolder.value.substring(1, 1) == "1" || currFolder.value.substring(1, 1) == "4" || currFolder.value.substring(1, 1) == "5" || parentFolder == "I") {
                var defRating = 0;
                var rating = document.getElementById('<%= rating.ClientID %>');
                if (typeof rating != 'undefined') {
                    defRating = rating.innerHTML;
                };
                if (amnt != defRating) {
                    var uidMail = document.getElementById('<%= hfUIDMail.ClientID %>').value;
                    var account = document.getElementById('<%= hfAccount.ClientID %>').value;
                    var url = '<%= Page.ResolveClientUrl("~/Ashx/GestoreRatingMail.ashx") %>';
                    Ext.Ajax.request({
                        url: url,
                        method: 'GET',
                        params: { idMail: uidMail, account: account, rating: amnt },
                        success: function (response, opts) {
                            var obj = Ext.decode(response.responseText);
                            if (obj == true) {
                                rating.style.width = amnt * 20 + 'px';
                                rating.innerHTML = amnt;
                            }
                            else {
                                Ext.Msg.alert('Errore rating', 'Errore nell\'impostazione del rating');
                            }
                        },
                        failure: function (response, opts) {
                            Ext.Msg.alert('Errore rating', 'Errore server per l\'impostazione del rating');
                        }
                    });
                }
            }
            else {
                return false;
            }
        };


        Ext.onReady(function () {
            var btnShowMailTree = Ext.get('<%= ibShowMailTree.ClientID %>');
            var hfIdMail = Ext.get('<%= hfIdMail.ClientID %>');
            if (typeof btnShowMailTree != 'undefined' && btnShowMailTree != null) {
                btnShowMailTree.addListener('click', function (e) {
                    if (typeof ShowMailTree != 'undefined' && ShowMailTree != null) {
                        ShowMailTree(hfIdMail);  
                    }
                }, this, { stopPropagation: true });
            };
        });
        Ext.onReady(function () {
            if (typeof ShowMailTree == 'undefined' || ShowMailTree == null) {
                if (typeof btnShowMailTree != 'undefined' && btnShowMailTree != null) {
                    btnShowMailTree.setVisibilityMode(Ext.Element.DISPLAY);
                    btnShowMailTree.setVisible(false);
                }
            }
        });


    </script>
</inl:InlineScript>


