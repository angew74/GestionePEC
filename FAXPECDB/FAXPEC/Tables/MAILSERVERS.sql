﻿CREATE TABLE [FAXPEC].[MAILSERVERS] (
    [ID_SVR]        FLOAT (53)    NOT NULL,
    [NOME]          VARCHAR (50)  NULL,
    [INDIRIZZO_IN]  VARCHAR (100) NULL,
    [PROTOCOLLO_IN] VARCHAR (5)   NULL,
    [PORTA_IN]      VARCHAR (3)   NULL,
    [SSL_IN]        VARCHAR (5)   NULL,
    [INDIRIZZO_OUT] VARCHAR (100) NULL,
    [PORTA_OUT]     VARCHAR (3)   NULL,
    [SSL_OUT]       VARCHAR (5)   NULL,
    [AUTH_OUT]      VARCHAR (20)  NULL,
    [DOMINUS]       VARCHAR (250) NULL,
    [FLG_ISPEC]     VARCHAR (1)   DEFAULT ((0)) NOT NULL,
    CONSTRAINT [MAILSERVERS_PK] PRIMARY KEY CLUSTERED ([ID_SVR] ASC)
);


GO

CREATE TRIGGER [FAXPEC].[InsteadOfInsertOn$MAILSERVERS]
   ON [FAXPEC].[MAILSERVERS]
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

               @new$ID_SVR float(53), 
               @new$NOME varchar(50), 
               @new$INDIRIZZO_IN varchar(100), 
               @new$PROTOCOLLO_IN varchar(5), 
               @new$PORTA_IN varchar(3), 
               @new$SSL_IN varchar(5), 
               @new$INDIRIZZO_OUT varchar(100), 
               @new$PORTA_OUT varchar(3), 
               @new$SSL_OUT varchar(5), 
               @new$AUTH_OUT varchar(20), 
               @new$DOMINUS varchar(250), 
               @new$FLG_ISPEC varchar(1)

            DECLARE
                ForEachInsertedRowTriggerCursor CURSOR LOCAL FORWARD_ONLY READ_ONLY FOR 
                  SELECT                   
                     ID_SVR, 
                     NOME, 
                     INDIRIZZO_IN, 
                     PROTOCOLLO_IN, 
                     PORTA_IN, 
                     SSL_IN, 
                     INDIRIZZO_OUT, 
                     PORTA_OUT, 
                     SSL_OUT, 
                     AUTH_OUT, 
                     DOMINUS, 
                     FLG_ISPEC
                  FROM inserted

            OPEN ForEachInsertedRowTriggerCursor

            FETCH ForEachInsertedRowTriggerCursor
                INTO              
                  @new$ID_SVR, 
                  @new$NOME, 
                  @new$INDIRIZZO_IN, 
                  @new$PROTOCOLLO_IN, 
                  @new$PORTA_IN, 
                  @new$SSL_IN, 
                  @new$INDIRIZZO_OUT, 
                  @new$PORTA_OUT, 
                  @new$SSL_OUT, 
                  @new$AUTH_OUT, 
                  @new$DOMINUS, 
                  @new$FLG_ISPEC

            WHILE @@fetch_status = 0
            
               BEGIN

                  /* row-level triggers implementation: begin*/
                  BEGIN
                     BEGIN
                        IF @triggerType = 'I'
                           BEGIN                          
                                 SELECT @new$ID_SVR = NEXT VALUE FOR FAXPEC.MAIL_SERVER_SEQ
                           END
                     END
                  END
                  /* row-level triggers implementation: end*/

                  /* DML-operation emulation*/
                  INSERT FAXPEC.MAILSERVERS(                   
                     ID_SVR, 
                     NOME, 
                     INDIRIZZO_IN, 
                     PROTOCOLLO_IN, 
                     PORTA_IN, 
                     SSL_IN, 
                     INDIRIZZO_OUT, 
                     PORTA_OUT, 
                     SSL_OUT, 
                     AUTH_OUT, 
                     DOMINUS, 
                     FLG_ISPEC)
                     VALUES (                     
                        @new$ID_SVR, 
                        @new$NOME, 
                        @new$INDIRIZZO_IN, 
                        @new$PROTOCOLLO_IN, 
                        @new$PORTA_IN, 
                        @new$SSL_IN, 
                        @new$INDIRIZZO_OUT, 
                        @new$PORTA_OUT, 
                        @new$SSL_OUT, 
                        @new$AUTH_OUT, 
                        @new$DOMINUS, 
                        @new$FLG_ISPEC)

                  FETCH ForEachInsertedRowTriggerCursor
                      INTO                       
                        @new$ID_SVR, 
                        @new$NOME, 
                        @new$INDIRIZZO_IN, 
                        @new$PROTOCOLLO_IN, 
                        @new$PORTA_IN, 
                        @new$SSL_IN, 
                        @new$INDIRIZZO_OUT, 
                        @new$PORTA_OUT, 
                        @new$SSL_OUT, 
                        @new$AUTH_OUT, 
                        @new$DOMINUS, 
                        @new$FLG_ISPEC

               END

            CLOSE ForEachInsertedRowTriggerCursor

            DEALLOCATE ForEachInsertedRowTriggerCursor

         END

GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAILSERVERS', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAILSERVERS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAILSERVERS.ID_SVR', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAILSERVERS', @level2type = N'COLUMN', @level2name = N'ID_SVR';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAILSERVERS.NOME', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAILSERVERS', @level2type = N'COLUMN', @level2name = N'NOME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAILSERVERS.INDIRIZZO_IN', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAILSERVERS', @level2type = N'COLUMN', @level2name = N'INDIRIZZO_IN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAILSERVERS.PROTOCOLLO_IN', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAILSERVERS', @level2type = N'COLUMN', @level2name = N'PROTOCOLLO_IN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAILSERVERS.PORTA_IN', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAILSERVERS', @level2type = N'COLUMN', @level2name = N'PORTA_IN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAILSERVERS.SSL_IN', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAILSERVERS', @level2type = N'COLUMN', @level2name = N'SSL_IN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAILSERVERS.INDIRIZZO_OUT', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAILSERVERS', @level2type = N'COLUMN', @level2name = N'INDIRIZZO_OUT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAILSERVERS.PORTA_OUT', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAILSERVERS', @level2type = N'COLUMN', @level2name = N'PORTA_OUT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAILSERVERS.SSL_OUT', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAILSERVERS', @level2type = N'COLUMN', @level2name = N'SSL_OUT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAILSERVERS.AUTH_OUT', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAILSERVERS', @level2type = N'COLUMN', @level2name = N'AUTH_OUT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAILSERVERS.DOMINUS', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAILSERVERS', @level2type = N'COLUMN', @level2name = N'DOMINUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAILSERVERS.FLG_ISPEC', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAILSERVERS', @level2type = N'COLUMN', @level2name = N'FLG_ISPEC';


GO
EXECUTE sp_addextendedproperty @name = N'MS_SSMA_SOURCE', @value = N'FAXPEC.MAILSERVERS.MAILSERVERS_PK', @level0type = N'SCHEMA', @level0name = N'FAXPEC', @level1type = N'TABLE', @level1name = N'MAILSERVERS', @level2type = N'CONSTRAINT', @level2name = N'MAILSERVERS_PK';

