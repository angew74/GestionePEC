CREATE TABLE [FAXPEC].[FOLDERS_SENDERS] (
    [ID]       NUMERIC (18) IDENTITY (1, 1) NOT NULL,
    [IDFOLDER] NUMERIC (18) NOT NULL,
    [IDSENDER] NUMERIC (10) NOT NULL,
    CONSTRAINT [FOLDERS_SENDERS_PK] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_FOLDERS_SENDERS_FOLDERS] FOREIGN KEY ([IDFOLDER]) REFERENCES [FAXPEC].[FOLDERS] ([ID]),
    CONSTRAINT [FK_FOLDERS_SENDERS_SENDERS] FOREIGN KEY ([IDSENDER]) REFERENCES [FAXPEC].[MAIL_SENDERS] ([ID_SENDER])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.FOLDERS_SENDERS', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'FOLDERS_SENDERS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.FOLDERS_SENDERS.ID', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'FOLDERS_SENDERS', @level2type = N'COLUMN', @level2name = N'ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.FOLDERS_SENDERS.IDFOLDER', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'FOLDERS_SENDERS', @level2type = N'COLUMN', @level2name = N'IDFOLDER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.FOLDERS_SENDERS.IDSENDER', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'FOLDERS_SENDERS', @level2type = N'COLUMN', @level2name = N'IDSENDER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.FOLDERS_SENDERS.FOLDERS_SENDERS_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'FOLDERS_SENDERS', @level2type = N'CONSTRAINT', @level2name = N'FOLDERS_SENDERS_PK';

