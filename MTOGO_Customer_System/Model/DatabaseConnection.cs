using Microsoft.Data.SqlClient;

namespace MTOGO_Customer_System.Model
{
    public class DatabaseConnection
    {
        private static readonly Lazy<DatabaseConnection> _instance = new Lazy<DatabaseConnection>(() => new DatabaseConnection());
        private SqlConnection _connection;
        private bool _isConnectionOpen = false;
        private readonly string _connectionString = "data source=Diekmann-Laptop;initial catalog=MTOGO;trusted_connection=true;TrustServerCertificate=True;";
        private DatabaseConnection()
        {
            _connection = new SqlConnection(_connectionString);
        }

        public static DatabaseConnection Instance => _instance.Value;

        public SqlConnection Connection
        {
            get
            {
                // Open connection if it isn't already open
                if (_connection.State != System.Data.ConnectionState.Open && !_isConnectionOpen)
                {
                    try
                    {
                        _connection = new SqlConnection(_connectionString);
                        _connection.Open();
                        _isConnectionOpen = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error opening connection: " + ex.Message);
                    }
                }
                return _connection;
            }
        }

        public void CloseConnection()
        {
            _connection.Close();
            _isConnectionOpen = false;
        }

        // Optionally reset the connection if needed
        public void ResetConnection()
        {
            if (_connection.State != System.Data.ConnectionState.Closed)
            {
                _connection.Close();
            }

            _connection = new SqlConnection(_connectionString); // Recreate connection if necessary
            _isConnectionOpen = false;
        }
    }
}
