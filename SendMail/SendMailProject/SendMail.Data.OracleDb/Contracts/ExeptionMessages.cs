using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMailApp.OracleCore.Contracts
{
    public class DalExMessages
    {
        public static string ID_NON_RESTITUITO = "Valore Chiave Primaria Non Restituito.";
        public static string NESSUNA_RIGA_MODIFICATA = "Nessuna riga modificata dal Comando di Aggiornamento.";
        public static string RUBRICA_NON_AGGIORNATA = "L'aggiornamento della Rubrica in banca dati non è riuscito.";
        public static string RUBRICA_NON_INSERITA = "L'inserimento della Rubrica in banca dati non è riuscito.";
        public static string RUBRICA_NON_RECUPERATA = "Il recupero dei Nominativi in Rubrica dalla banca dati non è riuscito.";
        public static string ERRORE_CANCELLAZIONE = "Errore nell'eliminazione del contatto.";
        public static string TITOLO_NON_RECUPERATO = "Il recupero dei Titoli dalla banca dati non è riuscito.";
        public static string TITOLO_NON_AGGIORNATO = "L'aggiornamento dei Titoli dalla banca dati non è riuscito.";
        public static string SOTTOTITOLO_NON_RECUPERATO = "Il recupero dei SottoTitoli dalla banca dati non è riuscito.";
    }
}
