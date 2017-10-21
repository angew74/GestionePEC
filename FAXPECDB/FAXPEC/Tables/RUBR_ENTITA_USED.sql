CREATE TABLE [FAXPEC].[RUBR_ENTITA_USED] (
    [ID_ENT_USED]     NUMERIC (10)    NOT NULL,
    [MAIL]            NVARCHAR (256)  NULL,
    [FAX]             VARCHAR (50)    NULL,
    [TELEFONO]        VARCHAR (50)    NULL,
    [CONTACT_REF]     NVARCHAR (200)  NULL,
    [REFERRAL_TYPE]   VARCHAR (20)    NULL,
    [COGNOME]         NVARCHAR (200)  NULL,
    [NOME]            NVARCHAR (100)  NULL,
    [COD_FIS]         VARCHAR (16)    NULL,
    [P_IVA]           VARCHAR (11)    NULL,
    [RAGIONE_SOCIALE] NVARCHAR (100)  NULL,
    [UFFICIO]         NVARCHAR (1000) NULL,
    [NOTE]            NVARCHAR (200)  NULL,
    [INDIRIZZO]       NVARCHAR (200)  NULL,
    [CIVICO]          VARCHAR (20)    NULL,
    [CAP]             VARCHAR (5)     NULL,
    [COMUNE]          NVARCHAR (100)  NULL,
    [SIGLA_PROV]      VARCHAR (2)     NULL,
    [COD_ISO_STATO]   NVARCHAR (5)    NULL,
    [ID_REFERRAL]     NUMERIC (10)    NULL,
    CONSTRAINT [RUBR_ENTITA_USED_PK_1] PRIMARY KEY CLUSTERED ([ID_ENT_USED] ASC)
);


GO
CREATE NONCLUSTERED INDEX [RUBR_ENTITA_USED_INDEX1]
    ON [FAXPEC].[RUBR_ENTITA_USED]([MAIL] ASC);


GO
ALTER INDEX [RUBR_ENTITA_USED_INDEX1]
    ON [FAXPEC].[RUBR_ENTITA_USED] DISABLE;


GO
CREATE NONCLUSTERED INDEX [RUBR_ENTITA_USED_INDEX2]
    ON [FAXPEC].[RUBR_ENTITA_USED]([FAX] ASC);


GO
ALTER INDEX [RUBR_ENTITA_USED_INDEX2]
    ON [FAXPEC].[RUBR_ENTITA_USED] DISABLE;


GO
CREATE NONCLUSTERED INDEX [RUBR_ENTITA_USED_INDEX3]
    ON [FAXPEC].[RUBR_ENTITA_USED]([TELEFONO] ASC);


GO
ALTER INDEX [RUBR_ENTITA_USED_INDEX3]
    ON [FAXPEC].[RUBR_ENTITA_USED] DISABLE;


GO
CREATE NONCLUSTERED INDEX [RUBR_ENTITA_USED_INDEX4]
    ON [FAXPEC].[RUBR_ENTITA_USED]([COGNOME] ASC, [NOME] ASC);


GO
ALTER INDEX [RUBR_ENTITA_USED_INDEX4]
    ON [FAXPEC].[RUBR_ENTITA_USED] DISABLE;


GO
CREATE NONCLUSTERED INDEX [RUBR_ENTITA_USED_INDEX5]
    ON [FAXPEC].[RUBR_ENTITA_USED]([CONTACT_REF] ASC);


GO
ALTER INDEX [RUBR_ENTITA_USED_INDEX5]
    ON [FAXPEC].[RUBR_ENTITA_USED] DISABLE;


GO
CREATE NONCLUSTERED INDEX [RUBR_ENTITA_USED_INDEX6]
    ON [FAXPEC].[RUBR_ENTITA_USED]([COD_FIS] ASC, [P_IVA] ASC);


GO
ALTER INDEX [RUBR_ENTITA_USED_INDEX6]
    ON [FAXPEC].[RUBR_ENTITA_USED] DISABLE;


GO
CREATE NONCLUSTERED INDEX [RUBR_ENTITA_USED_INDEX7]
    ON [FAXPEC].[RUBR_ENTITA_USED]([COD_ISO_STATO] ASC, [SIGLA_PROV] ASC, [COMUNE] ASC, [CAP] ASC, [INDIRIZZO] ASC, [CIVICO] ASC);


GO
ALTER INDEX [RUBR_ENTITA_USED_INDEX7]
    ON [FAXPEC].[RUBR_ENTITA_USED] DISABLE;


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.RUBR_ENTITA_USED_INDEX1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'INDEX', @level2name = N'RUBR_ENTITA_USED_INDEX1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.RUBR_ENTITA_USED_INDEX2', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'INDEX', @level2name = N'RUBR_ENTITA_USED_INDEX2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.RUBR_ENTITA_USED_INDEX3', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'INDEX', @level2name = N'RUBR_ENTITA_USED_INDEX3';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.RUBR_ENTITA_USED_INDEX4', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'INDEX', @level2name = N'RUBR_ENTITA_USED_INDEX4';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.RUBR_ENTITA_USED_INDEX5', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'INDEX', @level2name = N'RUBR_ENTITA_USED_INDEX5';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.RUBR_ENTITA_USED_INDEX6', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'INDEX', @level2name = N'RUBR_ENTITA_USED_INDEX6';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.RUBR_ENTITA_USED_INDEX7', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'INDEX', @level2name = N'RUBR_ENTITA_USED_INDEX7';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.ID_ENT_USED', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'ID_ENT_USED';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.MAIL', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'MAIL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.FAX', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'FAX';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.TELEFONO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'TELEFONO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.CONTACT_REF', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'CONTACT_REF';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.REFERRAL_TYPE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'REFERRAL_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.COGNOME', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'COGNOME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.NOME', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'NOME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.COD_FIS', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'COD_FIS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.P_IVA', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'P_IVA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.RAGIONE_SOCIALE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'RAGIONE_SOCIALE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.UFFICIO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'UFFICIO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.NOTE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'NOTE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.INDIRIZZO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'INDIRIZZO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.CIVICO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'CIVICO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.CAP', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'CAP';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.COMUNE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'COMUNE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.SIGLA_PROV', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'SIGLA_PROV';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.COD_ISO_STATO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'COD_ISO_STATO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.ID_REFERRAL', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'COLUMN', @level2name = N'ID_REFERRAL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ENTITA_USED.RUBR_ENTITA_USED_PK_1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ENTITA_USED', @level2type = N'CONSTRAINT', @level2name = N'RUBR_ENTITA_USED_PK_1';

