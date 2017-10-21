CREATE TABLE [FAXPEC].[MAIL_USERS_SENDER_BACKEND] (
    [REF_ID_USER]      NUMERIC (10)  NOT NULL,
    [REF_ID_SENDER]    NUMERIC (10)  NOT NULL,
    [ROLE]             VARCHAR (1)   DEFAULT ((0)) NOT NULL,
    [DATA_INSERIMENTO] DATETIME2 (0) DEFAULT (sysdatetime()) NOT NULL,
    CONSTRAINT [USERS_SENDER_BACKEND_PK] PRIMARY KEY CLUSTERED ([REF_ID_USER] ASC, [REF_ID_SENDER] ASC),
    CONSTRAINT [USERS_SENDER_BACKEND_MAIL_FK1] FOREIGN KEY ([REF_ID_SENDER]) REFERENCES [FAXPEC].[MAIL_SENDERS] ([ID_SENDER]),
    CONSTRAINT [USERS_SENDER_BACKEND_USER_FK1] FOREIGN KEY ([REF_ID_USER]) REFERENCES [FAXPEC].[MAIL_USERS_BACKEND] ([ID_USER]) ON DELETE CASCADE
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_USERS_SENDER_BACKEND', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_USERS_SENDER_BACKEND';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_USERS_SENDER_BACKEND.REF_ID_USER', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_USERS_SENDER_BACKEND', @level2type = N'COLUMN', @level2name = N'REF_ID_USER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_USERS_SENDER_BACKEND.REF_ID_SENDER', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_USERS_SENDER_BACKEND', @level2type = N'COLUMN', @level2name = N'REF_ID_SENDER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_USERS_SENDER_BACKEND.ROLE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_USERS_SENDER_BACKEND', @level2type = N'COLUMN', @level2name = N'ROLE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_USERS_SENDER_BACKEND.DATA_INSERIMENTO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_USERS_SENDER_BACKEND', @level2type = N'COLUMN', @level2name = N'DATA_INSERIMENTO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_USERS_SENDER_BACKEND.USERS_SENDER_BACKEND_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_USERS_SENDER_BACKEND', @level2type = N'CONSTRAINT', @level2name = N'USERS_SENDER_BACKEND_PK';

