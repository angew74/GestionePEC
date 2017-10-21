CREATE TABLE [FAXPEC].[LOG_LOG_CODES] (
    [LOG_CODE] VARCHAR (20)  NOT NULL,
    [DESCR]    VARCHAR (200) NOT NULL,
    CONSTRAINT [LOG_LOG_CODES_PK] PRIMARY KEY CLUSTERED ([LOG_CODE] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_LOG_CODES', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_LOG_CODES';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_LOG_CODES.LOG_CODE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_LOG_CODES', @level2type = N'COLUMN', @level2name = N'LOG_CODE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_LOG_CODES.DESCR', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_LOG_CODES', @level2type = N'COLUMN', @level2name = N'DESCR';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_LOG_CODES.LOG_LOG_CODES_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_LOG_CODES', @level2type = N'CONSTRAINT', @level2name = N'LOG_LOG_CODES_PK';

