﻿CREATE TABLE [FAXPEC].[MAIL_SENDERS_TITOLI] (
    [ID_MAIL_SENDER] NUMERIC (10) NOT NULL,
    [ID_TITOLO]      NUMERIC (10) NOT NULL,
    CONSTRAINT [MAIL_SENDERS_TITOLI_PK] PRIMARY KEY CLUSTERED ([ID_MAIL_SENDER] ASC, [ID_TITOLO] ASC),
    CONSTRAINT [MAIL_SENDERS_TITOLI_SENDERS_FK] FOREIGN KEY ([ID_MAIL_SENDER]) REFERENCES [FAXPEC].[MAIL_SENDERS] ([ID_SENDER]) ON DELETE CASCADE,
    CONSTRAINT [MAIL_SENDERS_TITOLI_TITOLO_FK] FOREIGN KEY ([ID_TITOLO]) REFERENCES [FAXPEC].[COMUNICAZIONI_TITOLI] ([ID_TITOLO]) ON DELETE CASCADE
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_SENDERS_TITOLI', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_SENDERS_TITOLI';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_SENDERS_TITOLI.ID_MAIL_SENDER', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_SENDERS_TITOLI', @level2type = N'COLUMN', @level2name = N'ID_MAIL_SENDER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_SENDERS_TITOLI.ID_TITOLO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_SENDERS_TITOLI', @level2type = N'COLUMN', @level2name = N'ID_TITOLO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_SENDERS_TITOLI.MAIL_SENDERS_TITOLI_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_SENDERS_TITOLI', @level2type = N'CONSTRAINT', @level2name = N'MAIL_SENDERS_TITOLI_PK';

