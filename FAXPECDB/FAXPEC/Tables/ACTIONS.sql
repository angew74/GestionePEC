CREATE TABLE [FAXPEC].[ACTIONS] (
    [ID]                     NUMERIC (18)  IDENTITY (1, 1) NOT NULL,
    [NOME_AZIONE]            VARCHAR (200) NOT NULL,
    [ID_NOME_DESTINAZIONE]   FLOAT (53)    NULL,
    [TIPO_DESTINAZIONE]      VARCHAR (1)   NULL,
    [TIPO_AZIONE]            VARCHAR (2)   NULL,
    [NUOVO_STATUS]           VARCHAR (2)   NULL,
    [ID_FOLDER_DESTINAZIONE] FLOAT (53)    NULL,
    CONSTRAINT [ACTIONS_PK] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.ACTIONS', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'ACTIONS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.ACTIONS.ID', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'ACTIONS', @level2type = N'COLUMN', @level2name = N'ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.ACTIONS.NOME_AZIONE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'ACTIONS', @level2type = N'COLUMN', @level2name = N'NOME_AZIONE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.ACTIONS.ID_NOME_DESTINAZIONE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'ACTIONS', @level2type = N'COLUMN', @level2name = N'ID_NOME_DESTINAZIONE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.ACTIONS.TIPO_DESTINAZIONE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'ACTIONS', @level2type = N'COLUMN', @level2name = N'TIPO_DESTINAZIONE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.ACTIONS.TIPO_AZIONE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'ACTIONS', @level2type = N'COLUMN', @level2name = N'TIPO_AZIONE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.ACTIONS.NUOVO_STATUS', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'ACTIONS', @level2type = N'COLUMN', @level2name = N'NUOVO_STATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.ACTIONS.ID_FOLDER_DESTINAZIONE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'ACTIONS', @level2type = N'COLUMN', @level2name = N'ID_FOLDER_DESTINAZIONE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.ACTIONS.ACTIONS_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'ACTIONS', @level2type = N'CONSTRAINT', @level2name = N'ACTIONS_PK';

