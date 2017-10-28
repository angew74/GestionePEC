﻿CREATE TABLE [FAXPEC].[COMUNICAZIONI] (
    [ID_COM]             NUMERIC (10)  NOT NULL,
    [REF_ID_SOTTOTITOLO] NUMERIC (10)  NOT NULL,
    [FLG_NOTIFICA]       VARCHAR (1)   DEFAULT ((0)) NULL,
    [MAIL_NOTIFICA]      VARCHAR (100) NULL,
    [UTE_INS]            VARCHAR (50)  NULL,
    [COD_APP_INS]        VARCHAR (20)  NULL,
    [ORIG_UID]           VARCHAR (50)  NULL,
    [AZ_PROT]            NUMERIC (1)   DEFAULT ((0)) NOT NULL,
    [IN_OUT]             NUMERIC (1)   DEFAULT ((1)) NOT NULL,
    [DELAY]              DATETIME2 (0) NULL,
    [UNIQUE_ID_MAPPER]   VARCHAR (100) NULL,
    CONSTRAINT [COMUNICAZIONI_PK] PRIMARY KEY CLUSTERED ([ID_COM] ASC),
    CONSTRAINT [COMUNICAZIONI_CHK1] CHECK ([AZ_PROT]=(1) OR [AZ_PROT]=(0)),
    CONSTRAINT [COMUNICAZIONI_CHK2] CHECK ([IN_OUT]=(1) OR [IN_OUT]=(0)),
    CONSTRAINT [COMUNICAZIONI_COMUNICAZIO_FK1] FOREIGN KEY ([REF_ID_SOTTOTITOLO]) REFERENCES [FAXPEC].[COMUNICAZIONI_SOTTOTITOLI] ([ID_SOTTOTITOLO]) ON DELETE CASCADE
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [COMUNICAZIONI_INDEX1]
    ON [FAXPEC].[COMUNICAZIONI]([UNIQUE_ID_MAPPER] ASC);


GO

CREATE TRIGGER [FAXPEC].[InsteadOfInsertOn$COMUNICAZIONI]
   ON [FAXPEC].[COMUNICAZIONI]
    INSTEAD OF INSERT
      AS 
         /*Generated by SQL Server Migration Assistant for Oracle version 7.3.0.*/
         BEGIN

            SET  NOCOUNT  ON

            DECLARE
               @triggerType char(1)

            SELECT @triggerType = 'I'

            /* column variables declaration*/
            DECLARE            
               @new$ID_COM numeric(10, 0), 
               @new$REF_ID_SOTTOTITOLO numeric(10, 0), 
               @new$FLG_NOTIFICA varchar(1), 
               @new$MAIL_NOTIFICA varchar(100), 
               @new$UTE_INS varchar(50), 
               @new$COD_APP_INS varchar(20), 
               @new$ORIG_UID varchar(50), 
               @new$AZ_PROT numeric(1, 0), 
               @new$IN_OUT numeric(1, 0), 
               @new$DELAY datetime2(0), 
               @new$UNIQUE_ID_MAPPER varchar(100)

            DECLARE
                ForEachInsertedRowTriggerCursor CURSOR LOCAL FORWARD_ONLY READ_ONLY FOR 
                  SELECT
                     ID_COM, 
                     REF_ID_SOTTOTITOLO, 
                     FLG_NOTIFICA, 
                     MAIL_NOTIFICA, 
                     UTE_INS, 
                     COD_APP_INS, 
                     ORIG_UID, 
                     AZ_PROT, 
                     IN_OUT, 
                     DELAY, 
                     UNIQUE_ID_MAPPER
                  FROM inserted

            OPEN ForEachInsertedRowTriggerCursor

            FETCH ForEachInsertedRowTriggerCursor
                INTO
                  @new$ID_COM, 
                  @new$REF_ID_SOTTOTITOLO, 
                  @new$FLG_NOTIFICA, 
                  @new$MAIL_NOTIFICA, 
                  @new$UTE_INS, 
                  @new$COD_APP_INS, 
                  @new$ORIG_UID, 
                  @new$AZ_PROT, 
                  @new$IN_OUT, 
                  @new$DELAY, 
                  @new$UNIQUE_ID_MAPPER

            WHILE @@fetch_status = 0
            
               BEGIN

                  /* row-level triggers implementation: begin*/
                  BEGIN
                     BEGIN
                        IF @triggerType = 'I'
                           BEGIN                            
                                 SELECT @new$ID_COM = NEXT VALUE FOR FAXPEC.COMUNICAZIONI_SEQ
                           END
                     END
                  END
                  /* row-level triggers implementation: end*/

                  /* DML-operation emulation*/
                  INSERT FAXPEC.COMUNICAZIONI(                 
                     ID_COM, 
                     REF_ID_SOTTOTITOLO, 
                     FLG_NOTIFICA, 
                     MAIL_NOTIFICA, 
                     UTE_INS, 
                     COD_APP_INS, 
                     ORIG_UID, 
                     AZ_PROT, 
                     IN_OUT, 
                     DELAY, 
                     UNIQUE_ID_MAPPER)
                     VALUES (                       
                        @new$ID_COM, 
                        @new$REF_ID_SOTTOTITOLO, 
                        @new$FLG_NOTIFICA, 
                        @new$MAIL_NOTIFICA, 
                        @new$UTE_INS, 
                        @new$COD_APP_INS, 
                        @new$ORIG_UID, 
                        @new$AZ_PROT, 
                        @new$IN_OUT, 
                        @new$DELAY, 
                        @new$UNIQUE_ID_MAPPER)

                  FETCH ForEachInsertedRowTriggerCursor
                      INTO                       
                        @new$ID_COM, 
                        @new$REF_ID_SOTTOTITOLO, 
                        @new$FLG_NOTIFICA, 
                        @new$MAIL_NOTIFICA, 
                        @new$UTE_INS, 
                        @new$COD_APP_INS, 
                        @new$ORIG_UID, 
                        @new$AZ_PROT, 
                        @new$IN_OUT, 
                        @new$DELAY, 
                        @new$UNIQUE_ID_MAPPER

               END

            CLOSE ForEachInsertedRowTriggerCursor

            DEALLOCATE ForEachInsertedRowTriggerCursor

         END

GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI.COMUNICAZIONI_INDEX1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI', @level2type = N'INDEX', @level2name = N'COMUNICAZIONI_INDEX1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI.ID_COM', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI', @level2type = N'COLUMN', @level2name = N'ID_COM';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI.REF_ID_SOTTOTITOLO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI', @level2type = N'COLUMN', @level2name = N'REF_ID_SOTTOTITOLO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI.FLG_NOTIFICA', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI', @level2type = N'COLUMN', @level2name = N'FLG_NOTIFICA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI.MAIL_NOTIFICA', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI', @level2type = N'COLUMN', @level2name = N'MAIL_NOTIFICA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI.UTE_INS', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI', @level2type = N'COLUMN', @level2name = N'UTE_INS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI.COD_APP_INS', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI', @level2type = N'COLUMN', @level2name = N'COD_APP_INS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI.ORIG_UID', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI', @level2type = N'COLUMN', @level2name = N'ORIG_UID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI.AZ_PROT', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI', @level2type = N'COLUMN', @level2name = N'AZ_PROT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI.IN_OUT', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI', @level2type = N'COLUMN', @level2name = N'IN_OUT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI.DELAY', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI', @level2type = N'COLUMN', @level2name = N'DELAY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI.UNIQUE_ID_MAPPER', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI', @level2type = N'COLUMN', @level2name = N'UNIQUE_ID_MAPPER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI.COMUNICAZIONI_CHK1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI', @level2type = N'CONSTRAINT', @level2name = N'COMUNICAZIONI_CHK1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI.COMUNICAZIONI_CHK2', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI', @level2type = N'CONSTRAINT', @level2name = N'COMUNICAZIONI_CHK2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI.COMUNICAZIONI_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI', @level2type = N'CONSTRAINT', @level2name = N'COMUNICAZIONI_PK';
