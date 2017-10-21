CREATE SEQUENCE [FAXPEC].[ACTIONS_SEQ]
    AS NUMERIC (28)
    START WITH 121
    INCREMENT BY 1
    MINVALUE 1
    CACHE 20;


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.ACTIONS_SEQ', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'SEQUENCE', @level1name = N'ACTIONS_SEQ';

