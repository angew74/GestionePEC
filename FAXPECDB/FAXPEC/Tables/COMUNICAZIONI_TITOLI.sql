﻿CREATE TABLE [FAXPEC].[COMUNICAZIONI_TITOLI] (
    [ID_TITOLO] NUMERIC (10)   NOT NULL,
    [APP_CODE]  VARCHAR (15)   NOT NULL,
    [PROT_CODE] VARCHAR (15)   NULL,
    [TITOLO]    NVARCHAR (100) NULL,
    [ACTIVE]    NUMERIC (1)    DEFAULT ((1)) NULL,
    [NOTE]      VARCHAR (500)  NULL,
    CONSTRAINT [COMUNICAZIONI_TITOLI_PK] PRIMARY KEY CLUSTERED ([ID_TITOLO] ASC)
);


GO

CREATE TRIGGER [FAXPEC].[InsteadOfInsertOn$COMUNICAZIONI_TITOLI]
   ON [FAXPEC].[COMUNICAZIONI_TITOLI]
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
               @new$ID_TITOLO numeric(10, 0), 
               @new$APP_CODE varchar(15), 
               @new$PROT_CODE varchar(15), 
               @new$TITOLO nvarchar(100), 
               @new$ACTIVE numeric(1, 0), 
               @new$NOTE varchar(500)

            DECLARE
                ForEachInsertedRowTriggerCursor CURSOR LOCAL FORWARD_ONLY READ_ONLY FOR 
                  SELECT                   
                     ID_TITOLO, 
                     APP_CODE, 
                     PROT_CODE, 
                     TITOLO, 
                     ACTIVE, 
                     NOTE
                  FROM inserted

            OPEN ForEachInsertedRowTriggerCursor

            FETCH ForEachInsertedRowTriggerCursor
                INTO               
                  @new$ID_TITOLO, 
                  @new$APP_CODE, 
                  @new$PROT_CODE, 
                  @new$TITOLO, 
                  @new$ACTIVE, 
                  @new$NOTE

            WHILE @@fetch_status = 0
            
               BEGIN

                  /* row-level triggers implementation: begin*/
                  BEGIN
                     BEGIN
                        IF @triggerType = 'I'
                           BEGIN                           
                                 SELECT @new$ID_TITOLO = NEXT VALUE FOR FAXPEC.COMUNICAZIONI_TITOLI_SEQ
                           END
                     END
                  END
                  /* row-level triggers implementation: end*/

                  /* DML-operation emulation*/
                  INSERT FAXPEC.COMUNICAZIONI_TITOLI(                  
                     ID_TITOLO, 
                     APP_CODE, 
                     PROT_CODE, 
                     TITOLO, 
                     ACTIVE, 
                     NOTE)
                     VALUES (                      
                        @new$ID_TITOLO, 
                        @new$APP_CODE, 
                        @new$PROT_CODE, 
                        @new$TITOLO, 
                        @new$ACTIVE, 
                        @new$NOTE)

                  FETCH ForEachInsertedRowTriggerCursor
                      INTO                     
                        @new$ID_TITOLO, 
                        @new$APP_CODE, 
                        @new$PROT_CODE, 
                        @new$TITOLO, 
                        @new$ACTIVE, 
                        @new$NOTE

               END

            CLOSE ForEachInsertedRowTriggerCursor

            DEALLOCATE ForEachInsertedRowTriggerCursor

         END

GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_TITOLI', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_TITOLI';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_TITOLI.ID_TITOLO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_TITOLI', @level2type = N'COLUMN', @level2name = N'ID_TITOLO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_TITOLI.APP_CODE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_TITOLI', @level2type = N'COLUMN', @level2name = N'APP_CODE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_TITOLI.PROT_CODE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_TITOLI', @level2type = N'COLUMN', @level2name = N'PROT_CODE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_TITOLI.TITOLO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_TITOLI', @level2type = N'COLUMN', @level2name = N'TITOLO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_TITOLI.ACTIVE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_TITOLI', @level2type = N'COLUMN', @level2name = N'ACTIVE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_TITOLI.NOTE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_TITOLI', @level2type = N'COLUMN', @level2name = N'NOTE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_TITOLI.COMUNICAZIONI_TITOLI_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_TITOLI', @level2type = N'CONSTRAINT', @level2name = N'COMUNICAZIONI_TITOLI_PK';
