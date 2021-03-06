﻿CREATE TABLE [FAXPEC].[LOG_ACTIONS] (
    [ID]          NUMERIC (10)  NOT NULL,
    [LOG_UID]     VARCHAR (40)  NOT NULL,
    [APP_CODE]    VARCHAR (20)  NULL,
    [LOG_CODE]    VARCHAR (20)  NULL,
    [USER_ID]     VARCHAR (255) NULL,
    [USER_MAIL]   VARCHAR (255) NULL,
    [LOG_DETAILS] VARCHAR (MAX) NULL,
    [LOG_DATE]    DATETIME2 (6) NULL,
    [LOG_LEVEL]   VARCHAR (10)  NULL,
    [OBJECT_ID]   VARCHAR (50)  NULL,
    CONSTRAINT [LOG_ACTIONS_PK] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO

CREATE TRIGGER [FAXPEC].[InsteadOfInsertOn$LOG_ACTIONS]
   ON [FAXPEC].[LOG_ACTIONS]
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
               @new$ID numeric(10, 0), 
               @new$LOG_UID varchar(40), 
               @new$APP_CODE varchar(20), 
               @new$LOG_CODE varchar(20), 
               @new$USER_ID varchar(255), 
               @new$USER_MAIL varchar(255), 
               @new$LOG_DETAILS varchar(max), 
               @new$LOG_DATE datetime2(6), 
               @new$LOG_LEVEL varchar(10), 
               @new$OBJECT_ID varchar(50)

            DECLARE
                ForEachInsertedRowTriggerCursor CURSOR LOCAL FORWARD_ONLY READ_ONLY FOR 
                  SELECT                   
                     ID, 
                     LOG_UID, 
                     APP_CODE, 
                     LOG_CODE, 
                     USER_ID, 
                     USER_MAIL, 
                     LOG_DETAILS, 
                     LOG_DATE, 
                     LOG_LEVEL, 
                     OBJECT_ID
                  FROM inserted

            OPEN ForEachInsertedRowTriggerCursor

            FETCH ForEachInsertedRowTriggerCursor
                INTO                 
                  @new$ID, 
                  @new$LOG_UID, 
                  @new$APP_CODE, 
                  @new$LOG_CODE, 
                  @new$USER_ID, 
                  @new$USER_MAIL, 
                  @new$LOG_DETAILS, 
                  @new$LOG_DATE, 
                  @new$LOG_LEVEL, 
                  @new$OBJECT_ID

            WHILE @@fetch_status = 0
            
               BEGIN

                  /* row-level triggers implementation: begin*/
                  BEGIN
                     BEGIN
                        IF @triggerType = 'I'
                           BEGIN
                              IF @new$ID IS NULL
                                 SELECT @new$ID = NEXT VALUE FOR FAXPEC.LOG_ACTIONS_SEQ
                           END
                     END
                  END
                  /* row-level triggers implementation: end*/

                  /* DML-operation emulation*/
                  INSERT FAXPEC.LOG_ACTIONS(                   
                     ID, 
                     LOG_UID, 
                     APP_CODE, 
                     LOG_CODE, 
                     USER_ID, 
                     USER_MAIL, 
                     LOG_DETAILS, 
                     LOG_DATE, 
                     LOG_LEVEL, 
                     OBJECT_ID)
                     VALUES (                       
                        @new$ID, 
                        @new$LOG_UID, 
                        @new$APP_CODE, 
                        @new$LOG_CODE, 
                        @new$USER_ID, 
                        @new$USER_MAIL, 
                        @new$LOG_DETAILS, 
                        @new$LOG_DATE, 
                        @new$LOG_LEVEL, 
                        @new$OBJECT_ID)

                  FETCH ForEachInsertedRowTriggerCursor
                      INTO                      
                        @new$ID, 
                        @new$LOG_UID, 
                        @new$APP_CODE, 
                        @new$LOG_CODE, 
                        @new$USER_ID, 
                        @new$USER_MAIL, 
                        @new$LOG_DETAILS, 
                        @new$LOG_DATE, 
                        @new$LOG_LEVEL, 
                        @new$OBJECT_ID

               END

            CLOSE ForEachInsertedRowTriggerCursor

            DEALLOCATE ForEachInsertedRowTriggerCursor

         END

GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_ACTIONS', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_ACTIONS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_ACTIONS.ID', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_ACTIONS', @level2type = N'COLUMN', @level2name = N'ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_ACTIONS.LOG_UID', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_ACTIONS', @level2type = N'COLUMN', @level2name = N'LOG_UID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_ACTIONS.APP_CODE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_ACTIONS', @level2type = N'COLUMN', @level2name = N'APP_CODE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_ACTIONS.LOG_CODE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_ACTIONS', @level2type = N'COLUMN', @level2name = N'LOG_CODE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_ACTIONS.USER_ID', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_ACTIONS', @level2type = N'COLUMN', @level2name = N'USER_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_ACTIONS.USER_MAIL', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_ACTIONS', @level2type = N'COLUMN', @level2name = N'USER_MAIL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_ACTIONS.LOG_DETAILS', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_ACTIONS', @level2type = N'COLUMN', @level2name = N'LOG_DETAILS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_ACTIONS.LOG_DATE', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_ACTIONS', @level2type = N'COLUMN', @level2name = N'LOG_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_ACTIONS.LOG_LEVEL', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_ACTIONS', @level2type = N'COLUMN', @level2name = N'LOG_LEVEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_ACTIONS.OBJECT_ID', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_ACTIONS', @level2type = N'COLUMN', @level2name = N'OBJECT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.LOG_ACTIONS.LOG_ACTIONS_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'LOG_ACTIONS', @level2type = N'CONSTRAINT', @level2name = N'LOG_ACTIONS_PK';

