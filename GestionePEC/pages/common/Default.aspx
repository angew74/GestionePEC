<%@ Page Title="HomePage" Language="C#" MasterPageFile="~/Master/Mail.Master" Theme="Delta" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GestionePEC.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="WestContentPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">  
 <div class="body-panel" style="padding: 6px; padding-bottom: 0;">
        <div class="content-panel">
            <div class="header-panel-gray">
                <div class="header-title">
                    <div class="header-text-left">
                        <label>
                            Benvenuti nell'Applicazione Gestione PEC</label>
                    </div>
                </div>
            </div>
            <div class="body-panel-gray">
<div id="pannelloGenerale"></div>
    </div>
</div>
    </div>
    <script type="text/javascript">
        Ext.require([
   'Ext.panel.Panel',
   'Ext.layout.container.Anchor'
        ]);
        Ext.onReady(function () {
            Ext.tip.QuickTipManager.init();
            var htmlFirst = "<div><b><i>Informazioni</i></b>" +
                            "<ul style='margin-top: 0px'>" +
                            "<li>Attraverso questa applicazione sarà possibile: </li>" +
                            "<li>1) visualizzare le emails delle caselle di Posta Elettronica collegate al proprio profilo utente. </li>" +
                            " <li>2) Archiviare le emails, archiviarle in cartelle personalizzate. Cestinare le emails per recuperarle in un secondo momento.</li>" +
                           " <li>3) Catalogare le emails con segnalibri e segnalare con la propria utenza la lavorazione della email. </li>" +
                           " <li>4) Effettuare ricerche avanzate delle emails all'interno delle caselle legate alla propria utenza. </li>" +
                            "<li>5) Ottenere una storia delle movimentazioni e delle lavorazioni delle emails. </li>" +
                            "<li>6) Gestire una rubrica dei contatti in proprio possesso. L'applicazione mette a disposizione di base quella dell'IPA (Indice delle Pubbliche Amministrazioni).</li>" +
                           " <li>7) Qualora si sia amministratori creare e gestire i profili degli utenti che accedono all'Applicazione. </li>" +
                           " </ul></div>";

            var htmlSecond = "<div><b><i>Dati riepilogativi del profilo utente:</i></b>" +
                             " <br /> <ul> <li>User:<b><i><label id='lblUser' /></i></b></li><br /> " +
                            " <li> Dipartimento:<b><i><label id='lblDip' /></i></b></li><br /> " +
                             " <li> Nome Cognome:<b><i><label id='lblcognom'  /></i></b></li><br /> " +
                            "  </ul>   <p style='padding-right: 5px; margin-top: 20px; padding-left: 5px; margin-bottom: 5px; padding-bottom: 5px; padding-top: 5px'>" +
                            "  <b><i>Messaggi Utente</i></b><br /> <br />" +
                           " <b>Attenzione !!!<br /> <font style='color: Maroon; font-weight: bolder'> Per gli Utenti:<br /> " +
                           " Le caselle che vengono gestite in questa demo sono visualizzate a titolo di esempio:<br /> " +
                           " di alcuni messaggi non sarà possibile visualizzare il contenuto del messaggio per ragioni di rispetto della privacy e della sicurezza " +
                           " del trattamento dei dati personali. </font></b> <br />  <br /></p> " +
                            " <p> <font style='color: #FF9933;'>Attenzione l'applicazione può non avere <u>Tutte le Funzionalità abilitate e funzionanti</u> poichè si tratta di " +
                            " un prototipo </font> </p> <br />  I servizi abilitati per il tuo profilo sono elencati nel menù a sinistra: selezionare " +
                          "una voce per procedere.  <br /> <br /> <b><i>Supporto tecnico e informazioni</i></b> " +
                            "<ul style='margin-top: 0px'>  <li>1) Consultare la guida in linea accessibile tramite la voce <b><i>Opzioni Utente</i></b>. </li> " +
                            "<li>2) La gestione dei profili utente e la raccolta di segnalazioni tecniche inerenti anomalie " +
                            " o malfunzionamenti del sistema può essere fatta telefonando il numero unico 06/xxxxxx la richiesta " +
                           " verrà inoltrata al primo operatore disponibile. </li> " +
                           " <li>3) Per questioni di carattere tecnico-amministrativo contattare: </li></ul><br /> " +
                            " <p align='center'><b><i>Terminato il lavoro ricordati di effettuare il logoff !</i></b></p></p></div>";
            var panelUno = Ext.create('Ext.panel.Panel', {
                 width: 1000,
                id: 'panelFirst',
                html: htmlFirst,
                resizable: false,
                resizeHandles: 'e se s',
                border:false
                //  renderTo: 'panelBody'
            });
            var panelTwo = Ext.create('Ext.panel.Panel', {
                 width: 1000,
                 id: 'panelTwo',
                 resizable: false,
                 resizeHandles: 'e se s',
                 html: htmlSecond,
                border:false
                // renderTo: 'panelSecondBody'
            });

            Ext.create('Ext.panel.Panel', {
                renderTo: 'pannelloGenerale',
                // frame: true,                
               // title: 'Informazioni Generali',
                resizable: false,
                resizeHandles: 'e se s',
                // width: 1000,
                // minWidth: 1220,
                // height: 590,
                border: false
               // layout: 'anchor',
                ,items: [
                  panelUno, panelTwo]
            });
        });
    </script>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="RightContentPlaceHolder" runat="server">
</asp:Content>
