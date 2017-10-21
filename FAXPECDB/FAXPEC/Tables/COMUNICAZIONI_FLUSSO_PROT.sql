﻿CREATE TABLE [FAXPEC].[COMUNICAZIONI_FLUSSO_PROT] (
    [REF_ID_COM]      NUMERIC (10)  NOT NULL,
    [STATO_OLD]       NUMERIC (2)   NULL,
    [STATO_NEW]       NUMERIC (2)   NOT NULL,
    [DATA_OPERAZIONE] DATETIME2 (6) DEFAULT (sysdatetime()) NOT NULL,
    [UTE_OPE]         VARCHAR (50)  NOT NULL,
    [ID_FLUSSO]       NUMERIC (10)  NOT NULL,
    CONSTRAINT [COMUNICAZIONI_FLUSSO_PROT_PK] PRIMARY KEY CLUSTERED ([ID_FLUSSO] ASC),
    CONSTRAINT [COMUNICAZIONI_FLUSSO_PRO_CHK1] CHECK ([STATO_OLD]=(5) OR [STATO_OLD]=(4) OR [STATO_OLD]=(3) OR [STATO_OLD]=(2) OR [STATO_OLD]=(1) OR [STATO_OLD]=(0)),
    CONSTRAINT [COMUNICAZIONI_FLUSSO_PRO_CHK2] CHECK ([STATO_NEW]=(5) OR [STATO_NEW]=(4) OR [STATO_NEW]=(3) OR [STATO_NEW]=(2) OR [STATO_NEW]=(1) OR [STATO_NEW]=(0)),
    CONSTRAINT [COMUNICAZIONI_FLUSSO_PROT_FK1] FOREIGN KEY ([REF_ID_COM]) REFERENCES [FAXPEC].[COMUNICAZIONI] ([ID_COM])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [COMUN_FLUSSO_PROT_INDEX1]
    ON [FAXPEC].[COMUNICAZIONI_FLUSSO_PROT]([REF_ID_COM] ASC, [DATA_OPERAZIONE] ASC);


GO

CREATE TRIGGER [FAXPEC].[InsteadOfInsertOn$COMUNICAZIONI_FLUSSO_PROT]
   ON [FAXPEC].[COMUNICAZIONI_FLUSSO_PROT]
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
               @new$REF_ID_COM numeric(10, 0), 
               @new$STATO_OLD numeric(2, 0), 
               @new$STATO_NEW numeric(2, 0), 
               @new$DATA_OPERAZIONE datetime2(6), 
               @new$UTE_OPE varchar(50), 
               @new$ID_FLUSSO numeric(10, 0)

            DECLARE
                ForEachInsertedRowTriggerCursor CURSOR LOCAL FORWARD_ONLY READ_ONLY FOR 
                  SELECT                      
                     REF_ID_COM, 
                     STATO_OLD, 
                     STATO_NEW, 
                     DATA_OPERAZIONE, 
                     UTE_OPE, 
                     ID_FLUSSO
                  FROM inserted

            OPEN ForEachInsertedRowTriggerCursor

            FETCH ForEachInsertedRowTriggerCursor
                INTO                 
                  @new$REF_ID_COM, 
                  @new$STATO_OLD, 
                  @new$STATO_NEW, 
                  @new$DATA_OPERAZIONE, 
                  @new$UTE_OPE, 
                  @new$ID_FLUSSO

            WHILE @@fetch_status = 0
            
               BEGIN

                  /* row-level triggers implementation: begin*/
                  BEGIN
                     BEGIN
                        IF @triggerType = 'I'
                           BEGIN
                              IF @new$ID_FLUSSO IS NULL
                                 SELECT @new$ID_FLUSSO = NEXT VALUE FOR FAXPEC.COM_PROT_FLUSSO_ID_SEQ
                           END
                     END
                  END
                  /* row-level triggers implementation: end*/

                  /* DML-operation emulation*/
                  INSERT FAXPEC.COMUNICAZIONI_FLUSSO_PROT(                    
                     REF_ID_COM, 
                     STATO_OLD, 
                     STATO_NEW, 
                     DATA_OPERAZIONE, 
                     UTE_OPE, 
                     ID_FLUSSO)
                     VALUES (                      
                        @new$REF_ID_COM, 
                        @new$STATO_OLD, 
                        @new$STATO_NEW, 
                        @new$DATA_OPERAZIONE, 
                        @new$UTE_OPE, 
                        @new$ID_FLUSSO)

                  FETCH ForEachInsertedRowTriggerCursor
                      INTO                        
                        @new$REF_ID_COM, 
                        @new$STATO_OLD, 
                        @new$STATO_NEW, 
                        @new$DATA_OPERAZIONE, 
                        @new$UTE_OPE, 
                        @new$ID_FLUSSO

               END

            CLOSE ForEachInsertedRowTriggerCursor

            DEALLOCATE ForEachInsertedRowTriggerCursor

         END

GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_FLUSSO_PROT.COMUN_FLUSSO_PROT_INDEX1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_FLUSSO_PROT', @level2type = N'INDEX', @level2name = N'COMUN_FLUSSO_PROT_INDEX1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_FLUSSO_PROT.COMUNICAZIONI_FLUSSO_PRO_CHK1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_FLUSSO_PROT', @level2type = N'CONSTRAINT', @level2name = N'COMUNICAZIONI_FLUSSO_PRO_CHK1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_FLUSSO_PROT.COMUNICAZIONI_FLUSSO_PRO_CHK2', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_FLUSSO_PROT', @level2type = N'CONSTRAINT', @level2name = N'COMUNICAZIONI_FLUSSO_PRO_CHK2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_FLUSSO_PROT', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_FLUSSO_PROT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_FLUSSO_PROT.REF_ID_COM', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_FLUSSO_PROT', @level2type = N'COLUMN', @level2name = N'REF_ID_COM';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_FLUSSO_PROT.STATO_OLD', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_FLUSSO_PROT', @level2type = N'COLUMN', @level2name = N'STATO_OLD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_FLUSSO_PROT.STATO_NEW', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_FLUSSO_PROT', @level2type = N'COLUMN', @level2name = N'STATO_NEW';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_FLUSSO_PROT.DATA_OPERAZIONE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_FLUSSO_PROT', @level2type = N'COLUMN', @level2name = N'DATA_OPERAZIONE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_FLUSSO_PROT.UTE_OPE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_FLUSSO_PROT', @level2type = N'COLUMN', @level2name = N'UTE_OPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_FLUSSO_PROT.ID_FLUSSO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_FLUSSO_PROT', @level2type = N'COLUMN', @level2name = N'ID_FLUSSO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_FLUSSO_PROT.COMUNICAZIONI_FLUSSO_PROT_FK1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_FLUSSO_PROT', @level2type = N'CONSTRAINT', @level2name = N'COMUNICAZIONI_FLUSSO_PROT_FK1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_FLUSSO_PROT.COMUNICAZIONI_FLUSSO_PROT_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_FLUSSO_PROT', @level2type = N'CONSTRAINT', @level2name = N'COMUNICAZIONI_FLUSSO_PROT_PK';

