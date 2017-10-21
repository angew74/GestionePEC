CREATE TABLE [FAXPEC].[USERLOGINS] (
    [USERID]        VARCHAR (44)  NOT NULL,
    [PROVIDERKEY]   VARCHAR (100) NULL,
    [LOGINPROVIDER] VARCHAR (100) NULL
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.USERLOGINS', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'USERLOGINS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.USERLOGINS.USERID', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'USERLOGINS', @level2type = N'COLUMN', @level2name = N'USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.USERLOGINS.PROVIDERKEY', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'USERLOGINS', @level2type = N'COLUMN', @level2name = N'PROVIDERKEY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.USERLOGINS.LOGINPROVIDER', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'USERLOGINS', @level2type = N'COLUMN', @level2name = N'LOGINPROVIDER';

