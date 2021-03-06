﻿CREATE VIEW FAXPEC.V_CONTATTI_RUBR_AND_IPA (
   SRC, 
   MAIL_ID, 
   MAIL, 
   TEL, 
   FAX, 
   ID_REFERRAL, 
   ID_PADRE, 
   REFERRAL_TYPE, 
   COGNOME, 
   NOME, 
   COD_FIS, 
   P_IVA, 
   RAGIONE_SOCIALE, 
   UFFICIO, 
   NOTE, 
   REF_ID_ADDRESS, 
   FLG_IPA, 
   IPA_DN, 
   IPA_ID, 
   DISAMB_PRE, 
   DISAMB_POST, 
   REF_ORG, 
   SITO_WEB, 
   AFF_IPA, 
   INDIRIZZO, 
   CIVICO, 
   CAP, 
   COMUNE, 
   SIGLA_PROV, 
   COD_ISO_STATO, 
   INDIRIZZO_NON_STANDARD)
AS 
   /*Generated by SQL Server Migration Assistant for Oracle version 7.3.0.*/
   SELECT 
      'R' AS SRC, 
      con.ID_CONTACT AS MAIL_ID, 
      con.MAIL AS MAIL, 
      con.TELEFONO AS TEL, 
      con.FAX AS FAX, 
      ent.ID_REFERRAL, 
      ent.ID_PADRE, 
      ent.REFERRAL_TYPE, 
      ent.COGNOME, 
      ent.NOME, 
      ent.COD_FIS, 
      ent.P_IVA, 
      ent.RAGIONE_SOCIALE, 
      ent.UFFICIO, 
      ent.NOTE, 
      ent.REF_ID_ADDRESS, 
      ent.FLG_IPA, 
      ent.IPA_DN, 
      ent.IPA_ID, 
      ent.DISAMB_PRE, 
      ent.DISAMB_POST, 
      ent.REF_ORG, 
      ent.SITO_WEB, 
      ent.AFF_IPA, 
      ind.INDIRIZZO, 
      ind.CIVICO, 
      ind.CAP, 
      ind.COMUNE, 
      ind.SIGLA_PROV, 
      ind.COD_ISO_STATO, 
      ind.INDIRIZZO_NON_STANDARD
   FROM 
      FAXPEC.RUBR_ENTITA  AS ent 
         INNER JOIN FAXPEC.RUBR_CONTATTI  AS con 
         ON ent.ID_REFERRAL = con.REF_ID_REFERRAL 
         LEFT OUTER JOIN FAXPEC.RUBR_ADDRESS  AS ind 
         ON ent.REF_ID_ADDRESS = ind.ID_ADDRESS 
   WHERE con.MAIL IS NOT NULL
    UNION
   SELECT TOP 9223372036854775807 WITH TIES 
      'I' AS SRC, 
      IPA.ID_RUB AS MAIL_ID, 
      IPA.MAIL AS MAIL, 
      IPA.TELEFONO AS TEL, 
      IPA.FAX AS FAX, 
      ent.ID_REFERRAL, 
      ent.ID_PADRE, 
      ent.REFERRAL_TYPE, 
      ent.COGNOME, 
      ent.NOME, 
      ent.COD_FIS, 
      ent.P_IVA, 
      ent.RAGIONE_SOCIALE, 
      ent.UFFICIO, 
      ent.NOTE, 
      ent.REF_ID_ADDRESS, 
      ent.FLG_IPA, 
      ent.IPA_DN, 
      ent.IPA_ID, 
      ent.DISAMB_PRE, 
      ent.DISAMB_POST, 
      ent.REF_ORG, 
      ent.SITO_WEB, 
      ent.AFF_IPA, 
      ind.INDIRIZZO, 
      ind.CIVICO, 
      ind.CAP, 
      ind.COMUNE, 
      ind.SIGLA_PROV, 
      ind.COD_ISO_STATO, 
      ind.INDIRIZZO_NON_STANDARD
   FROM 
      FAXPEC.RUBR_ENTITA  AS ent 
         INNER JOIN FAXPEC.IPA 
         ON ssma_oracle.instr2_varchar(IPA.DN, ent.IPA_DN) = 1 
         LEFT OUTER JOIN FAXPEC.RUBR_ADDRESS  AS ind 
         ON ent.REF_ID_ADDRESS = ind.ID_ADDRESS 
   WHERE IPA.MAIL IS NOT NULL AND ent.IPA_DN IS NOT NULL
   ORDER BY 6 ASC, 1 DESC