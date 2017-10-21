CREATE TABLE [FAXPEC].[RUBR_CONTATTI_TAGS] (
    [REF_ID_CONTACT] NUMERIC (10) NOT NULL,
    [REF_ID_TAG]     NUMERIC (10) NOT NULL,
    CONSTRAINT [RUBR_CONTATTI_TAGS_PK] PRIMARY KEY CLUSTERED ([REF_ID_CONTACT] ASC, [REF_ID_TAG] ASC),
    CONSTRAINT [RUBR_CONTATTI_TAGS_RUBR_E_FK1] FOREIGN KEY ([REF_ID_CONTACT]) REFERENCES [FAXPEC].[RUBR_ENTITA] ([ID_REFERRAL]),
    CONSTRAINT [RUBR_CONTATTI_TAGS_RUBR_T_FK1] FOREIGN KEY ([REF_ID_TAG]) REFERENCES [FAXPEC].[RUBR_TAGS] ([ID_TAG])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_CONTATTI_TAGS', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_CONTATTI_TAGS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_CONTATTI_TAGS.REF_ID_CONTACT', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_CONTATTI_TAGS', @level2type = N'COLUMN', @level2name = N'REF_ID_CONTACT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_CONTATTI_TAGS.REF_ID_TAG', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_CONTATTI_TAGS', @level2type = N'COLUMN', @level2name = N'REF_ID_TAG';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_CONTATTI_TAGS.RUBR_CONTATTI_TAGS_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_CONTATTI_TAGS', @level2type = N'CONSTRAINT', @level2name = N'RUBR_CONTATTI_TAGS_PK';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_CONTATTI_TAGS.RUBR_CONTATTI_TAGS_RUBR_T_FK1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_CONTATTI_TAGS', @level2type = N'CONSTRAINT', @level2name = N'RUBR_CONTATTI_TAGS_RUBR_T_FK1';

