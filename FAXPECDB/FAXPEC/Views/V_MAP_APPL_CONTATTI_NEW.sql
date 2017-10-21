 CREATE VIEW FAXPEC.V_MAP_APPL_CONTATTI_NEW(ID_MAP, ID_TITOLO, APP_CODE, TITOLO, TITOLO_PROT_CODE, 
   TITOLO_ACTIVE, ID_SOTTOTITOLO, SOTTOTITOLO, SOTTOTITOLO_PROT_CODE, SOTTOTITOLO_ACTIVE, 
   COM_CODE, ID_CONTACT, MAIL, FAX, TELEFONO, REF_ID_REFERRAL,
    FLG_PEC, REF_ORG, ID_CANALE, CODICE, ID_BACKEND, BACKEND_CODE, BACKEND_DESCR, CATEGORY, DESCR_PLUS) AS
   SELECT   t0.ID_MAP,
       dt0.ID_TITOLO,
         dt0.APP_CODE,
         dt0.TITOLO,
         dt0.TITOLO_PROT_CODE,
         dt0.TITOLO_ACTIVE,
         dt0.ID_SOTTOTITOLO,
         dt0.SOTTOTITOLO,
         dt0.SOTTOTITOLO_PROT_CODE,
         dt0.SOTTOTITOLO_ACTIVE,
         dt0.COM_CODE,
         t1.ID_CONTACT,
         t1.MAIL,
         t1.FAX,
         t1.TELEFONO,
         t1.REF_ID_REFERRAL,
         t1.FLG_PEC,
         t0.ref_id_entita AS REF_ORG,
         t2.ID_CANALE,
         t2.CODICE,
         t3.ID_BACKEND,
         t3.BACKEND_CODE,
         t3.BACKEND_DESCR,
        t3.CATEGORY,
         t3.DESCR_PLUS
       FROM (((rubr_contatti_backend t0
       LEFT OUTER JOIN
         (SELECT c0.id_titolo, 
                 c0.app_code,
                 c0.titolo,
                c0.prot_code AS titolo_prot_code,
                 c0.active AS titolo_active,
                 c1.id_sottotitolo,
                 c1.sottotitolo,
                 c1.prot_code AS sottotitolo_prot_code,
                 c1.active AS sottotitolo_active,
                 c1.com_code 
           FROM comunicazioni_titoli c0
           LEFT OUTER JOIN comunicazioni_sottotitoli c1 
           ON c0.id_titolo = c1.ref_id_titolo
         ) dt0
       ON t0.ref_id_titolo = dt0.id_titolo)
       LEFT OUTER JOIN rubr_contatti t1
      ON t0.ref_id_contatto = t1.id_contact)
       LEFT OUTER JOIN comunicazioni_canali t2 
       ON t0.ref_id_canale = t2.id_canale)
       LEFT OUTER JOIN rubr_backend t3 
       ON t0.ref_id_backend = t3.id_backend