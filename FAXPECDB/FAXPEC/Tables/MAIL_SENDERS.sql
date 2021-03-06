﻿CREATE TABLE [FAXPEC].[MAIL_SENDERS] (
    [ID_SENDER]       NUMERIC (10)  NOT NULL,
    [MAIL]            VARCHAR (100) NOT NULL,
    [ID_MAILSERVER]   NUMERIC (10)  NOT NULL,
    [USERNAME]        VARCHAR (100) NOT NULL,
    [PASSWORD]        VARCHAR (50)  NOT NULL,
    [ID_RESPONSABILE] NUMERIC (10)  NULL,
    [FLG_MANAGED]     VARCHAR (1)   DEFAULT (NULL) NULL,
    CONSTRAINT [MAIL_SENDERS_PK] PRIMARY KEY CLUSTERED ([ID_SENDER] ASC)
);


GO
CREATE NONCLUSTERED INDEX [MAIL_SENDERS_INDEX1]
    ON [FAXPEC].[MAIL_SENDERS]([MAIL] ASC);


GO
ALTER INDEX [MAIL_SENDERS_INDEX1]
    ON [FAXPEC].[MAIL_SENDERS] DISABLE;


GO

CREATE TRIGGER [FAXPEC].[InsteadOfInsertOn$MAIL_SENDERS]
   ON [FAXPEC].[MAIL_SENDERS]
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
               @new$ID_SENDER numeric(10, 0), 
               @new$MAIL varchar(100), 
               @new$ID_MAILSERVER numeric(10, 0), 
               @new$USERNAME varchar(100), 
               @new$PASSWORD varchar(50), 
               @new$ID_RESPONSABILE numeric(10, 0), 
               @new$FLG_MANAGED varchar(1)

            DECLARE
                ForEachInsertedRowTriggerCursor CURSOR LOCAL FORWARD_ONLY READ_ONLY FOR 
                  SELECT                     
                     ID_SENDER, 
                     MAIL, 
                     ID_MAILSERVER, 
                     USERNAME, 
                     PASSWORD, 
                     ID_RESPONSABILE, 
                     FLG_MANAGED
                  FROM inserted

            OPEN ForEachInsertedRowTriggerCursor

            FETCH ForEachInsertedRowTriggerCursor
                INTO                 
                  @new$ID_SENDER, 
                  @new$MAIL, 
                  @new$ID_MAILSERVER, 
                  @new$USERNAME, 
                  @new$PASSWORD, 
                  @new$ID_RESPONSABILE, 
                  @new$FLG_MANAGED

            WHILE @@fetch_status = 0
            
               BEGIN

                  /* row-level triggers implementation: begin*/
                  BEGIN
                     BEGIN
                        IF @triggerType = 'I'
                           BEGIN                            
                                 SELECT @new$ID_SENDER = NEXT VALUE FOR FAXPEC.MAIL_SENDERS_SEQ
                           END
                     END
                  END
                  /* row-level triggers implementation: end*/

                  /* DML-operation emulation*/
                  INSERT FAXPEC.MAIL_SENDERS(                    
                     ID_SENDER, 
                     MAIL, 
                     ID_MAILSERVER, 
                     USERNAME, 
                     PASSWORD, 
                     ID_RESPONSABILE, 
                     FLG_MANAGED)
                     VALUES (                      
                        @new$ID_SENDER, 
                        @new$MAIL, 
                        @new$ID_MAILSERVER, 
                        @new$USERNAME, 
                        @new$PASSWORD, 
                        @new$ID_RESPONSABILE, 
                        @new$FLG_MANAGED)

                  FETCH ForEachInsertedRowTriggerCursor
                      INTO                        
                        @new$ID_SENDER, 
                        @new$MAIL, 
                        @new$ID_MAILSERVER, 
                        @new$USERNAME, 
                        @new$PASSWORD, 
                        @new$ID_RESPONSABILE, 
                        @new$FLG_MANAGED

               END

            CLOSE ForEachInsertedRowTriggerCursor

            DEALLOCATE ForEachInsertedRowTriggerCursor

         END

GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_SENDERS.MAIL_SENDERS_INDEX1', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_SENDERS', @level2type = N'INDEX', @level2name = N'MAIL_SENDERS_INDEX1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_SENDERS', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_SENDERS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_SENDERS.ID_SENDER', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_SENDERS', @level2type = N'COLUMN', @level2name = N'ID_SENDER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_SENDERS.MAIL', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_SENDERS', @level2type = N'COLUMN', @level2name = N'MAIL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_SENDERS.ID_MAILSERVER', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_SENDERS', @level2type = N'COLUMN', @level2name = N'ID_MAILSERVER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_SENDERS.USERNAME', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_SENDERS', @level2type = N'COLUMN', @level2name = N'USERNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_SENDERS.PASSWORD', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_SENDERS', @level2type = N'COLUMN', @level2name = N'PASSWORD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_SENDERS.ID_RESPONSABILE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_SENDERS', @level2type = N'COLUMN', @level2name = N'ID_RESPONSABILE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_SENDERS.FLG_MANAGED', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_SENDERS', @level2type = N'COLUMN', @level2name = N'FLG_MANAGED';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAIL_SENDERS.MAIL_SENDERS_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAIL_SENDERS', @level2type = N'CONSTRAINT', @level2name = N'MAIL_SENDERS_PK';

