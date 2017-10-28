﻿CREATE TABLE [FAXPEC].[COMUNICAZIONI_CANALI] (
    [ID_CANALE] NUMERIC (10) NOT NULL,
    [CODICE]    VARCHAR (20) NOT NULL,
    [PREFISSO]  VARCHAR (20) NULL,
    CONSTRAINT [COMUNICAZIONI_CANALI_PK] PRIMARY KEY CLUSTERED ([ID_CANALE] ASC),
    CONSTRAINT [COMUNICAZIONI_CANALI_UK1] UNIQUE NONCLUSTERED ([CODICE] ASC)
);


GO

CREATE TRIGGER [FAXPEC].[InsteadOfInsertOn$COMUNICAZIONI_CANALI]
   ON [FAXPEC].[COMUNICAZIONI_CANALI]
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
               @new$ID_CANALE numeric(10, 0), 
               @new$CODICE varchar(20), 
               @new$PREFISSO varchar(20)

            DECLARE
                ForEachInsertedRowTriggerCursor CURSOR LOCAL FORWARD_ONLY READ_ONLY FOR 
                  SELECT ID_CANALE, CODICE, PREFISSO
                  FROM inserted

            OPEN ForEachInsertedRowTriggerCursor

            FETCH ForEachInsertedRowTriggerCursor
                INTO @new$ID_CANALE, @new$CODICE, @new$PREFISSO

            WHILE @@fetch_status = 0
            
               BEGIN

                  /* row-level triggers implementation: begin*/
                  BEGIN
                     BEGIN
                        IF @triggerType = 'I'
                           BEGIN                         
                                 SELECT @new$ID_CANALE = NEXT VALUE FOR FAXPEC.COMUNICAZIONI_CANALI_SEQ
                           END
                     END
                  END
                  /* row-level triggers implementation: end*/

                  /* DML-operation emulation*/
                  INSERT FAXPEC.COMUNICAZIONI_CANALI(ID_CANALE, CODICE, PREFISSO)
                     VALUES (@new$ID_CANALE, @new$CODICE, @new$PREFISSO)

                  FETCH ForEachInsertedRowTriggerCursor
                      INTO @new$ID_CANALE, @new$CODICE, @new$PREFISSO

               END

            CLOSE ForEachInsertedRowTriggerCursor

            DEALLOCATE ForEachInsertedRowTriggerCursor

         END

GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_CANALI', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_CANALI';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_CANALI.ID_CANALE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_CANALI', @level2type = N'COLUMN', @level2name = N'ID_CANALE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_CANALI.CODICE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_CANALI', @level2type = N'COLUMN', @level2name = N'CODICE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_CANALI.PREFISSO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_CANALI', @level2type = N'COLUMN', @level2name = N'PREFISSO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_CANALI.COMUNICAZIONI_CANALI_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_CANALI', @level2type = N'CONSTRAINT', @level2name = N'COMUNICAZIONI_CANALI_PK';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_CANALI.COMUNICAZIONI_CANALI_UK1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_CANALI', @level2type = N'CONSTRAINT', @level2name = N'COMUNICAZIONI_CANALI_UK1';
