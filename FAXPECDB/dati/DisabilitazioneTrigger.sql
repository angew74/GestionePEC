USE FAXPEC;



GO

delete from FAXPEC.RUBR_ENTITA;
deletE from FAXPEC.RUBR_CONTATTI;
DELETE FROM FAXPEC.RUBR_CONTATTI_BACKEND;
DELETE FROM FAXPEC.RUBR_BACKEND;
DELETE FROM FAXPEC.RUBR_ADDRESS;
DELETE FROM FAXPEC.MAIL_SENDERS;
DELETE FROM FAXPEC.COMUNICAZIONI_TITOLI;
DELETE FROM FAXPEC.MAIL_SENDERS_TITOLI;
DELETE FROM FAXPEC.COMUNICAZIONI_SOTTOTITOLI;
DELETE FROM FAXPEC.MAIL_USERS_ADMIN_BACKEND;
DELETE FROM FAXPEC.MAIL_USERS_BACKEND;
DELETE FROM FAXPEC.MAIL_USERS_SENDER_BACKEND;
DELETE FROM FAXPEC.MAILSERVERS;


DISABLE TRIGGER FAXPEC.InsteadOfInsertOn$RUBR_ENTITA ON FAXPEC.RUBR_ENTITA;
DISABLE TRIGGER FAXPEC.InsteadOfInsertOn$RUBR_ADDRESS ON FAXPEC.RUBR_ADDRESS;
DISABLE TRIGGER FAXPEC.InsteadOfInsertOn$RUBR_CONTATTI ON FAXPEC.RUBR_CONTATTI;
DISABLE TRIGGER FAXPEC.InsteadOfInsertOn$RUBR_CONTATTI_BACKEND ON FAXPEC.RUBR_CONTATTI_BACKEND;
DISABLE TRIGGER FAXPEC.InsteadOfInsertOn$RUBR_BACKEND ON FAXPEC.RUBR_BACKEND;
DISABLE TRIGGER FAXPEC.InsteadOfInsertOn$MAIL_SENDERS ON FAXPEC.MAIL_SENDERS;
DISABLE TRIGGER FAXPEC.InsteadOfInsertOn$COMUNICAZIONI_TITOLI ON FAXPEC.COMUNICAZIONI_TITOLI;
DISABLE TRIGGER FAXPEC.InsteadOfInsertOn$COMUNICAZIONI_SOTTOTITOLI ON FAXPEC.COMUNICAZIONI_SOTTOTITOLI;
DISABLE TRIGGER FAXPEC.InsteadOfInsertOn$MAIL_USERS_BACKEND ON FAXPEC.MAIL_USERS_BACKEND;
DISABLE TRIGGER FAXPEC.InsteadOfInsertOn$MAILSERVERS ON FAXPEC.MAILSERVERS;
DISABLE TRIGGER FAXPEC.InsteadOfInsertOn$MAIL_USERS_BACKEND ON FAXPEC.MAIL_USERS_BACKEND;


GO
ENABLE Trigger FAXPEC.InsteadOfInsertOn$RUBR_ENTITA ON FAXPEC.RUBR_ENTITA;
ENABLE TRIGGER FAXPEC.InsteadOfInsertOn$RUBR_ADDRESS ON FAXPEC.RUBR_ADDRESS;
ENABLE TRIGGER FAXPEC.InsteadOfInsertOn$RUBR_CONTATTI ON FAXPEC.RUBR_CONTATTI;
ENABLE TRIGGER FAXPEC.InsteadOfInsertOn$RUBR_CONTATTI_BACKEND ON FAXPEC.RUBR_CONTATTI_BACKEND;
ENABLE TRIGGER FAXPEC.InsteadOfInsertOn$RUBR_BACKEND ON FAXPEC.RUBR_BACKEND;
ENABLE TRIGGER FAXPEC.InsteadOfInsertOn$MAIL_SENDERS ON FAXPEC.MAIL_SENDERS;
ENABLE TRIGGER FAXPEC.InsteadOfInsertOn$COMUNICAZIONI_TITOLI ON FAXPEC.COMUNICAZIONI_TITOLI;
ENABLE TRIGGER FAXPEC.InsteadOfInsertOn$COMUNICAZIONI_SOTTOTITOLI ON FAXPEC.COMUNICAZIONI_SOTTOTITOLI;
ENABLE TRIGGER FAXPEC.InsteadOfInsertOn$MAIL_USERS_BACKEND ON FAXPEC.MAIL_USERS_BACKEND;
ENABLE TRIGGER FAXPEC.InsteadOfInsertOn$MAILSERVERS ON FAXPEC.MAILSERVERS;


GO