/********************************************************************/	   
function isNumberKey(evt)
{
 var charCode = (evt.which) ? evt.which : event.keyCode
 if (charCode > 31 && (charCode < 48 || charCode > 57))
	return false;

 return true;
}
/********************************************************************/	 
function isAlphabeticForNamesKey(evt) {
    try {
        evt = (evt) ? evt : window.event;
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (!((charCode <= 90 && charCode >= 65) || (charCode <= 122 && charCode >= 97) || charCode == 39)) {
            evt.returnValue = false;
        }
    }
    catch (e) {
        evt.returnValue = false;
    }

    return true;
}
/*********************************************************************************/

function isAlphabeticKey(evt) {
    try {
        evt = (evt) ? evt : window.event;
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (!((charCode <= 90 && charCode >= 65) || (charCode <= 122 && charCode >= 97) || charCode == 45 || charCode == 46 || charCode == 95)) {
            evt.returnValue = false;
        }
    }
    catch (e) {
        evt.returnValue = false;
    }

    return true;
}


/********************************************************************/    
function SetProgressPosition(e, container) {
    try
    {
        if(e!=null && document.getElementById(container)!=null)
        {
            var posx = 0;
            var posy = 0;
            if (!e) var e = window.event;
            if (e.pageX || e.pageY) {
                posx = e.pageX;
                posy = e.pageY;
            }
            else if (e.clientX || e.clientY) {
                posx = e.clientX + document.documentElement.scrollLeft;
                posy = e.clientY + document.documentElement.scrollTop;
            }
            
            document.getElementById(container).style.left = posx - 8 + "px";
            document.getElementById(container).style.top = posy - 8 + "px";
        }
    }
    catch(e)
    {
        return false;
    }
}
/********************************************************************/
function CalculatePopupPosition() {
    Sys.Extended.UI.CommonToolkitScripts.getLocation = function(element) {
        if (element === document.documentElement) {
            return new Sys.UI.Point(0, 0);
        }
        if (Sys.Browser.agent == Sys.Browser.InternetExplorer && Sys.Browser.version < 7) {
            if (element.window === element || element.nodeType === 9 || !element.getClientRects || !element.getBoundingClientRect) return new Sys.UI.Point(0, 0); var screenRects = element.getClientRects(); if (!screenRects || !screenRects.length) {
                return new Sys.UI.Point(0, 0);
            }
            var first = screenRects[0]; var dLeft = 0; var dTop = 0; var inFrame = false; try {
                inFrame = element.ownerDocument.parentWindow.frameElement;
            } catch (ex) {
                inFrame = true;
            }
            if (inFrame) {
                var clientRect = element.getBoundingClientRect(); if (!clientRect) {
                    return new Sys.UI.Point(0, 0);
                }
                var minLeft = first.left; var minTop = first.top; for (var i = 1; i < screenRects.length; i++) {
                    var r = screenRects[i]; if (r.left < minLeft) {
                        minLeft = r.left;
                    }
                    if (r.top < minTop) {
                        minTop = r.top;
                    }
                }
                dLeft = minLeft - clientRect.left; dTop = minTop - clientRect.top;
            }
            var ownerDocument = element.document.documentElement; return new Sys.UI.Point(first.left - 2 - dLeft + ownerDocument.scrollLeft, first.top - 2 - dTop + ownerDocument.scrollTop);
        }

        return findPos(element);
    }
}


/********************************************************************/
function findPos(obj) {

    var curleft = curtop = 0;

    var scrollLeft = obj.scrollLeft;
    var scrollTop = obj.scrollTop;
    if (obj.offsetParent) {
        do {
            curleft += obj.offsetLeft;
            curtop += obj.offsetTop;
        } while (obj = obj.offsetParent);
    }

    return new Sys.UI.Point(curleft + scrollLeft, curtop - scrollTop)
}

/********************************************************************/
function SetWaitPosition(e, panel) {
    try {
        if (panel != null) {
            panel.style.display = 'block';

            var posx = 0;
            var posy = 0;
            var e = window.event;
            if (e.pageX || e.pageY) {
                posx = e.pageX;
                posy = e.pageY;
            }
            else if (e.clientX || e.clientY) {
                posx = e.clientX + document.documentElement.scrollLeft;
                posy = e.clientY + document.documentElement.scrollTop;
            }

            panel.style.left = posx - 8 + "px";
            panel.style.top = posy - 108 + "px";
        }
    }
    catch (e) {
        return false;
    }
} 
/********************************************************************/
function SetBackUrl() {
    if(document.referrer != "")
        location.reload(document.referrer);
    else history.go(-1);
}
/********************************************************************/


String.prototype.trim = function() { return this.replace(/^\s+|\s+$/g, ''); };

/********************************************************************/
function showHideLayer(idLayer) {

    var objLayer = document.getElementById(idLayer);
    //(objLayer.style.visibility == "hidden") ? objLayer.style.visibility = "visible" : objLayer.style.visibility = "hidden";
    if (objLayer.style.visibility == "hidden") {

        objLayer.style.position = "relative";
        objLayer.style.visibility = "visible";
    }
    else {

        objLayer.style.position = "absolute";
        objLayer.style.visibility = "hidden";
    }
}
/*********************************************************************************/
function setAnchorText(id) {

    var objAnchor = document.getElementById(id);
    (objAnchor.innerHTML == "Visualizza Storico") ? objAnchor.innerHTML = "Nascondi Storico" : objAnchor.innerHTML = "Visualizza Storico";
}

/*********************************************************************************/
function InitViewPort(isLogin, showmenu, showRightPanel) {
    var titleCenter = '';
    if (document.getElementById('<%=hfCenterTitle.ClientID %>') != null)
        titleCenter = document.getElementById('<%=hfCenterTitle.ClientID %>').value;

    if (titleCenter == '')
        titleCenter = 'Cambi di Residenza e Domicilio (CRI, CRE, CFAM)';

    CreateViewPort(titleCenter, '', isLogin, showmenu, showRightPanel);
}
/*********************************************************************************/
function ClosePanelMessaggi() {
    var pan = document.getElementById("ContainerMsg")
    pan.style.display = "none";
}
/*********************************************************************************/
function ClosePanelMessaggi(Id) {
    var pan = document.getElementById(Id)
    if (pan != null)
        pan.style.display = "none";

    return false;
}
/*********************************************************************************/
function MinimizePanelMessaggi(PanelId, ButtonId) {

    var h = pageHeight();
    var pan = document.getElementById(PanelId)
    var but = document.getElementById(ButtonId)

    if ((pan != null) && (but != null)) {
        if (but.className == "bMinimize") {
            but.className = "bMaximize";
            but.title = "Massimizza";
            pan.style.top = (h - 26) + "px";
        }
        else {
            but.className = "bMinimize"
            but.title = "Minimizza";
            pan.style.top = (h - 145) + "px";
        }
    }

    return false;
}
/*********************************************************************************/

function MinimizeWSPanelMessaggi(PanelId, ButtonId) {
    //Funzione che minimizza il pannello dei messaggi senza operare in modalità di switch
    //come invece fa la funzione MinimizePanelMessaggi (WS sta per Without Switching)
    var h = pageHeight();
    var pan = document.getElementById(PanelId)
    var but = document.getElementById(ButtonId)

    if ((pan != null) && (but != null)) {

        but.className = "bMaximize";
        but.title = "Massimizza"
        pan.style.top = (h - 26) + "px";

    }

    return false;
}

function SetProgressPosition(e, container) {
    try {
        if (e != null && document.getElementById(container) != null) {
            var posx = 0;
            var posy = 0;
            if (!e) var e = window.event;
            if (e.pageX || e.pageY) {
                posx = e.pageX;
                posy = e.pageY;
            }
            else if (e.clientX || e.clientY) {
                posx = e.clientX + document.documentElement.scrollLeft;
                posy = e.clientY + document.documentElement.scrollTop;
            }

            document.getElementById(container).style.left = posx - 8 + "px";
            document.getElementById(container).style.top = posy - 8 + "px";
        }
    }
    catch (e) {
        return false;
    }

}
/*********************************************************************************/
function fnUnloadHandler() {
    if (isBrowserClosed()) {
        var url = logOffFromBrowser + '?usr=' + utente + '&dip=' + dipartimento;
        window.open(url, 'logoff', 'width=450,height=450,menubar=no,Left=0,Top=0,resizable=no,ScreenX=1,ScreenY=1,scrollbars=no,hotkeys=no');
    }
}
/*********************************************************************************/
function isBrowserClosed() {

    var browserWindowWidth = 0;
    var browserWindowHeight = 0;
    if (parseInt(navigator.appVersion) > 3) {
        if (navigator.appName == "Netscape") {
            browserWindowWidth = window.innerWidth;
            browserWindowHeight = window.innerHeight;
        }

        if (navigator.appName.indexOf("Microsoft") != -1) {
            browserWindowWidth = top.window.document.body.offsetWidth;
            browserWindowHeight = top.window.document.body.offsetHeight;
        }
    }

    try {
        return (event.clientY < 0 && event.clientX > (browserWindowWidth - 25)) ? true : false;
    }
    catch (e) {
        return false;
    }

}
/*********************************************************************************/
function backButtonOverride() {
    // Work around a Safari bug
    // that sometimes produces a blank page
    setTimeout("backButtonOverrideBody()", 1);
}
/*********************************************************************************/
function backButtonOverrideBody() {
    // Works if we backed up to get here
    try {
        history.forward();
    }
    /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/catch (e) {
        // OK to ignore
    }
    // Every quarter-second, try again. The only
    // guaranteed method for Opera, Firefox,
    // and Safari, which don't always call
    // onLoad but *do* resume any timers when
    // returning to a page
    setTimeout("backButtonOverrideBody()", 500);
}
/*********************************************************************************/
function SetWaitPosition(e, panel) {
    try {
        if (panel != null) {
            panel.style.display = 'block';

            var posx = 0;
            var posy = 0;
            var e = window.event;
            if (e.pageX || e.pageY) {
                posx = e.pageX;
                posy = e.pageY;
            }
            else if (e.clientX || e.clientY) {
                posx = e.clientX + document.documentElement.scrollLeft;
                posy = e.clientY + document.documentElement.scrollTop;
            }

            panel.style.left = posx - 8 + "px";
            panel.style.top = posy - 108 + "px";
        }
    }
    catch (e) {
        return false;
    }
}
/*********************************************************************************/
function SetFocusOnItemDate(currentItemId, nextItemId, evt) {

    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;

    if (charCode != 9 && charCode != 16) {
        var currentItem = document.getElementById(currentItemId);
        var nextItem = document.getElementById(nextItemId);

        if (currentItem.value.length == 2) nextItem.focus();
    }
}
/*********************************************************************************/
//Controlla che la data sia corretta, includendo anche il controllo sugli anni bisestili
function isValidDate(dateValue) {
    var arr = new Array();

    //arr = dateField.value.toString().split('/');	
    arr = dateValue.toString().split('/');

    var day = arr[0];
    var month = arr[1];
    var year = parseInt(arr[2], 10);

    if (day > 31 || month > 12 || (day == 29 && month == 2 && !isAnnoBisestile(year)) || year < 1800) return false;

    // Using form values, create a new date object
    var myDate = new Date(arr[2], arr[1] - 1, day);

    return (myDate.getDate() == day);
}
/*********************************************************************************/
/*
Questa funzione valuta se un anno e' bisestile o meno, con il seguente algoritmo:
un anno e' bisestile se e' divisibile per 4 ma non per 100. Tuttavia e' bisestile un anno 
divisibile per 400, come l'anno 2000.	
*/

function isAnnoBisestile(anno) {


    if (anno % 400 == 0) return true;
    else {
        return ((anno % 4 == 0) && (anno % 100 != 0));
    }
}

/*********************************************************************************/
function HideControl(idControl) {

    var ctrl = document.getElementById(idControl);

    if (ctrl != null) {
        ctrl.style.visibility = "hidden";
    }

}
/*********************************************************************************/
function DisableControl(idControl) {

    var ctrl = document.getElementById(idControl);

    if (ctrl != null) {
        ctrl.disabled = true;
    }

}
/*********************************************************************************/
function ValidatePage() {

    if (typeof (Page_ClientValidate) == 'function') {
        Page_ClientValidate();
    }

    if (Page_IsValid) {
        return true;
    }
    else {
        return false;
    }
}

/*********************************************************************************/

function SetMsgAndButton(idDivMsg, butCercaId, butStampaId) {

    if (ValidatePage()) { //se la pagina ha superato i controlli di validazione

        showHideLayer(idDivMsg);
        HideControl(butCercaId);
        DisableControl(butStampaId);
    }
    return true;
}
/*********************************************************************************/

/*******************UC_VANA_TOGGLE_BUTTON*****************************************/
function SHT_ShowHideToggle(obj) {  
    if (obj.className == "bMinimize") {
        obj.parentElement.parentElement.parentElement.nextSibling.style.display = "none";
        obj.className = "bMaximize";
    }
    else {
        obj.parentElement.parentElement.parentElement.nextSibling.style.display = "block";
        obj.className = "bMinimize";
    }

}
/*********************************************************************************/
function pageHeight() { return window.innerHeight != null ? window.innerHeight : document.documentElement && document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body != null ? document.body.clientHeight : null; }
