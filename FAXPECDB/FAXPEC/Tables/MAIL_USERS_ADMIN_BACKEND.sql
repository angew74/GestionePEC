CREATE TABLE [FAXPEC].[MAIL_USERS_ADMIN_BACKEND] (
    [REF_ID_USER] NUMERIC (10) NOT NULL,
    [DEPARTMENT]  NUMERIC (3)  NOT NULL,
    CONSTRAINT [MAIL_USERS_ADMIN_BACKEND_PK] PRIMARY KEY CLUSTERED ([REF_ID_USER] ASC, [DEPARTMENT] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_USERS_ADMIN_BACKEND', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_USERS_ADMIN_BACKEND';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_USERS_ADMIN_BACKEND.REF_ID_USER', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_USERS_ADMIN_BACKEND', @level2type = N'COLUMN', @level2name = N'REF_ID_USER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_USERS_ADMIN_BACKEND.DEPARTMENT', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_USERS_ADMIN_BACKEND', @level2type = N'COLUMN', @level2name = N'DEPARTMENT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_USERS_ADMIN_BACKEND.MAIL_USERS_ADMIN_BACKEND_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_USERS_ADMIN_BACKEND', @level2type = N'CONSTRAINT', @level2name = N'MAIL_USERS_ADMIN_BACKEND_PK';

