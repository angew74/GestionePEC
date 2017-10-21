CREATE SEQUENCE [FAXPEC].[MAIL_REFS_NEW_SEQ]
    AS NUMERIC (28)
    START WITH 1554
    INCREMENT BY 1
    MINVALUE 1
    CACHE 20;


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_REFS_NEW_SEQ', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'SEQUENCE', @level1name = N'MAIL_REFS_NEW_SEQ';

