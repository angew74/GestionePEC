CREATE TABLE [FAXPEC].[IPA] (
    [ID_RUB]         FLOAT (53)     NOT NULL,
    [COGNOME]        VARCHAR (200)  NULL,
    [NOME]           VARCHAR (100)  NULL,
    [CODFIS]         VARCHAR (16)   NULL,
    [PIVA]           VARCHAR (11)   NULL,
    [RAGIONESOCIALE] VARCHAR (100)  NULL,
    [UFFICIO]        VARCHAR (1000) NULL,
    [MAIL]           NVARCHAR (100) NULL,
    [FAX]            VARCHAR (50)   NULL,
    [TELEFONO]       VARCHAR (50)   NULL,
    [STATO]          VARCHAR (50)   NULL,
    [PROVINCIA]      VARCHAR (2)    NULL,
    [CITTA]          VARCHAR (50)   NULL,
    [SEDIME]         VARCHAR (10)   NULL,
    [VIA]            VARCHAR (80)   NULL,
    [NUMERO]         VARCHAR (8)    NULL,
    [LETTERA]        VARCHAR (5)    NULL,
    [ID_TITOLO]      FLOAT (53)     NULL,
    [CAP]            VARCHAR (5)    NULL,
    [CERTIFIED]      NUMERIC (1)    NULL,
    [NOTE]           VARCHAR (200)  NULL,
    [FLG_IPA]        CHAR (1)       NULL,
    [DN]             VARCHAR (200)  NULL,
    [COD_UNIVOCO]    VARCHAR (30)   NULL,
    [ID_PADRE]       FLOAT (53)     NULL,
    [MAIL_DOMAIN]    VARCHAR (100)  NULL,
    [REFERRAL_TYPE]  VARCHAR (20)   NULL,
    [REF_ORG]        FLOAT (53)     NULL
);


GO
CREATE NONCLUSTERED INDEX [IPA_INDEX1]
    ON [FAXPEC].[IPA]([REF_ORG] DESC);


GO
ALTER INDEX [IPA_INDEX1]
    ON [FAXPEC].[IPA] DISABLE;


GO
CREATE NONCLUSTERED INDEX [IPA_INDEX2]
    ON [FAXPEC].[IPA]([COD_UNIVOCO] ASC);


GO
ALTER INDEX [IPA_INDEX2]
    ON [FAXPEC].[IPA] DISABLE;


GO
CREATE NONCLUSTERED INDEX [IPA_INDEX3]
    ON [FAXPEC].[IPA]([DN] ASC);


GO
ALTER INDEX [IPA_INDEX3]
    ON [FAXPEC].[IPA] DISABLE;


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.IPA_INDEX1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'INDEX', @level2name = N'IPA_INDEX1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.IPA_INDEX2', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'INDEX', @level2name = N'IPA_INDEX2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.IPA_INDEX3', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'INDEX', @level2name = N'IPA_INDEX3';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.ID_RUB', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'ID_RUB';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.COGNOME', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'COGNOME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.NOME', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'NOME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.CODFIS', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'CODFIS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.PIVA', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'PIVA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.RAGIONESOCIALE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'RAGIONESOCIALE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.UFFICIO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'UFFICIO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.MAIL', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'MAIL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.FAX', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'FAX';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.TELEFONO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'TELEFONO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.STATO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'STATO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.PROVINCIA', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'PROVINCIA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.CITTA', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'CITTA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.SEDIME', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'SEDIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.VIA', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'VIA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.NUMERO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'NUMERO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.LETTERA', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'LETTERA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.ID_TITOLO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'ID_TITOLO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.CAP', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'CAP';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.CERTIFIED', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'CERTIFIED';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.NOTE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'NOTE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.FLG_IPA', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'FLG_IPA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.DN', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'DN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.COD_UNIVOCO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'COD_UNIVOCO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.ID_PADRE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'ID_PADRE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.MAIL_DOMAIN', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'MAIL_DOMAIN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.REFERRAL_TYPE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'REFERRAL_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.IPA.REF_ORG', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'IPA', @level2type = N'COLUMN', @level2name = N'REF_ORG';

