﻿CREATE SEQUENCE [FAXPEC].[COM_PROT_FLUSSO_ID_SEQ]
    AS NUMERIC (28)
    START WITH 2
    INCREMENT BY 1
    MINVALUE 1
    MAXVALUE 9999999999
    NO CACHE;


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COM_PROT_FLUSSO_ID_SEQ', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'SEQUENCE', @level1name = N'COM_PROT_FLUSSO_ID_SEQ';

