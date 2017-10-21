CREATE TABLE [FAXPEC].[USERROLES] (
    [USERID] VARCHAR (45) NOT NULL,
    [ROLEID] VARCHAR (45) NOT NULL,
    CONSTRAINT [PK_USERROLES_ID] PRIMARY KEY CLUSTERED ([USERID] ASC, [ROLEID] ASC),
    CONSTRAINT [FK_USERROLES_ROLEID] FOREIGN KEY ([ROLEID]) REFERENCES [FAXPEC].[ROLES] ([ID]),
    CONSTRAINT [FK_USERROLES_USERID] FOREIGN KEY ([USERID]) REFERENCES [FAXPEC].[USERS] ([ID])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.USERROLES.PK_USERROLES_ID', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'USERROLES', @level2type = N'CONSTRAINT', @level2name = N'PK_USERROLES_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.USERROLES', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'USERROLES';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.USERROLES.USERID', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'USERROLES', @level2type = N'COLUMN', @level2name = N'USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.USERROLES.ROLEID', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'USERROLES', @level2type = N'COLUMN', @level2name = N'ROLEID';

