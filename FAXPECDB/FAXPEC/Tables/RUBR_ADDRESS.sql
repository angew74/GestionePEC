﻿CREATE TABLE [FAXPEC].[RUBR_ADDRESS] (
    [ID_ADDRESS]             NUMERIC (10)   NOT NULL,
    [INDIRIZZO]              NVARCHAR (500) NULL,
    [CIVICO]                 VARCHAR (20)   NULL,
    [CAP]                    VARCHAR (5)    NULL,
    [COMUNE]                 NVARCHAR (100) NOT NULL,
    [SIGLA_PROV]             VARCHAR (2)    NULL,
    [COD_ISO_STATO]          NVARCHAR (5)   NULL,
    [FLG_IPA]                VARCHAR (1)    DEFAULT ((0)) NOT NULL,
    [INDIRIZZO_NON_STANDARD] VARCHAR (1)    DEFAULT ((0)) NOT NULL,
    [AFF_IPA]                NUMERIC (3)    NULL,
    CONSTRAINT [RUBR_ADDRESS_PK] PRIMARY KEY CLUSTERED ([ID_ADDRESS] ASC),
    CONSTRAINT [RUBR_ADDRESS_CHK1] CHECK ([FLG_IPA]='1' OR [FLG_IPA]='0')
);


GO

CREATE TRIGGER [FAXPEC].[InsteadOfInsertOn$RUBR_ADDRESS]
   ON [FAXPEC].[RUBR_ADDRESS]
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
               @new$ID_ADDRESS numeric(10, 0), 
               @new$INDIRIZZO nvarchar(500), 
               @new$CIVICO varchar(20), 
               @new$CAP varchar(5), 
               @new$COMUNE nvarchar(100), 
               @new$SIGLA_PROV varchar(2), 
               @new$COD_ISO_STATO nvarchar(5), 
               @new$FLG_IPA varchar(1), 
               @new$INDIRIZZO_NON_STANDARD varchar(1), 
               @new$AFF_IPA numeric(3, 0)

            DECLARE
                ForEachInsertedRowTriggerCursor CURSOR LOCAL FORWARD_ONLY READ_ONLY FOR 
                  SELECT                   
                     ID_ADDRESS, 
                     INDIRIZZO, 
                     CIVICO, 
                     CAP, 
                     COMUNE, 
                     SIGLA_PROV, 
                     COD_ISO_STATO, 
                     FLG_IPA, 
                     INDIRIZZO_NON_STANDARD, 
                     AFF_IPA
                  FROM inserted

            OPEN ForEachInsertedRowTriggerCursor

            FETCH ForEachInsertedRowTriggerCursor
                INTO                
                  @new$ID_ADDRESS, 
                  @new$INDIRIZZO, 
                  @new$CIVICO, 
                  @new$CAP, 
                  @new$COMUNE, 
                  @new$SIGLA_PROV, 
                  @new$COD_ISO_STATO, 
                  @new$FLG_IPA, 
                  @new$INDIRIZZO_NON_STANDARD, 
                  @new$AFF_IPA

            WHILE @@fetch_status = 0
            
               BEGIN

                  /* row-level triggers implementation: begin*/
                  BEGIN
                     BEGIN
                        IF @triggerType = 'I'
                           BEGIN                           
                                 SELECT @new$ID_ADDRESS = NEXT VALUE FOR FAXPEC.RUBR_ADDRESS_SEQ
                           END
                     END
                  END
                  /* row-level triggers implementation: end*/

                  /* DML-operation emulation*/
                  INSERT FAXPEC.RUBR_ADDRESS(                   
                     ID_ADDRESS, 
                     INDIRIZZO, 
                     CIVICO, 
                     CAP, 
                     COMUNE, 
                     SIGLA_PROV, 
                     COD_ISO_STATO, 
                     FLG_IPA, 
                     INDIRIZZO_NON_STANDARD, 
                     AFF_IPA)
                     VALUES (                       
                        @new$ID_ADDRESS, 
                        @new$INDIRIZZO, 
                        @new$CIVICO, 
                        @new$CAP, 
                        @new$COMUNE, 
                        @new$SIGLA_PROV, 
                        @new$COD_ISO_STATO, 
                        @new$FLG_IPA, 
                        @new$INDIRIZZO_NON_STANDARD, 
                        @new$AFF_IPA)

                  FETCH ForEachInsertedRowTriggerCursor
                      INTO                        
                        @new$ID_ADDRESS, 
                        @new$INDIRIZZO, 
                        @new$CIVICO, 
                        @new$CAP, 
                        @new$COMUNE, 
                        @new$SIGLA_PROV, 
                        @new$COD_ISO_STATO, 
                        @new$FLG_IPA, 
                        @new$INDIRIZZO_NON_STANDARD, 
                        @new$AFF_IPA

               END

            CLOSE ForEachInsertedRowTriggerCursor

            DEALLOCATE ForEachInsertedRowTriggerCursor

         END

GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ADDRESS', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ADDRESS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ADDRESS.ID_ADDRESS', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ADDRESS', @level2type = N'COLUMN', @level2name = N'ID_ADDRESS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ADDRESS.INDIRIZZO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ADDRESS', @level2type = N'COLUMN', @level2name = N'INDIRIZZO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ADDRESS.CIVICO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ADDRESS', @level2type = N'COLUMN', @level2name = N'CIVICO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ADDRESS.CAP', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ADDRESS', @level2type = N'COLUMN', @level2name = N'CAP';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ADDRESS.COMUNE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ADDRESS', @level2type = N'COLUMN', @level2name = N'COMUNE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ADDRESS.SIGLA_PROV', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ADDRESS', @level2type = N'COLUMN', @level2name = N'SIGLA_PROV';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ADDRESS.COD_ISO_STATO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ADDRESS', @level2type = N'COLUMN', @level2name = N'COD_ISO_STATO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ADDRESS.FLG_IPA', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ADDRESS', @level2type = N'COLUMN', @level2name = N'FLG_IPA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ADDRESS.INDIRIZZO_NON_STANDARD', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ADDRESS', @level2type = N'COLUMN', @level2name = N'INDIRIZZO_NON_STANDARD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ADDRESS.AFF_IPA', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ADDRESS', @level2type = N'COLUMN', @level2name = N'AFF_IPA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ADDRESS.RUBR_ADDRESS_CHK1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ADDRESS', @level2type = N'CONSTRAINT', @level2name = N'RUBR_ADDRESS_CHK1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.RUBR_ADDRESS.RUBR_ADDRESS_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'RUBR_ADDRESS', @level2type = N'CONSTRAINT', @level2name = N'RUBR_ADDRESS_PK';

