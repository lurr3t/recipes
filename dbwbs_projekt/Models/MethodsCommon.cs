using System.Data.SqlClient;

namespace dbwbs_projekt.Models; 

public class MethodsCommon {
    
    // remember to close connection after use
    public static SqlConnection ConnectDb() {
        Console.WriteLine("Connecting to the database...");
        SqlConnection dbConnection = new SqlConnection();
        var password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD");
        dbConnection.ConnectionString = $"Server=db; Database=projekt; User Id=sa; Password={password}; TrustServerCertificate=True";
        Console.WriteLine("Connected to the database");
        return dbConnection;
    } 
    
    
}