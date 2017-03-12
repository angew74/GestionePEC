/* function per testare il codice fiscale */
function CheckCodFis(oSnd, args){
    var cFis = args.Value.toUpperCase();
    switch(cFis.length){
        case 11:
            if(CheckCodFisProvvisorio(cFis)){
                args.IsValid = true;
            }
            break;
        case 16:
            if(CheckCodFisDef(cFis)){
                args.IsValid = true;
            }
            break;
        default:
            args.IsValid = false;
            break;
    }
    return true;
};
function CheckCodFisProvvisorio(cFis){
    for(var  i = 0; i < cFis.length; i++){
        if(!isNumber(cFis.charAt(i))){
            return false;
        }
    }
    if(cFis.substring(0, 7) == '0000000'){
        return false;
    }
    var  pintCodUff = Number(cFis.slice(7, 10));
    if (pintCodUff == 0){
        var appo = Number(cFis.slice(0, 7));
        if((appo > 0) && (appo < 273961)){
            return true;
        }
        if(((appo > 0400000) && (appo < 1072480))
        || ((appo > 1500000) && (appo < 1828637))
        || ((appo > 2000000) && (appo < 2054096))) {
            if(checkDigit(cFis)){
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    } else if (((pintCodUff > 0) && (pintCodUff < 101))
        || ((pintCodUff > 119) && (pintCodUff < 122))){
        return cfNum(cFis);
    } else if ((pintCodUff > 150) && (pintCodUff < 246)){
        if (checkDigit(cFis))
        {
            return true;
        } else {
            return false;
        }
    } else if ((pintCodUff > 300) && (pintCodUff < 767)) {
        if (checkDigit(cFis)) {
            return true;
        } else {
            return false;
        }
    } else if ((pintCodUff > 899) && (pintCodUff < 951)) {
            if (checkDigit(cFis)) {
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
};
function checkDigit(cFis){
    var intAppoggio;
    var pintAppo;
    var pintUltimoCarattere;
    var pintTotale = 0;
    for (var i = 1; i < 11; i += 2) {
        var strElem = cFis.slice(i, i + 1);
        intAppoggio = Number(strElem);
        pintAppo = intAppoggio * 2;
        var strS2 = pintAppo.toString();
        for (var j = 0; j < strS2.length; j++) {
            var strElem1 = strS2.slice(j, j + 1);
            pintTotale += Number(strElem1);
        }
    }
    //somma dei valori attribuiti alle cifre dispari
    for (var k = 0; k < 9; k += 2) {
        var strElem2 = cFis.slice(k, k + 1);
        pintTotale += Number(strElem2);
    }
    // estraggo l'ultima cifra del codice fiscale 
    var strElem = cFis.slice(10, 11);
    intAppoggio = Number(strElem);
    pintUltimoCarattere = intAppoggio;
    // controllo del complemento a dieci della cifra a destra del totale 
    if (pintTotale % 10 == 0) {
        return (pintTotale % 10) == pintUltimoCarattere;
    } else {
        return (10 - (pintTotale % 10)) == pintUltimoCarattere;
    }
};
function cfNum(cFis){
    var  pintCodUff = Number(cFis.slice(0, 7));
    if (checkDigit(cFis) == false) {
        return false;
    } else {
        if(Number(cFis.slice(0, 7)) < 8000000){
            return true;
        } else if (pintCodUff > 95) {
            return false;
        } else {
            return true;
        }
    }
};
//controlli codice fiscale persona fisica
function CheckCodFisDef(cFis){
   	if(controllaCorrettezzaChar(cFis) == "0"){
        return true;
    } else {
        return false;
	}
};
function controllaCorrettezzaChar(codicefiscale){ 
   	for (i=0; i<6; i++){
		if(isNumber(codicefiscale.charAt(i))){
			return "2"; // errore
		}
	}
	for (i=6; i<8; i++){
		if(!(isNumber(codicefiscale.charAt(i))) && 
		((codicefiscale.charAt(i) != "L") && 
		(codicefiscale.charAt(i) != "M") &&
		(codicefiscale.charAt(i) != "N") &&
		(codicefiscale.charAt(i) != "P") &&
		(codicefiscale.charAt(i) != "Q") &&
		(codicefiscale.charAt(i) != "R") &&
		(codicefiscale.charAt(i) != "S") &&
		(codicefiscale.charAt(i) != "T") &&
		(codicefiscale.charAt(i) != "U") &&
		(codicefiscale.charAt(i) != "V")))
		    return "2";  // errore
	}
	if (!((codicefiscale.charAt(8) != "A") ||
		(codicefiscale.charAt(8) != "B") ||	
		(codicefiscale.charAt(8) != "C") ||
		(codicefiscale.charAt(8) != "D") ||
		(codicefiscale.charAt(8) != "E") ||
		(codicefiscale.charAt(8) != "H") ||
		(codicefiscale.charAt(8) != "L") ||
		(codicefiscale.charAt(8) != "M") ||
		(codicefiscale.charAt(8) != "P") ||
		(codicefiscale.charAt(8) != "R") ||
		(codicefiscale.charAt(8) != "S") ||
		(codicefiscale.charAt(8) != "T"))){
		    return "2"; 
    } // errore
	for (i = 9; i < 11; i++){
		if (!(isNumber(codicefiscale.charAt(i))) &&
		((codicefiscale.charAt(i) != "L") &&
		 (codicefiscale.charAt(i) != "M") &&
		 (codicefiscale.charAt(i) != "N") &&
		 (codicefiscale.charAt(i) != "P") &&
		 (codicefiscale.charAt(i) != "Q") &&
		 (codicefiscale.charAt(i) != "R") &&
		 (codicefiscale.charAt(i) != "S") &&
		 (codicefiscale.charAt(i) != "T") &&
		 (codicefiscale.charAt(i) != "U") &&
		 (codicefiscale.charAt(i) != "V"))){
		    return "2";
		}
	}
	// controllo formale del giorno
	var intgiorno = trasforma_giorno(9, codicefiscale);
	if(intgiorno > 31){
	    intgiorno -= 40;
	}
	if (intgiorno < 1 ||intgiorno > 31){
		return "2";
    }
	var strElem = codicefiscale.substring(8, 9);
	var strMese = trasforma_mese(strElem);
	var strAnno = trasforma_giorno(6, codicefiscale);  	
	if(strAnno.length == 1){
		strAnno = "0" + strAnno;
	}
	var strGiorno = intgiorno;
	if(strGiorno.length == 1){
	    strGiorno = "0" + strGiorno;
	}
	var data = strGiorno + strMese + strAnno;
	if(!(controllaData(data))){
	    return "2"; 
	}
	if((codicefiscale.charAt(11) != "A") &&
	   (codicefiscale.charAt(11) != "B") &&
	   (codicefiscale.charAt(11) != "C") &&
	   (codicefiscale.charAt(11) != "D") &&
	   (codicefiscale.charAt(11) != "E") &&
	   (codicefiscale.charAt(11) != "F") &&
	   (codicefiscale.charAt(11) != "G") &&
	   (codicefiscale.charAt(11) != "H") &&
	   (codicefiscale.charAt(11) != "I") &&
	   (codicefiscale.charAt(11) != "L") &&
	   (codicefiscale.charAt(11) != "M") &&
	   (codicefiscale.charAt(11) != "Z")){
	   return "2"; 
	}
// errore 
	for(i=12; i<15; i++){
	    var lettera = false;
		if(!(isNumber(codicefiscale.charAt(i)))){
			lettera = true;
			if((codicefiscale.charAt(i) != "L") &&
			   (codicefiscale.charAt(i) != "M") &&
			   (codicefiscale.charAt(i) != "N") &&
			   (codicefiscale.charAt(i) != "P") &&
			   (codicefiscale.charAt(i) != "Q") &&
			   (codicefiscale.charAt(i) != "R") &&
			   (codicefiscale.charAt(i) != "S") &&
			   (codicefiscale.charAt(i) != "T") &&			 	
			   (codicefiscale.charAt(i) != "U") &&
			   (codicefiscale.charAt(i) != "V")){
			    return "3"; 
			}
		}
	}
	if (!(lettera)){
		var intNumeroCodCat = codicefiscale.substring(12, 15);
		if (intNumeroCodCat == "000")
		{ return "numero codice catastale 2";}
		
		if ((codicefiscale.charAt(11) == "M") && 
		    (Number(intNumeroCodCat) > 399))
		{ return "numero codice catastale 2";}

	}
	if (controllaCheckDigit(codicefiscale)){
		return "0";
    } else {
        return "1";
    }
};
// controllo se numerico
function isNumber (o) { return o != null && !isNaN(o) && isFinite(Number(o)); };
// decodifica giorno
function trasforma_giorno(giorno, codicefiscale){
	var appo = "";
	for (i = giorno; i < giorno + 2; i++){
		if(isNumber(codicefiscale.charAt(i))){
			appo += codicefiscale.charAt(i);
		} else {
		    switch(codicefiscale.charAt(i)) {
		        case "L":
			        appo += "0";
				    break;
			    case "M":
				    appo += "1";
				    break;
			    case "N":
				    appo += "2";
				    break;
			    case "P":
				    appo += "3";
				    break;
			    case "Q":
				    appo += "4";
				    break;
			    case "R":
				    appo += "5";
				    break;
			    case "S":
				    appo += "6";
				    break;
			    case "T":
				    appo += "7";
				    break;
			    case "U":
				    appo += "8";
				    break;
			    case "V":
				    appo += "9";
				    break;
		    }
	    }
	}
	return appo;
};
// decodifica mese
function trasforma_mese(lettera){
	var mese = "";
	switch(lettera){
		case " ":
			break;
		case "A":
			mese="01";
			break;
		case "B":
			mese="02";
			break;
		case "C":
			mese="03";
			break;
		case "D":
			mese="04";
			break;
		case "E":
			mese="05";
			break;
		case "H":
			mese="06";
			break;
		case "L":
			mese="07";
			break;
		case "M":
			mese="08";
			break;
		case "P":
			mese="09";
			break;
		case "R":
			mese="10";
			break;
		case "S":
			mese="11";
			break;
		case "T":
			mese="12";
			break;		
	}
	return mese;
};
// controllo anno nascita
function controllaData(data){
	var strAnno = data.substring(4, data.length);
	if ((data.length == 8) || (data.length == 6)){
		if(data.length == 6){
		    strAnno = "19" + strAnno;
		}
		if(Number(strAnno) < 1870){
		    return "anno < 1970"; 
		} else { 
		    return true;
		}
	}
	else { return "lunghezza data"};
	
	var strMese = data.substring(2, 4);
    var strGiorno = data.substring(0, 2);	
	
	if((strMese > 12) || (strGiorno > 31) || (strMese < 1) || (strGiorno < 1))
	    return false;
	    
	switch(strMese){
	    case "02":
	    var bisestile = false;
	        if ((strAnno % 4) == 0){
	            if ((strAnno % 400) == 0){
	                if ((strAnno % 1000) == 0){
	                    bisestile = true; 
	                }
	            } else {
	                bisestile = true; 
	            }
	        }
	        if ((bisestile && (strGiorno > 29)) || (!bisestile && (strGiorno > 28))){
	            return false;
	        }
	        break;
	    case "04":
	    case "06":
	    case "09":
	    case "11":
	        if (strGiorno > 30) {
	            return false;
	        }
	        break;
	    }
	    return true;      
};
function controllaCheckDigit (codicefiscale)
{
    var matrCar = matriceCaratteri();
	var intAppoggio = 0;
	var charCarattereEsaminato;
	for (i=0; i<15; i++){
		charCarattereEsaminato = codicefiscale.charAt(i);
		var strElem = codicefiscale.substring(i, i+1);
		var intResto = 	(i % 2);
		switch(intResto)
		{
			case 0:
				if(!(isNumber(charCarattereEsaminato))){
				    intAppoggio += matrCar.caratteriDispari.indexOf(strElem);
				} else {
				    intAppoggio += matrCar.numeriDispari.indexOf(strElem);
				}
			    break;
			case 1:
				if(!isNumber(charCarattereEsaminato)){   
				    intAppoggio += matrCar.caratteriPari.indexOf(strElem);
				} else {   
				    intAppoggio += matrCar.numeriPari.indexOf(strElem);
				}
			    break;
			default:
			    break;
		}
	}	
	var checkDigit = codicefiscale.substring(15, 16);
	return ((intAppoggio % 26) == matrCar.caratteriPari.indexOf(checkDigit));
};
function matriceCaratteri(){
    var matrCar = {};
    matrCar.caratteriDispari = ["B", "A", "K", "P", "L", "C", "Q", "D", "R", "E", "V", "O", "S",
        "F", "T", "G", "U", "H", "M", "I", "N", "J", "W", "Z", "Y", "X"];
    matrCar.numeriDispari = ["1", "0",,,, "2",, "3",, "4",,,, "5",, "6",, "7",, "8",, "9"];
    matrCar.caratteriPari= ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
		"N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"];
	matrCar.numeriPari = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];
	return matrCar;
};