﻿CREATE TABLE [FAXPEC].[RUBR_MAPPING_PROTOCOLLO] (
    [COD_APPL]    VARCHAR (20)     NOT NULL,
    [COD_ID]      VARCHAR (20)     NOT NULL,
    [STR_ORG]     VARCHAR (20)     NULL,
    [COD_UTE_RSP] VARCHAR (20)     NULL,
    [ID]          NUMERIC (10)     NOT NULL,
    [ROWID]       UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    CONSTRAINT [RUBR_MAPPING_PROTOCOLLO_PK] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [ROWID$INDEX]
    ON [FAXPEC].[RUBR_MAPPING_PROTOCOLLO]([ROWID] ASC);


GO
CREATE NONCLUSTERED INDEX [RUBR_MAPPING_PROTOCOLLO_INDEX1]
    ON [FAXPEC].[RUBR_MAPPING_PROTOCOLLO]([STR_ORG] ASC, [COD_UTE_RSP] ASC);


GO
CREATE NONCLUSTERED INDEX [RUBR_MAPPING_PROTOCOLLO_INDEX2]
    ON [FAXPEC].[RUBR_MAPPING_PROTOCOLLO]([COD_APPL] ASC, [COD_ID] ASC);


GO

CREATE TRIGGER [FAXPEC].[InsteadOfInsertOn$RUBR_MAPPING_PROTOCOLLO]
   ON [FAXPEC].[RUBR_MAPPING_PROTOCOLLO]
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
               @new$COD_APPL varchar(20), 
               @new$COD_ID varchar(20), 
               @new$STR_ORG varchar(20), 
               @new$COD_UTE_RSP varchar(20), 
               @new$ID numeric(10, 0)

            DECLARE
                ForEachInsertedRowTriggerCursor CURSOR LOCAL FORWARD_ONLY READ_ONLY FOR 
                  SELECT                  
                     COD_APPL, 
                     COD_ID, 
                     STR_ORG, 
                     COD_UTE_RSP, 
                     ID
                  FROM inserted

            OPEN ForEachInsertedRowTriggerCursor

            FETCH ForEachInsertedRowTriggerCursor
                INTO                 
                  @new$COD_APPL, 
                  @new$COD_ID, 
                  @new$STR_ORG, 
                  @new$COD_UTE_RSP, 
                  @new$ID

            WHILE @@fetch_status = 0
            
               BEGIN

                  /* row-level triggers implementation: begin*/
                  BEGIN
                     BEGIN
                        IF @triggerType = 'I'
                           BEGIN
                              IF @new$ID IS NULL
                                 SELECT @new$ID = NEXT VALUE FOR FAXPEC.RUBR_MAPPING_PROT_ID_SEQ
                           END
                     END
                  END
                  /* row-level triggers implementation: end*/

                  /* DML-operation emulation*/
                  INSERT FAXPEC.RUBR_MAPPING_PROTOCOLLO(                   
                     COD_APPL, 
                     COD_ID, 
                     STR_ORG, 
                     COD_UTE_RSP, 
                     ID)
                     VALUES (                
                        @new$COD_APPL, 
                        @new$COD_ID, 
                        @new$STR_ORG, 
                        @new$COD_UTE_RSP, 
                        @new$ID)

                  FETCH ForEachInsertedRowTriggerCursor
                      INTO                     
                        @new$COD_APPL, 
                        @new$COD_ID, 
                        @new$STR_ORG, 
                        @new$COD_UTE_RSP, 
                        @new$ID

               END

            CLOSE ForEachInsertedRowTriggerCursor

            DEALLOCATE ForEachInsertedRowTriggerCursor

         END

GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_MAPPING_PROTOCOLLO.RUBR_MAPPING_PROTOCOLLO_INDEX1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_MAPPING_PROTOCOLLO', @level2type = N'INDEX', @level2name = N'RUBR_MAPPING_PROTOCOLLO_INDEX1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_MAPPING_PROTOCOLLO.RUBR_MAPPING_PROTOCOLLO_INDEX2', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_MAPPING_PROTOCOLLO', @level2type = N'INDEX', @level2name = N'RUBR_MAPPING_PROTOCOLLO_INDEX2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_MAPPING_PROTOCOLLO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_MAPPING_PROTOCOLLO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_MAPPING_PROTOCOLLO.COD_APPL', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_MAPPING_PROTOCOLLO', @level2type = N'COLUMN', @level2name = N'COD_APPL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_MAPPING_PROTOCOLLO.COD_ID', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_MAPPING_PROTOCOLLO', @level2type = N'COLUMN', @level2name = N'COD_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_MAPPING_PROTOCOLLO.STR_ORG', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_MAPPING_PROTOCOLLO', @level2type = N'COLUMN', @level2name = N'STR_ORG';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_MAPPING_PROTOCOLLO.COD_UTE_RSP', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_MAPPING_PROTOCOLLO', @level2type = N'COLUMN', @level2name = N'COD_UTE_RSP';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_MAPPING_PROTOCOLLO.ID', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_MAPPING_PROTOCOLLO', @level2type = N'COLUMN', @level2name = N'ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_MAPPING_PROTOCOLLO.RUBR_MAPPING_PROTOCOLLO_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_MAPPING_PROTOCOLLO', @level2type = N'CONSTRAINT', @level2name = N'RUBR_MAPPING_PROTOCOLLO_PK';

