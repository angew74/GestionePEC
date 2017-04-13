namespace AspNet.Identity.SQLServerProvider
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;


    public class SQLServerDataContext : IDisposable
    {
        public SQLServerDataContext()
            : this("DefaultConnection")
        {
        }

        public SQLServerDataContext(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            Connection = new SqlConnection(connectionString);
        }

        public SQLServerDataContext(SqlConnection connection)
        {
            Connection = connection;
        }

        ~SQLServerDataContext()
        {
            Dispose(false);
        }

        public SqlConnection Connection { get; private set; }

        [SuppressMessage("Microsoft.Globalization", "CA1306:SetLocaleForDataTypes", Justification = "Review.")]
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "It actually is a parameterized SQL query.")]
        public DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            OpenClosedConnection();

            var resultTable = new DataTable();
            var transaction = Connection.BeginTransaction(IsolationLevel.ReadCommitted);
            var command = new SqlCommand(query, Connection,transaction) { CommandType = CommandType.Text };

            command.Parameters.AddRange(parameters);

            var dataAdapter = new SqlDataAdapter(command);

            try
            {
                dataAdapter.Fill(resultTable);
            }
            catch (SqlException ex)
            {
                // Repeating OracleCommand because the procedure has been invalidated.
                if (ex.Number == 4068)
                {
                    dataAdapter.Fill(resultTable);
                }
                else
                {
                    throw;
                }
            }

            transaction.Commit();

            return resultTable;
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "It actually is a parameterized SQL query.")]
        public object ExecuteScalarQuery(string query, params SqlParameter[] parameters)
        {
            OpenClosedConnection();

            object result;
            var transaction = Connection.BeginTransaction(IsolationLevel.ReadCommitted);
            var command = new SqlCommand(query, Connection,transaction) { CommandType = CommandType.Text };

            command.Parameters.AddRange(parameters);

            try
            {
                result = command.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                // Repeating OracleCommand because the procedure has been invalidated.
                if (ex.Number == 4068)
                {
                    result = command.ExecuteScalar();
                }
                else
                {
                    throw;
                }
            }

            transaction.Commit();

            return result;
        }

        public T ExecuteScalarQuery<T>(string query, params SqlParameter[] parameters)
        {
            return (T)ExecuteScalarQuery(query, parameters);
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "It actually is a parameterized SQL query.")]
        public int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            OpenClosedConnection();

            int result;
            var transaction = Connection.BeginTransaction(IsolationLevel.ReadCommitted);
            var command = new SqlCommand(query, Connection,transaction) { CommandType = CommandType.Text };

            command.Parameters.AddRange(parameters);

            try
            {
                result = command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                // Repeating OracleCommand because the procedure has been invalidated.
                if (ex.Number == 4068)
                {
                    result = command.ExecuteNonQuery();
                }
                else
                {
                    throw;
                }
            }

            transaction.Commit();

            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Connection != null)
                {
                    Connection.Close();
                    Connection.Dispose();
                    Connection = null;
                }
            }
        }

        private void OpenClosedConnection()
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }
    }
}