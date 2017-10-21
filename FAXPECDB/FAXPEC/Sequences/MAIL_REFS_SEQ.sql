﻿CREATE SEQUENCE [FAXPEC].[MAIL_REFS_SEQ]
    AS NUMERIC (28)
    START WITH 4507
    INCREMENT BY 1
    MINVALUE 1
    MAXVALUE 99999999999999999999
    NO CACHE;


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_REFS_SEQ', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'SEQUENCE', @level1name = N'MAIL_REFS_SEQ';

