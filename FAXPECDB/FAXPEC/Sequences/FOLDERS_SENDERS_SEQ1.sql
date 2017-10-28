﻿CREATE SEQUENCE [FAXPEC].[FOLDERS_SENDERS_SEQ1]
    AS NUMERIC (28)
    START WITH 461
    INCREMENT BY 1
    MINVALUE 1
    CACHE 20;


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.FOLDERS_SENDERS_SEQ1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'SEQUENCE', @level1name = N'FOLDERS_SENDERS_SEQ1';
