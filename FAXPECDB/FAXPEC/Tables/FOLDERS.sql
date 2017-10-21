CREATE TABLE [FAXPEC].[FOLDERS] (
    [ID]     NUMERIC (18)  NOT NULL,
    [NOME]   VARCHAR (200) NOT NULL,
    [TIPO]   VARCHAR (1)   NOT NULL,
    [SYSTEM] VARCHAR (1)   NULL,
    [IDNOME] FLOAT (53)    NULL,
    CONSTRAINT [FOLDERS_PK] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.FOLDERS', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'FOLDERS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.FOLDERS.ID', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'FOLDERS', @level2type = N'COLUMN', @level2name = N'ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.FOLDERS.NOME', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'FOLDERS', @level2type = N'COLUMN', @level2name = N'NOME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.FOLDERS.TIPO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'FOLDERS', @level2type = N'COLUMN', @level2name = N'TIPO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.FOLDERS.SYSTEM', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'FOLDERS', @level2type = N'COLUMN', @level2name = N'SYSTEM';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.FOLDERS.IDNOME', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'FOLDERS', @level2type = N'COLUMN', @level2name = N'IDNOME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.FOLDERS.FOLDERS_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'FOLDERS', @level2type = N'CONSTRAINT', @level2name = N'FOLDERS_PK';

