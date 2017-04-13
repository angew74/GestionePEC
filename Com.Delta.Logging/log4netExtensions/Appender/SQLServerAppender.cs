
#if !SSCLI

using log4net.Appender;
using log4net.Core;
using log4net.Util;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using log4net.Layout;

namespace log4netExtensions.Appender
{
    public class SQLServerAppender : BufferingAppenderSkeleton
    {
        public SQLServerAppender()
        {
            //m_useTransactions = false;
            m_commandType = System.Data.CommandType.Text;
            m_parameters = new ArrayList();
            //m_reconnectOnError = false;
        }
        #region Public Instance Properties

        /// <summary>
        /// Gets or sets the database connection string that is used to connect to 
        /// the database.
        /// </summary>
        /// <value>
        /// The database connection string used to connect to the database.
        /// </value>
        /// <remarks>
        /// <para>
        /// The connections string is specific to the connection type.
        /// See ConnectionType for more information.
        /// </para>
        /// </remarks>
        /// <example>Connection string for MS Access via ODBC:
        /// <code>"DSN=MS Access Database;UID=admin;PWD=;SystemDB=C:\data\System.mdw;SafeTransactions = 0;FIL=MS Access;DriverID = 25;DBQ=C:\data\train33.mdb"</code>
        /// </example>
        /// <example>Another connection string for MS Access via ODBC:
        /// <code>"Driver={Microsoft Access Driver (*.mdb)};DBQ=C:\Work\cvs_root\log4net-1.2\access.mdb;UID=;PWD=;"</code>
        /// </example>
        /// <example>Connection string for MS Access via OLE DB:
        /// <code>"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Work\cvs_root\log4net-1.2\access.mdb;User Id=;Password=;"</code>
        /// </example>
        public string ConnectionString
        {
            get { return m_connectionString; }
            set { m_connectionString = value; }
        }

        /// <summary>
        /// Gets or sets the command text that is used to insert logging events
        /// into the database.
        /// </summary>
        /// <value>
        /// The command text used to insert logging events into the database.
        /// </value>
        /// <remarks>
        /// <para>
        /// Either the text of the prepared statement or the
        /// name of the stored procedure to execute to write into
        /// the database.
        /// </para>
        /// <para>
        /// The <see cref="CommandType"/> property determines if
        /// this text is a prepared statement or a stored procedure.
        /// </para>
        /// </remarks>
        public string CommandText
        {
            get { return m_commandText; }
            set { m_commandText = value; }
        }

        /// <summary>
        /// Gets or sets the command type to execute.
        /// </summary>
        /// <value>
        /// The command type to execute.
        /// </value>
        /// <remarks>
        /// <para>
        /// This value may be either <see cref="System.Data.CommandType.Text"/> (<c>System.Data.CommandType.Text</c>) to specify
        /// that the <see cref="CommandText"/> is a prepared statement to execute, 
        /// or <see cref="System.Data.CommandType.StoredProcedure"/> (<c>System.Data.CommandType.StoredProcedure</c>) to specify that the
        /// <see cref="CommandText"/> property is the name of a stored procedure
        /// to execute.
        /// </para>
        /// <para>
        /// The default value is <see cref="System.Data.CommandType.Text"/> (<c>System.Data.CommandType.Text</c>).
        /// </para>
        /// </remarks>
        public CommandType CommandType
        {
            get { return m_commandType; }
            set { m_commandType = value; }
        }

        /// <summary>
        /// Should transactions be used to insert logging events in the database.
        /// </summary>
        /// <value>
        /// <c>true</c> if transactions should be used to insert logging events in
        /// the database, otherwise <c>false</c>. The default value is <c>true</c>.
        /// </value>
        /// <remarks>
        /// <para>
        /// Gets or sets a value that indicates whether transactions should be used
        /// to insert logging events in the database.
        /// </para>
        /// <para>
        /// When set a single transaction will be used to insert the buffered events
        /// into the database. Otherwise each event will be inserted without using
        /// an explicit transaction.
        /// </para>
        /// </remarks>
        public bool UseTransactions
        {
            get { return m_useTransactions; }
            set { m_useTransactions = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="SecurityContext"/> used to call the NetSend method.
        /// </summary>
        /// <value>
        /// The <see cref="SecurityContext"/> used to call the NetSend method.
        /// </value>
        /// <remarks>
        /// <para>
        /// Unless a <see cref="SecurityContext"/> specified here for this appender
        /// the <see cref="SecurityContextProvider.DefaultProvider"/> is queried for the
        /// security context to use. The default behavior is to use the security context
        /// of the current thread.
        /// </para>
        /// </remarks>
        public SecurityContext SecurityContext
        {
            get { return m_securityContext; }
            set { m_securityContext = value; }
        }

        /// <summary>
        /// Should this appender try to reconnect to the database on error.
        /// </summary>
        /// <value>
        /// <c>true</c> if the appender should try to reconnect to the database after an
        /// error has occurred, otherwise <c>false</c>. The default value is <c>false</c>, 
        /// i.e. not to try to reconnect.
        /// </value>
        /// <remarks>
        /// <para>
        /// The default behaviour is for the appender not to try to reconnect to the
        /// database if an error occurs. Subsequent logging events are discarded.
        /// </para>
        /// <para>
        /// To force the appender to attempt to reconnect to the database set this
        /// property to <c>true</c>.
        /// </para>
        /// <note>
        /// When the appender attempts to connect to the database there may be a
        /// delay of up to the connection timeout specified in the connection string.
        /// If the appender is being used synchronously (the default behaviour for
        /// this appender) then this delay will impact the calling application on
        /// the current thread. Until the connection can be reestablished this
        /// potential delay may occur multiple times.
        /// </note>
        /// </remarks>
        public bool ReconnectOnError
        {
            get { return m_reconnectOnError; }
            set { m_reconnectOnError = value; }
        }

        #endregion // Public Instance Properties

        #region Protected Instance Properties

        /// <summary>
        /// Gets or sets the underlying <see cref="IDbConnection" />.
        /// </summary>
        /// <value>
        /// The underlying <see cref="IDbConnection" />.
        /// </value>
        /// <remarks>
        /// <see cref="SQLServerAppender" /> creates a <see cref="IDbConnection" /> to insert 
        /// logging events into a database.  Classes deriving from <see cref="SQLServerAppender" /> 
        /// can use this property to get or set this <see cref="IDbConnection" />.  Use the 
        /// underlying <see cref="IDbConnection" /> returned from <see cref="Connection" /> if 
        /// you require access beyond that which <see cref="SQLServerAppender" /> provides.
        /// </remarks>
        protected SqlConnection Connection
        {
            get { return this.m_dbConnection; }
            set { this.m_dbConnection = value; }
        }

        #endregion // Protected Instance Properties

        #region Implementation of IOptionHandler

        /// <summary>
        /// Initialize the appender based on the options set
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is part of the <see cref="IOptionHandler"/> delayed object
        /// activation scheme. The <see cref="ActivateOptions"/> method must 
        /// be called on this object after the configuration properties have
        /// been set. Until <see cref="ActivateOptions"/> is called this
        /// object is in an undefined state and must not be used. 
        /// </para>
        /// <para>
        /// If any of the configuration properties are modified then 
        /// <see cref="ActivateOptions"/> must be called again.
        /// </para>
        /// </remarks>
        override public void ActivateOptions()
        {
            base.ActivateOptions();

            // Are we using a command object
            m_usePreparedCommand = (m_commandText != null && m_commandText.Length > 0);

            if (m_securityContext == null)
            {
                m_securityContext = SecurityContextProvider.DefaultProvider.CreateSecurityContext(this);
            }

            InitializeDatabaseConnection();
            InitializeDatabaseCommand();
        }

        #endregion

        #region Override implementation of AppenderSkeleton

        /// <summary>
        /// Override the parent method to close the database
        /// </summary>
        /// <remarks>
        /// <para>
        /// Closes the database command and database connection.
        /// </para>
        /// </remarks>
        override protected void OnClose()
        {
            base.OnClose();
            if (m_dbCommand != null)
            {
                m_dbCommand.Dispose();
                m_dbCommand = null;
            }
            if (m_dbConnection != null)
            {
                m_dbConnection.Close();
                m_dbConnection = null;
            }
        }

        #endregion

        #region Override implementation of BufferingAppenderSkeleton

        /// <summary>
        /// Inserts the events into the database.
        /// </summary>
        /// <param name="events">The events to insert into the database.</param>
        /// <remarks>
        /// <para>
        /// Insert all the events specified in the <paramref name="events"/>
        /// array into the database.
        /// </para>
        /// </remarks>
        override protected void SendBuffer(LoggingEvent[] events)
        {
            LogLog.Debug(typeof(SQLServerAppender), "WRITE_LOG_TO_DB_START");
            if (m_reconnectOnError && (m_dbConnection == null || m_dbConnection.State != ConnectionState.Open))
            {
                LogLog.Debug(typeof(SQLServerAppender), "SQLSERVERAppender: Attempting to reconnect to database. Current Connection State: " + ((m_dbConnection == null) ? "<null>" : m_dbConnection.State.ToString()));
                //LogLog.Debug("SQLServerAppender: Attempting to reconnect to database. Current Connection State: " + ((m_dbConnection == null) ? "<null>" : m_dbConnection.State.ToString()));

                InitializeDatabaseConnection();
                InitializeDatabaseCommand();
            }

            // Check that the connection exists and is open
            if (m_dbConnection != null && m_dbConnection.State == ConnectionState.Open)
            {
                if (m_useTransactions)
                {
                    // Create transaction
                    // NJC - Do this on 2 lines because it can confuse the debugger
                    LogLog.Debug(typeof(SQLServerAppender), "WRITE_LOG_TO_DB_START_TRANSACTION");
                    SqlTransaction dbTran = null;
                    try
                    {
                        dbTran = m_dbConnection.BeginTransaction();

                        SendBuffer(dbTran, events);

                        // commit transaction
                        dbTran.Commit();
                        LogLog.Debug(typeof(SQLServerAppender), "WRITE_LOG_TO_DB_END_TRANSACTION");
                    }
                    catch (Exception ex)
                    {
                        LogLog.Debug(typeof(SQLServerAppender), "WRITE_LOG_TO_DB_TRANSACTION_ERROR:" + ex.Message);
                        // rollback the transaction
                        if (dbTran != null)
                        {
                            try
                            {
                                dbTran.Rollback();
                            }
                            catch (Exception)
                            {
                                // Ignore exception
                            }
                        }

                        // Can't insert into the database. That's a bad thing
                        ErrorHandler.Error("Exception while writing to database", ex);
                    }
                }
                else
                {
                    /*
                     * try catch aggiunto per risolvere problema 
                     * mancata riconnessione a sqlserver
                     * in caso di blocco comunicazione client/server
                     * (il pool di connessioni è invalido 
                     * ma con lo state che rimane segnato a open)
                     * nota:
                     * m_reconnectOnError deve essere configurato a true
                    */
                    try
                    {
                        // Send without transaction
                        LogLog.Debug(typeof(SQLServerAppender), "WRITE_LOG_TO_DB_NO_TRANSACTION");
                        SendBuffer(null, events);
                        LogLog.Debug(typeof(SQLServerAppender), "WRITE_LOG_TO_DB_NO_TRANSACTION_DONE");
                    }
                    catch (SqlException exx)
                    {
                        LogLog.Debug(typeof(SQLServerAppender), "WRITE_LOG_TO_DB_NO_TRANSACTION_ERROR:" + exx.Message);
                        m_dbConnection = null;
                    }
                    catch (Exception exx1)
                    {
                        LogLog.Debug(typeof(SQLServerAppender), "WRITE_LOG_TO_DB_NO_TRANSACTION_ERROR:" + exx1.Message);
                        m_dbConnection = null;
                        throw;
                    }
                }
            }
        }

        #endregion // Override implementation of BufferingAppenderSkeleton

        #region Public Instance Methods

        /// <summary>
        /// Adds a parameter to the command.
        /// </summary>
        /// <param name="parameter">The parameter to add to the command.</param>
        /// <remarks>
        /// <para>
        /// Adds a parameter to the ordered list of command parameters.
        /// </para>
        /// </remarks>
        public void AddParameter(SQLServerAppenderParameter parameter)
        {
            m_parameters.Add(parameter);
        }


        #endregion // Public Instance Methods

        #region Protected Instance Methods

        /// <summary>
        /// Writes the events to the database using the transaction specified.
        /// </summary>
        /// <param name="dbTran">The transaction that the events will be executed under.</param>
        /// <param name="events">The array of events to insert into the database.</param>
        /// <remarks>
        /// <para>
        /// The transaction argument can be <c>null</c> if the appender has been
        /// configured not to use transactions. See <see cref="UseTransactions"/>
        /// property for more information.
        /// </para>
        /// </remarks>
        virtual protected void SendBuffer(SqlTransaction dbTran, LoggingEvent[] events)
        {
            if (m_usePreparedCommand)
            {
                // Send buffer using the prepared command object

                if (m_dbCommand != null)
                {
                    LogLog.Debug(typeof(SQLServerAppender), "SQLSERVER_COMMAND=" + m_dbCommand.CommandText);
                    ArrayList paramArr = null;
                    foreach (SQLServerAppenderParameter param in m_parameters)
                    {
                        paramArr = new ArrayList();
                        SqlParameter sqlParam = (SqlParameter)m_dbCommand.Parameters[param.ParameterName];
                        foreach (LoggingEvent e in events)
                        {
                            object value = param.Layout.Format(e);
                            if (value.ToString() == "(null)")
                            {
                                value = System.DBNull.Value;
                            }
                            paramArr.Add(value);

                        }
                        sqlParam.Value = paramArr.ToArray();
                    }
                   // m_dbCommand..ArrayBindCount = events.Length;
                    int i = m_dbCommand.ExecuteNonQuery();
                    LogLog.Debug(typeof(SQLServerAppender), "SQLSERVER_COMMAND_OUTPUT=" + i);

                }
            }
            else
            {
                LogLog.Debug(typeof(SQLServerAppender), "ERROR_NOT_IMPLEMENTED_EXCEPTION");
                throw new NotImplementedException();

            }

        }

        /// <summary>
        /// Formats the log message into database statement text.
        /// </summary>
        /// <param name="logEvent">The event being logged.</param>
        /// <remarks>
        /// This method can be overridden by subclasses to provide 
        /// more control over the format of the database statement.
        /// </remarks>
        /// <returns>
        /// Text that can be passed to a <see cref="System.Data.IDbCommand"/>.
        /// </returns>
        virtual protected string GetLogStatement(LoggingEvent logEvent)
        {
            if (Layout == null)
            {
                ErrorHandler.Error("ADOAppender: No Layout specified.");
                return "";
            }
            else
            {
                StringWriter writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
                Layout.Format(writer, logEvent);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Connects to the database.
        /// </summary>		
        private void InitializeDatabaseConnection()
        {
            try
            {
                // Create the connection object
                m_dbConnection = new SqlConnection();

                // Set the connection string
                m_dbConnection.ConnectionString = m_connectionString;

                using (SecurityContext.Impersonate(this))
                {
                    // Open the database connection
                    m_dbConnection.Open();
                }
            }
            catch (System.Exception e)
            {
                // Sadly, your connection string is bad.
                ErrorHandler.Error("Could not open database connection [" + m_connectionString + "]", e);

                m_dbConnection = null;
            }
        }

        /// <summary>
        /// Retrieves the class type of the ADO.NET provider.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Gets the Type of the ADO.NET provider to use to connect to the
        /// database. This method resolves the type specified in the 
        /// ConnectionType property.
        /// </para>
        /// <para>
        /// Subclasses can override this method to return a different type
        /// if necessary.
        /// </para>
        /// </remarks>
        /// <returns>The <see cref="Type"/> of the ADO.NET provider</returns>
        virtual protected Type ResolveConnectionType()
        {
            try
            {
                return SystemInfo.GetTypeFromString(m_connectionType, true, false);
            }
            catch (Exception ex)
            {
                ErrorHandler.Error("Failed to load connection type [" + m_connectionType + "]", ex);
                throw;
            }
        }

        /// <summary>
        /// Prepares the database command and initialize the parameters.
        /// </summary>
        private void InitializeDatabaseCommand()
        {
            if (m_dbConnection != null && m_usePreparedCommand)
            {
                try
                {
                    // Create the command object
                    m_dbCommand = m_dbConnection.CreateCommand();

                    // Set the command string
                    m_dbCommand.CommandText = m_commandText;

                    // Set the command type
                    m_dbCommand.CommandType = m_commandType;
                }
                catch (System.Exception e)
                {
                    ErrorHandler.Error("Could not create database command [" + m_commandText + "]", e);

                    if (m_dbCommand != null)
                    {
                        try
                        {
                            m_dbCommand.Dispose();
                        }
                        catch
                        {
                            // Ignore exception
                        }
                        m_dbCommand = null;
                    }
                }

                if (m_dbCommand != null)
                {
                    try
                    {
                        foreach (SQLServerAppenderParameter param in m_parameters)
                        {
                            try
                            {
                                param.Prepare(m_dbCommand);
                            }
                            catch (System.Exception e)
                            {
                                ErrorHandler.Error("Could not add database command parameter [" + param.ParameterName + "]", e);
                                throw;
                            }
                        }
                    }
                    catch
                    {
                        try
                        {
                            m_dbCommand.Dispose();
                        }
                        catch
                        {
                            // Ignore exception
                        }
                        m_dbCommand = null;
                    }
                }

                if (m_dbCommand != null)
                {
                    try
                    {
                        // Prepare the command statement.
                        m_dbCommand.Prepare();
                    }
                    catch (System.Exception e)
                    {
                        ErrorHandler.Error("Could not prepare database command [" + m_commandText + "]", e);
                        try
                        {
                            m_dbCommand.Dispose();
                        }
                        catch
                        {
                            // Ignore exception
                        }
                        m_dbCommand = null;
                    }
                }
            }
        }

        #endregion // Protected Instance Methods

        #region Protected Instance Fields

        /// <summary>
        /// Flag to indicate if we are using a command object
        /// </summary>
        /// <remarks>
        /// <para>
        /// Set to <c>true</c> when the appender is to use a prepared
        /// statement or stored procedure to insert into the database.
        /// </para>
        /// </remarks>
        protected bool m_usePreparedCommand;

        /// <summary>
        /// The list of <see cref="SQLServerAppenderParameter"/> objects.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The list of <see cref="SQLServerAppenderParameter"/> objects.
        /// </para>
        /// </remarks>
        protected ArrayList m_parameters;

        #endregion // Protected Instance Fields

        #region Private Instance Fields

        /// <summary>
        /// The security context to use for privileged calls
        /// </summary>
        private SecurityContext m_securityContext;

        /// <summary>
        /// The <see cref="IDbConnection" /> that will be used
        /// to insert logging events into a database.
        /// </summary>
        private SqlConnection m_dbConnection;

        /// <summary>
        /// The database command.
        /// </summary>
        private SqlCommand m_dbCommand;

        /// <summary>
        /// Database connection string.
        /// </summary>
        private string m_connectionString;

        /// <summary>
        /// String type name of the <see cref="IDbConnection"/> type name.
        /// </summary>
        private string m_connectionType = null;

        /// <summary>
        /// The text of the command.
        /// </summary>
        private string m_commandText;

        /// <summary>
        /// The command type.
        /// </summary>
        private CommandType m_commandType;

        /// <summary>
        /// Indicates whether to use transactions when writing to the database.
        /// </summary>
        private bool m_useTransactions;

        /// <summary>
        /// Indicates whether to use transactions when writing to the database.
        /// </summary>
        private bool m_reconnectOnError;

        #endregion // Private Instance Fields

        #region Private Static Fields

        /// <summary>
        /// The fully qualified type of the AdoNetAppender class.
        /// </summary>
        /// <remarks>
        /// Used by the internal logger to record the Type of the
        /// log message.
        /// </remarks>
        private readonly static Type declaringType = typeof(AdoNetAppender);

        #endregion Private Static Fields
    }

    /// <summary>
    /// Parameter type used by the <see cref="SQLServerAppender"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides the basic database parameter properties
    /// as defined by the <see cref="System.Data.IDbDataParameter"/> interface.
    /// </para>
    /// <para>This type can be subclassed to provide database specific
    /// functionality. The two methods that are called externally are
    /// <see cref="Prepare"/> and <see cref="FormatValue"/>.
    /// </para>
    /// </remarks>
    public class SQLServerAppenderParameter
    {
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLServerAppenderParameter" /> class.
        /// </summary>
        /// <remarks>
        /// Default constructor for the SQLServerAppenderParameter class.
        /// </remarks>
        public SQLServerAppenderParameter()
        {
            m_precision = 0;
            m_scale = 0;
            m_size = 0;
        }

        #endregion // Public Instance Constructors

        #region Public Instance Properties

        /// <summary>
        /// Gets or sets the name of this parameter.
        /// </summary>
        /// <value>
        /// The name of this parameter.
        /// </value>
        /// <remarks>
        /// <para>
        /// The name of this parameter. The parameter name
        /// must match up to a named parameter to the SQL stored procedure
        /// or prepared statement.
        /// </para>
        /// </remarks>
        public string ParameterName
        {
            get { return m_parameterName; }
            set { m_parameterName = value; }
        }

        /// <summary>
        /// Gets or sets the database type for this parameter.
        /// </summary>
        /// <value>
        /// The database type for this parameter.
        /// </value>
        /// <remarks>
        /// <para>
        /// The database type for this parameter. This property should
        /// be set to the database type from the <see cref="DbType"/>
        /// enumeration. See <see cref="IDataParameter.DbType"/>.
        /// </para>
        /// <para>
        /// This property is optional. If not specified the ADO.NET provider 
        /// will attempt to infer the type from the value.
        /// </para>
        /// </remarks>
        /// <seealso cref="IDataParameter.DbType" />
        public DbType DbType
        {
            get { return m_dbType; }
            set
            {
                m_dbType = value;
                m_inferType = false;
            }
        }

        /// <summary>
        /// Gets or sets the precision for this parameter.
        /// </summary>
        /// <value>
        /// The precision for this parameter.
        /// </value>
        /// <remarks>
        /// <para>
        /// The maximum number of digits used to represent the Value.
        /// </para>
        /// <para>
        /// This property is optional. If not specified the ADO.NET provider 
        /// will attempt to infer the precision from the value.
        /// </para>
        /// </remarks>
        /// <seealso cref="IDbDataParameter.Precision" />
        public byte Precision
        {
            get { return m_precision; }
            set { m_precision = value; }
        }

        /// <summary>
        /// Gets or sets the scale for this parameter.
        /// </summary>
        /// <value>
        /// The scale for this parameter.
        /// </value>
        /// <remarks>
        /// <para>
        /// The number of decimal places to which Value is resolved.
        /// </para>
        /// <para>
        /// This property is optional. If not specified the ADO.NET provider 
        /// will attempt to infer the scale from the value.
        /// </para>
        /// </remarks>
        /// <seealso cref="IDbDataParameter.Scale" />
        public byte Scale
        {
            get { return m_scale; }
            set { m_scale = value; }
        }

        /// <summary>
        /// Gets or sets the size for this parameter.
        /// </summary>
        /// <value>
        /// The size for this parameter.
        /// </value>
        /// <remarks>
        /// <para>
        /// The maximum size, in bytes, of the data within the column.
        /// </para>
        /// <para>
        /// This property is optional. If not specified the ADO.NET provider 
        /// will attempt to infer the size from the value.
        /// </para>
        /// </remarks>
        /// <seealso cref="IDbDataParameter.Size" />
        public int Size
        {
            get { return m_size; }
            set { m_size = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="IRawLayout"/> to use to 
        /// render the logging event into an object for this 
        /// parameter.
        /// </summary>
        /// <value>
        /// The <see cref="IRawLayout"/> used to render the
        /// logging event into an object for this parameter.
        /// </value>
        /// <remarks>
        /// <para>
        /// The <see cref="IRawLayout"/> that renders the value for this
        /// parameter.
        /// </para>
        /// <para>
        /// The <see cref="RawLayoutConverter"/> can be used to adapt
        /// any <see cref="ILayout"/> into a <see cref="IRawLayout"/>
        /// for use in the property.
        /// </para>
        /// </remarks>
        public IRawLayout Layout
        {
            get { return m_layout; }
            set { m_layout = value; }
        }

        #endregion // Public Instance Properties

        #region Public Instance Methods

        /// <summary>
        /// Prepare the specified database command object.
        /// </summary>
        /// <param name="command">The command to prepare.</param>
        /// <remarks>
        /// <para>
        /// Prepares the database command object by adding
        /// this parameter to its collection of parameters.
        /// </para>
        /// </remarks>
        virtual public void Prepare(SqlCommand command)
        {
            // Create a new parameter
            SqlParameter param = command.CreateParameter();

            // Set the parameter properties
            param.ParameterName = m_parameterName;

            if (!m_inferType)
            {
                param.DbType = m_dbType;
            }
            if (m_precision != 0)
            {
                param.Precision = m_precision;
            }
            if (m_scale != 0)
            {
                param.Scale = m_scale;
            }
            if (m_size != 0)
            {
                param.Size = m_size;
            }

            // Add the parameter to the collection of params
            command.Parameters.Add(param);
        }

        /// <summary>
        /// Renders the logging event and set the parameter value in the command.
        /// </summary>
        /// <param name="command">The command containing the parameter.</param>
        /// <param name="loggingEvent">The event to be rendered.</param>
        /// <remarks>
        /// <para>
        /// Renders the logging event using this parameters layout
        /// object. Sets the value of the parameter on the command object.
        /// </para>
        /// </remarks>
        virtual public void FormatValue(SqlCommand command, LoggingEvent loggingEvent)
        {
            // Lookup the parameter
            SqlParameter param = (SqlParameter)command.Parameters[m_parameterName];

            param.Value = Layout.Format(loggingEvent);
        }

        #endregion // Public Instance Methods

        #region Private Instance Fields

        /// <summary>
        /// The name of this parameter.
        /// </summary>
        private string m_parameterName;

        /// <summary>
        /// The database type for this parameter.
        /// </summary>
        private DbType m_dbType;

        /// <summary>
        /// Flag to infer type rather than use the DbType
        /// </summary>
        private bool m_inferType = true;

        /// <summary>
        /// The precision for this parameter.
        /// </summary>
        private byte m_precision;

        /// <summary>
        /// The scale for this parameter.
        /// </summary>
        private byte m_scale;

        /// <summary>
        /// The size for this parameter.
        /// </summary>
        private int m_size;

        /// <summary>
        /// The <see cref="IRawLayout"/> to use to render the
        /// logging event into an object for this parameter.
        /// </summary>
        private IRawLayout m_layout;

        #endregion // Private Instance Fields
    }
}

#endif // !SSCLI

    

