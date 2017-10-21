CREATE TABLE [FAXPEC].[COMUNICAZIONI_ALLEGATI] (
    [ID_ALLEGATO]     NUMERIC (10)    IDENTITY (1, 1) NOT NULL,
    [REF_ID_COM]      NUMERIC (10)    NOT NULL,
    [ALLEGATO_TPU]    VARCHAR (200)   NULL,
    [ALLEGATO_FILE]   VARBINARY (MAX) NULL,
    [ALLEGATO_EXT]    VARCHAR (5)     NULL,
    [FLG_INS_PROT]    VARCHAR (1)     DEFAULT ((0)) NOT NULL,
    [FLG_PROT_TO_UPL] VARCHAR (1)     DEFAULT ((0)) NOT NULL,
    [ALLEGATO_NAME]   VARCHAR (200)   NULL,
    [DA_LAVORARE]     NUMERIC (1)     DEFAULT ((0)) NOT NULL,
    CONSTRAINT [COMUNICAZIONI_ALLEGATI_PK] PRIMARY KEY CLUSTERED ([ID_ALLEGATO] ASC),
    CONSTRAINT [COMUNICAZIONI_ALLEGATI_CHK1] CHECK ([FLG_INS_PROT]='2' OR [FLG_INS_PROT]='1' OR [FLG_INS_PROT]='0'),
    CONSTRAINT [COMUNICAZIONI_ALLEGATI_CHK2] CHECK ([FLG_PROT_TO_UPL]='2' OR [FLG_PROT_TO_UPL]='1' OR [FLG_PROT_TO_UPL]='0'),
    CONSTRAINT [COMUNICAZIONI_ALLEGATI_CO_FK1] FOREIGN KEY ([REF_ID_COM]) REFERENCES [FAXPEC].[COMUNICAZIONI] ([ID_COM]) ON DELETE CASCADE
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_ALLEGATI', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_ALLEGATI';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_ALLEGATI.ID_ALLEGATO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_ALLEGATI', @level2type = N'COLUMN', @level2name = N'ID_ALLEGATO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_ALLEGATI.REF_ID_COM', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_ALLEGATI', @level2type = N'COLUMN', @level2name = N'REF_ID_COM';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_ALLEGATI.ALLEGATO_TPU', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_ALLEGATI', @level2type = N'COLUMN', @level2name = N'ALLEGATO_TPU';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_ALLEGATI.ALLEGATO_FILE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_ALLEGATI', @level2type = N'COLUMN', @level2name = N'ALLEGATO_FILE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_ALLEGATI.ALLEGATO_EXT', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_ALLEGATI', @level2type = N'COLUMN', @level2name = N'ALLEGATO_EXT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_ALLEGATI.FLG_INS_PROT', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_ALLEGATI', @level2type = N'COLUMN', @level2name = N'FLG_INS_PROT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_ALLEGATI.FLG_PROT_TO_UPL', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_ALLEGATI', @level2type = N'COLUMN', @level2name = N'FLG_PROT_TO_UPL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_ALLEGATI.ALLEGATO_NAME', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_ALLEGATI', @level2type = N'COLUMN', @level2name = N'ALLEGATO_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_ALLEGATI.DA_LAVORARE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_ALLEGATI', @level2type = N'COLUMN', @level2name = N'DA_LAVORARE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_ALLEGATI.COMUNICAZIONI_ALLEGATI_CHK1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_ALLEGATI', @level2type = N'CONSTRAINT', @level2name = N'COMUNICAZIONI_ALLEGATI_CHK1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_ALLEGATI.COMUNICAZIONI_ALLEGATI_CHK2', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_ALLEGATI', @level2type = N'CONSTRAINT', @level2name = N'COMUNICAZIONI_ALLEGATI_CHK2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_ALLEGATI.COMUNICAZIONI_ALLEGATI_CO_FK1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_ALLEGATI', @level2type = N'CONSTRAINT', @level2name = N'COMUNICAZIONI_ALLEGATI_CO_FK1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_ALLEGATI.COMUNICAZIONI_ALLEGATI_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_ALLEGATI', @level2type = N'CONSTRAINT', @level2name = N'COMUNICAZIONI_ALLEGATI_PK';

