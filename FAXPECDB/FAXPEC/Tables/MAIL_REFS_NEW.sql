CREATE TABLE [FAXPEC].[MAIL_REFS_NEW] (
    [ID_REF]            NUMERIC (10)  IDENTITY (1, 1) NOT NULL,
    [REF_ID_MAIL]       NUMERIC (10)  NOT NULL,
    [MAIL_DESTINATARIO] VARCHAR (256) NULL,
    [TIPO_REF]          VARCHAR (5)   DEFAULT ('TO') NOT NULL,
    [REF_ID_COM_DEST]   NUMERIC (10)  NULL,
    CONSTRAINT [MAIL_REFS_NEW_PK] PRIMARY KEY CLUSTERED ([ID_REF] ASC),
    CONSTRAINT [MAIL_REFS_NEW_CHK1] CHECK ([TIPO_REF]='CCN' OR [TIPO_REF]='CC' OR [TIPO_REF]='TO'),
    CONSTRAINT [MAIL_REFS_NEW_COM_DEST_FK1] FOREIGN KEY ([REF_ID_COM_DEST]) REFERENCES [FAXPEC].[COMUNICAZIONI_DESTINATARI] ([ID_COM_DEST]) ON DELETE CASCADE,
    CONSTRAINT [MAIL_REFS_NEW_MAIL_CONTEN_FK1] FOREIGN KEY ([REF_ID_MAIL]) REFERENCES [FAXPEC].[MAIL_CONTENT] ([ID_MAIL]) ON DELETE CASCADE
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_REFS_NEW', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_REFS_NEW';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_REFS_NEW.ID_REF', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_REFS_NEW', @level2type = N'COLUMN', @level2name = N'ID_REF';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_REFS_NEW.REF_ID_MAIL', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_REFS_NEW', @level2type = N'COLUMN', @level2name = N'REF_ID_MAIL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_REFS_NEW.MAIL_DESTINATARIO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_REFS_NEW', @level2type = N'COLUMN', @level2name = N'MAIL_DESTINATARIO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_REFS_NEW.TIPO_REF', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_REFS_NEW', @level2type = N'COLUMN', @level2name = N'TIPO_REF';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_REFS_NEW.REF_ID_COM_DEST', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_REFS_NEW', @level2type = N'COLUMN', @level2name = N'REF_ID_COM_DEST';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_REFS_NEW.MAIL_REFS_NEW_CHK1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_REFS_NEW', @level2type = N'CONSTRAINT', @level2name = N'MAIL_REFS_NEW_CHK1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_REFS_NEW.MAIL_REFS_NEW_COM_DEST_FK1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_REFS_NEW', @level2type = N'CONSTRAINT', @level2name = N'MAIL_REFS_NEW_COM_DEST_FK1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_REFS_NEW.MAIL_REFS_NEW_MAIL_CONTEN_FK1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_REFS_NEW', @level2type = N'CONSTRAINT', @level2name = N'MAIL_REFS_NEW_MAIL_CONTEN_FK1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_REFS_NEW.MAIL_REFS_NEW_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_REFS_NEW', @level2type = N'CONSTRAINT', @level2name = N'MAIL_REFS_NEW_PK';

