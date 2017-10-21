CREATE SEQUENCE [FAXPEC].[RUBR_TAGS_SEQ]
    AS NUMERIC (28)
    INCREMENT BY 1
    MINVALUE 1
    MAXVALUE 9999999999
    NO CACHE;


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_TAGS_SEQ', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'SEQUENCE', @level1name = N'RUBR_TAGS_SEQ';

