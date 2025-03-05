using System.Data;
using System.Data.SqlClient;

namespace dbwbs_projekt.Models; 



public class UserMethods {

    
    public static UserDetails ReadUser(string username, out string errormsg) {
          
        errormsg = "";
        UserDetails ud = new UserDetails();
        ud.username = "";
        ud.passwordEncrypted = "";

        SqlConnection dbConnection = MethodsCommon.ConnectDb();
        dbConnection.Open();
        try {
            String sqlString = "SELECT * FROM tbl_user WHERE user_name = @username";
            SqlCommand dbCommand = new SqlCommand(sqlString, dbConnection);
            dbCommand.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
            SqlDataReader reader = dbCommand.ExecuteReader();
            while (reader.Read()) {
                ud.userId = Convert.ToInt16(reader["user_id"]);
                ud.passwordEncrypted = reader["user_password"].ToString();
            }
        }
        catch (Exception e) {
            errormsg = e.Message;
        } finally {
            dbConnection.Close();
        }

        return ud;
    } 
    
    // Returns the user id of the user crated
    public static int? CreateUser(UserDetails ud, out string errormsg) {
          
        errormsg = "";

        // check if user exists
        try {
            
            if (ReadUser(ud.username, out errormsg).username != "") {
                errormsg = "User " + ud.username + "exists";
                throw new Exception(errormsg);
            }
        }
        catch (Exception e) {
            Console.WriteLine(e);
            errormsg = "User " + ud.username + " already exists";
            throw;
        }
        
        
        int? primary = null;
        
        SqlConnection dbConnection = MethodsCommon.ConnectDb();
        dbConnection.Open();
        try {
            String sqlString = "INSERT INTO [tbl_user]\n([user_name])\n" +
                               "VALUES (@username)\n" +
                               "SELECT CAST(scope_identity() AS int);";
            SqlCommand dbCommand = new SqlCommand(sqlString, dbConnection);
            dbCommand.Parameters.Add("@username", SqlDbType.NVarChar).Value = ud.username;
            primary = Convert.ToInt16(dbCommand.ExecuteScalar());
            
        }
        catch (Exception e) {
            errormsg = e.Message;
        } finally {
            dbConnection.Close();
        }

        return primary;
    } 
    
    
    public static void UpdateUser(UserDetails ud, out string errormsg) {
          
        errormsg = "";
        
        SqlConnection dbConnection = MethodsCommon.ConnectDb();
        dbConnection.Open();
        try {
            String sqlString = "UPDATE [tbl_user] SET\n" +
                               "[user_password] = @password\n" +
                               "WHERE user_name = @username";
            SqlCommand dbCommand = new SqlCommand(sqlString, dbConnection);
            dbCommand.Parameters.Add("@username", SqlDbType.NVarChar).Value = ud.username;
            dbCommand.Parameters.Add("@password", SqlDbType.NVarChar).Value = ud.passwordEncrypted;
            dbCommand.ExecuteNonQuery();
        }
        catch (Exception e) {
            errormsg = e.Message;
        } finally {
            dbConnection.Close();
        }
    } 
    
    
    
    
    
}