CREATE TABLE [FAXPEC].[RUBR_REFERRAL_TYPE] (
    [REFERRAL_TYPE] VARCHAR (20)  NOT NULL,
    [DESCRIPTION]   NVARCHAR (50) NULL,
    CONSTRAINT [RUBR_REFERRAL_TYPE_PK] PRIMARY KEY CLUSTERED ([REFERRAL_TYPE] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_REFERRAL_TYPE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_REFERRAL_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_REFERRAL_TYPE.REFERRAL_TYPE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_REFERRAL_TYPE', @level2type = N'COLUMN', @level2name = N'REFERRAL_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_REFERRAL_TYPE.DESCRIPTION', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_REFERRAL_TYPE', @level2type = N'COLUMN', @level2name = N'DESCRIPTION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_REFERRAL_TYPE.RUBR_REFERRAL_TYPE_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_REFERRAL_TYPE', @level2type = N'CONSTRAINT', @level2name = N'RUBR_REFERRAL_TYPE_PK';

