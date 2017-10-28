using System;
using System.Collections.Generic;
using System.Text;

namespace FaxPec.Model.WorkFlows
{
    /// <summary>
    /// Interfaccia per la gestione del workflow relativo all'istruzione di una nuova richiesta.
    /// </summary>
    public interface IIstruzioneRichiestaWorkflow
    {
        /// <summary>
        /// Traccia il task corrente del workflow.
        /// </summary>
        int CurrentTask { get; set; }    
        
        /// <summary>
        /// Verifica la validità formale della richiesta.
        /// </summary>
        void Task1_CheckRichiesta();

        /// <summary>
        /// Verifica lo 'status' del nominativo rispetto alla rubrica.
        /// </summary>
        void Task2_CheckNominativoInRubrica();

        /// <summary>
        /// Crea la nuova richiesta.
        /// </summary>
        void Task3_CreateRichiesta();
    }



}
