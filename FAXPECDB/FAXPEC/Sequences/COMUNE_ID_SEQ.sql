CREATE SEQUENCE [FAXPEC].[COMUNE_ID_SEQ]
    AS NUMERIC (28)
    START WITH 2001
    INCREMENT BY 1
    MINVALUE 1
    MAXVALUE 99999999
    CACHE 20;


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNE_ID_SEQ', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'SEQUENCE', @level1name = N'COMUNE_ID_SEQ';

