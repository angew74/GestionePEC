CREATE TABLE [FAXPEC].[ACTIONS_FOLDERS] (
    [ID]       NUMERIC (18) IDENTITY (1, 1) NOT NULL,
    [IDFOLDER] NUMERIC (18) NOT NULL,
    [IDACTION] NUMERIC (18) NOT NULL,
    CONSTRAINT [ACTIONS_FOLDERS_PK] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [ACTIONS_FOLDERS_FK1] FOREIGN KEY ([IDACTION]) REFERENCES [FAXPEC].[ACTIONS] ([ID]),
    CONSTRAINT [FK__ACTIONS_FOLDERS_ACTIONS] FOREIGN KEY ([IDFOLDER]) REFERENCES [FAXPEC].[FOLDERS] ([ID])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.ACTIONS_FOLDERS', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'ACTIONS_FOLDERS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.ACTIONS_FOLDERS.ID', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'ACTIONS_FOLDERS', @level2type = N'COLUMN', @level2name = N'ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.ACTIONS_FOLDERS.IDFOLDER', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'ACTIONS_FOLDERS', @level2type = N'COLUMN', @level2name = N'IDFOLDER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.ACTIONS_FOLDERS.IDACTION', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'ACTIONS_FOLDERS', @level2type = N'COLUMN', @level2name = N'IDACTION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.ACTIONS_FOLDERS.ACTIONS_FOLDERS_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'ACTIONS_FOLDERS', @level2type = N'CONSTRAINT', @level2name = N'ACTIONS_FOLDERS_PK';

