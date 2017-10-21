CREATE TABLE [FAXPEC].[LOG_APP_CODES] (
    [APP_CODE] VARCHAR (20) NOT NULL,
    [DESCR]    VARCHAR (50) NOT NULL,
    CONSTRAINT [APP_CODES_PK] PRIMARY KEY CLUSTERED ([APP_CODE] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_APP_CODES.APP_CODES_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_APP_CODES', @level2type = N'CONSTRAINT', @level2name = N'APP_CODES_PK';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_APP_CODES', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_APP_CODES';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_APP_CODES.APP_CODE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_APP_CODES', @level2type = N'COLUMN', @level2name = N'APP_CODE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_APP_CODES.DESCR', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_APP_CODES', @level2type = N'COLUMN', @level2name = N'DESCR';

