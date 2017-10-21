﻿CREATE SEQUENCE [FAXPEC].[MAIL_ALLEGATI_SEQ]
    AS NUMERIC (28)
    START WITH 181
    INCREMENT BY 1
    MINVALUE 1
    MAXVALUE 9999999999
    CACHE 20;


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_ALLEGATI_SEQ', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'SEQUENCE', @level1name = N'MAIL_ALLEGATI_SEQ';

