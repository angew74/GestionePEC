﻿CREATE TABLE [FAXPEC].[COMUNICAZIONI_SOTTOTITOLI] (
    [ID_SOTTOTITOLO]     NUMERIC (10)   NOT NULL,
    [REF_ID_TITOLO]      NUMERIC (10)   NOT NULL,
    [SOTTOTITOLO]        NVARCHAR (100) NULL,
    [ACTIVE]             NUMERIC (1)    DEFAULT ((1)) NOT NULL,
    [NOTE]               VARCHAR (500)  NULL,
    [COM_CODE]           VARCHAR (15)   NULL,
    [PROT_MANAGED]       NUMERIC (1)    DEFAULT ((0)) NOT NULL,
    [PROT_SUBCODE]       VARCHAR (20)   NULL,
    [PROT_PWD]           VARCHAR (20)   NULL,
    [PROT_TIPI_AMMESSI]  VARCHAR (20)   NULL,
    [PROT_LOAD_ALLEGATI] NUMERIC (1)    DEFAULT ((0)) NOT NULL,
    [PROT_CODE]          VARCHAR (20)   NULL,
    CONSTRAINT [COMUNICAZIONI_SOTTOTITOLI_PK] PRIMARY KEY CLUSTERED ([ID_SOTTOTITOLO] ASC),
    CONSTRAINT [COMUNICAZIONI_SOTTOTITOL_CHK1] CHECK ([ACTIVE]=(1) OR [ACTIVE]=(0)),
    CONSTRAINT [COMUNICAZIONI_SOTTOTITOL_CHK2] CHECK ([PROT_MANAGED]=(1) OR [PROT_MANAGED]=(0)),
    CONSTRAINT [COMUNICAZIONI_SOTTOTITOL_CHK3] CHECK ([PROT_LOAD_ALLEGATI]=(1) OR [PROT_LOAD_ALLEGATI]=(0)),
    CONSTRAINT [COMUNICAZIONI_SOTTOTITOLI_FK1] FOREIGN KEY ([REF_ID_TITOLO]) REFERENCES [FAXPEC].[COMUNICAZIONI_TITOLI] ([ID_TITOLO])
);


GO
CREATE NONCLUSTERED INDEX [COMUN_SOTTOTITOLI_INDEX1]
    ON [FAXPEC].[COMUNICAZIONI_SOTTOTITOLI]([REF_ID_TITOLO] ASC);


GO
ALTER INDEX [COMUN_SOTTOTITOLI_INDEX1]
    ON [FAXPEC].[COMUNICAZIONI_SOTTOTITOLI] DISABLE;


GO

CREATE TRIGGER [FAXPEC].[InsteadOfInsertOn$COMUNICAZIONI_SOTTOTITOLI]
   ON [FAXPEC].[COMUNICAZIONI_SOTTOTITOLI]
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
               @new$ID_SOTTOTITOLO numeric(10, 0), 
               @new$REF_ID_TITOLO numeric(10, 0), 
               @new$SOTTOTITOLO nvarchar(100), 
               @new$ACTIVE numeric(1, 0), 
               @new$NOTE varchar(500), 
               @new$COM_CODE varchar(15), 
               @new$PROT_MANAGED numeric(1, 0), 
               @new$PROT_SUBCODE varchar(20), 
               @new$PROT_PWD varchar(20), 
               @new$PROT_TIPI_AMMESSI varchar(20), 
               @new$PROT_LOAD_ALLEGATI numeric(1, 0), 
               @new$PROT_CODE varchar(20)

            DECLARE
                ForEachInsertedRowTriggerCursor CURSOR LOCAL FORWARD_ONLY READ_ONLY FOR 
                  SELECT                     
                     ID_SOTTOTITOLO, 
                     REF_ID_TITOLO, 
                     SOTTOTITOLO, 
                     ACTIVE, 
                     NOTE, 
                     COM_CODE, 
                     PROT_MANAGED, 
                     PROT_SUBCODE, 
                     PROT_PWD, 
                     PROT_TIPI_AMMESSI, 
                     PROT_LOAD_ALLEGATI, 
                     PROT_CODE
                  FROM inserted

            OPEN ForEachInsertedRowTriggerCursor

            FETCH ForEachInsertedRowTriggerCursor
                INTO              
                  @new$ID_SOTTOTITOLO, 
                  @new$REF_ID_TITOLO, 
                  @new$SOTTOTITOLO, 
                  @new$ACTIVE, 
                  @new$NOTE, 
                  @new$COM_CODE, 
                  @new$PROT_MANAGED, 
                  @new$PROT_SUBCODE, 
                  @new$PROT_PWD, 
                  @new$PROT_TIPI_AMMESSI, 
                  @new$PROT_LOAD_ALLEGATI, 
                  @new$PROT_CODE

            WHILE @@fetch_status = 0
            
               BEGIN

                  /* row-level triggers implementation: begin*/
                  BEGIN
                     BEGIN
                        IF @triggerType = 'I'
                           BEGIN                           
                                 SELECT @new$ID_SOTTOTITOLO = NEXT VALUE FOR FAXPEC.COMUNICAZIONI_SOTTOTITOLI_SEQ
                           END
                     END
                  END
                  /* row-level triggers implementation: end*/

                  /* DML-operation emulation*/
                  INSERT FAXPEC.COMUNICAZIONI_SOTTOTITOLI(                   
                     ID_SOTTOTITOLO, 
                     REF_ID_TITOLO, 
                     SOTTOTITOLO, 
                     ACTIVE, 
                     NOTE, 
                     COM_CODE, 
                     PROT_MANAGED, 
                     PROT_SUBCODE, 
                     PROT_PWD, 
                     PROT_TIPI_AMMESSI, 
                     PROT_LOAD_ALLEGATI, 
                     PROT_CODE)
                     VALUES (                      
                        @new$ID_SOTTOTITOLO, 
                        @new$REF_ID_TITOLO, 
                        @new$SOTTOTITOLO, 
                        @new$ACTIVE, 
                        @new$NOTE, 
                        @new$COM_CODE, 
                        @new$PROT_MANAGED, 
                        @new$PROT_SUBCODE, 
                        @new$PROT_PWD, 
                        @new$PROT_TIPI_AMMESSI, 
                        @new$PROT_LOAD_ALLEGATI, 
                        @new$PROT_CODE)

                  FETCH ForEachInsertedRowTriggerCursor
                      INTO                       
                        @new$ID_SOTTOTITOLO, 
                        @new$REF_ID_TITOLO, 
                        @new$SOTTOTITOLO, 
                        @new$ACTIVE, 
                        @new$NOTE, 
                        @new$COM_CODE, 
                        @new$PROT_MANAGED, 
                        @new$PROT_SUBCODE, 
                        @new$PROT_PWD, 
                        @new$PROT_TIPI_AMMESSI, 
                        @new$PROT_LOAD_ALLEGATI, 
                        @new$PROT_CODE

               END

            CLOSE ForEachInsertedRowTriggerCursor

            DEALLOCATE ForEachInsertedRowTriggerCursor

         END

GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_SOTTOTITOLI.COMUN_SOTTOTITOLI_INDEX1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_SOTTOTITOLI', @level2type = N'INDEX', @level2name = N'COMUN_SOTTOTITOLI_INDEX1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_SOTTOTITOLI.COMUNICAZIONI_SOTTOTITOL_CHK1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_SOTTOTITOLI', @level2type = N'CONSTRAINT', @level2name = N'COMUNICAZIONI_SOTTOTITOL_CHK1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_SOTTOTITOLI.COMUNICAZIONI_SOTTOTITOL_CHK2', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_SOTTOTITOLI', @level2type = N'CONSTRAINT', @level2name = N'COMUNICAZIONI_SOTTOTITOL_CHK2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_SOTTOTITOLI.COMUNICAZIONI_SOTTOTITOL_CHK3', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_SOTTOTITOLI', @level2type = N'CONSTRAINT', @level2name = N'COMUNICAZIONI_SOTTOTITOL_CHK3';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_SOTTOTITOLI', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_SOTTOTITOLI';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_SOTTOTITOLI.ID_SOTTOTITOLO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_SOTTOTITOLI', @level2type = N'COLUMN', @level2name = N'ID_SOTTOTITOLO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_SOTTOTITOLI.REF_ID_TITOLO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_SOTTOTITOLI', @level2type = N'COLUMN', @level2name = N'REF_ID_TITOLO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_SOTTOTITOLI.SOTTOTITOLO', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_SOTTOTITOLI', @level2type = N'COLUMN', @level2name = N'SOTTOTITOLO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_SOTTOTITOLI.ACTIVE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_SOTTOTITOLI', @level2type = N'COLUMN', @level2name = N'ACTIVE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_SOTTOTITOLI.NOTE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_SOTTOTITOLI', @level2type = N'COLUMN', @level2name = N'NOTE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_SOTTOTITOLI.COM_CODE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_SOTTOTITOLI', @level2type = N'COLUMN', @level2name = N'COM_CODE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_SOTTOTITOLI.PROT_MANAGED', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_SOTTOTITOLI', @level2type = N'COLUMN', @level2name = N'PROT_MANAGED';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_SOTTOTITOLI.PROT_SUBCODE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_SOTTOTITOLI', @level2type = N'COLUMN', @level2name = N'PROT_SUBCODE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_SOTTOTITOLI.PROT_PWD', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_SOTTOTITOLI', @level2type = N'COLUMN', @level2name = N'PROT_PWD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_SOTTOTITOLI.PROT_TIPI_AMMESSI', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_SOTTOTITOLI', @level2type = N'COLUMN', @level2name = N'PROT_TIPI_AMMESSI';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_SOTTOTITOLI.PROT_LOAD_ALLEGATI', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_SOTTOTITOLI', @level2type = N'COLUMN', @level2name = N'PROT_LOAD_ALLEGATI';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_SOTTOTITOLI.PROT_CODE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_SOTTOTITOLI', @level2type = N'COLUMN', @level2name = N'PROT_CODE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.COMUNICAZIONI_SOTTOTITOLI.COMUNICAZIONI_SOTTOTITOLI_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'COMUNICAZIONI_SOTTOTITOLI', @level2type = N'CONSTRAINT', @level2name = N'COMUNICAZIONI_SOTTOTITOLI_PK';

