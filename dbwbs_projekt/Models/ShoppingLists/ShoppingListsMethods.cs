using System.Data;
using System.Data.SqlClient;

namespace dbwbs_projekt.Models.ShoppingLists; 

public class ShoppingListsMethods {
    
    // returns the id of the created shopping list
    public static int CreateShoppingList(out string errormsg, int user, string? title) {
        
        errormsg = "";
        SqlConnection dbConnection = MethodsCommon.ConnectDb();
        dbConnection.Open();

        int primary = 0;
        
        try {
            string sqlString = "INSERT INTO [tbl_shopping_list]\n([sl_user_id]," +
                               " [sl_created], [sl_title])\nVALUES (@userId, GETDATE()," +
                               " @title)\nSELECT CAST(scope_identity() AS int);";
            SqlCommand dbCommand = new SqlCommand(sqlString, dbConnection);
            
            dbCommand.Parameters.Add("@userId", SqlDbType.Int).Value = user;
            dbCommand.Parameters.AddWithValue("@title", title ?? (object)DBNull.Value);
            
            // Get the primary key from scope_identity
            primary = Convert.ToInt16(dbCommand.ExecuteScalar());
        } catch (Exception e) {
            errormsg = e.Message;
        } finally {
            dbConnection.Close();
        }

        return primary;
    }
    
    
    public static List<ShoppingListPageDetails> ReadShoppingLists(int user, string? search, string? sort, string? order, out string errormsg) {
        errormsg = "";
          
        // Checks the input
        if (search == null) {
            search = "";
        }
        if (sort == null) {
            sort = "sl_created ";
        }
        if (order == null) { 
            order = "DESC";
        }
          
        List<ShoppingListPageDetails> shoppingLists = new List<ShoppingListPageDetails>();
        List<int> keys = new List<int>();
        SqlConnection dbConnection = MethodsCommon.ConnectDb();
        dbConnection.Open();
        try {
            String sqlString = "SELECT sl_id FROM tbl_shopping_list\n" +
                               "FULL JOIN tbl_ingredient ON tbl_ingredient.i_sl_id = tbl_shopping_list.sl_id\n" +
                               "INNER JOIN tbl_user ON tbl_user.user_id = tbl_shopping_list.sl_user_id\n" +
                               "WHERE (user_id=@user) AND\n" +
                               "      ((sl_title LIKE '%' + @input + '%') OR\n" +
                               "      (sl_created LIKE '%' + @input + '%') OR\n" +
                               "      (i_amount LIKE '%' + @input + '%') OR\n" +
                               "      (i_name LIKE '%' + @input + '%') OR\n" +
                               "      (i_unit LIKE '%' + @input + '%'))\n" +
                               "GROUP BY sl_id, sl_created, sl_title\n" +
                               "ORDER BY sl_created DESC";
            
            SqlCommand dbCommand = new SqlCommand(sqlString, dbConnection);
              
            dbCommand.Parameters.Add("@user", SqlDbType.Int).Value = user;
            dbCommand.Parameters.Add("@input", SqlDbType.NVarChar).Value = search;
            //dbCommand.Parameters.Add("@sort", SqlDbType.NVarChar).Value = sort;
            //dbCommand.Parameters.Add("@order", SqlDbType.NVarChar).Value = order;
              
            SqlDataReader reader = dbCommand.ExecuteReader();
              
            while (reader.Read()) {
                keys.Add((Convert.ToInt16(reader["sl_id"])));
            }
        }
        catch (Exception e) {
            errormsg = e.Message;
        } finally {
            dbConnection.Close();
        }

        foreach (int key in keys) {
            shoppingLists.Add(ReadShoppingList(key, out errormsg));
        }

        return shoppingLists;
    } 
    
    public static ShoppingListPageDetails ReadShoppingList(int slId, out string errormsg) {
          
        errormsg = "";


        ShoppingListPageDetails shoppingList = new ShoppingListPageDetails();
        // Adds the list id to th list
        shoppingList.sl_id = slId;
        SqlConnection dbConnection = MethodsCommon.ConnectDb();
        dbConnection.Open();
        try {
            String sqlString = "SELECT * FROM tbl_shopping_list WHERE sl_id = @id";
            SqlCommand dbCommand = new SqlCommand(sqlString, dbConnection);
            dbCommand.Parameters.Add("@id", SqlDbType.Int).Value = slId;
            SqlDataReader reader = dbCommand.ExecuteReader();
            while (reader.Read()) {
                shoppingList.sl_created = Convert.ToDateTime(reader["sl_created"]);
                shoppingList.sl_title = reader["sl_title"].ToString();
            }
        }
        catch (Exception e) {
            errormsg = e.Message;
        } finally {
            dbConnection.Close();
        }

        IngredientsMethods im = new IngredientsMethods();
        shoppingList.Ingredients = im.ReadIngredients(dbConnection, null, slId, out errormsg);

        return shoppingList;
    } 
    
    public static void DeleteShoppingList(int slId, out string errormsg) {
        errormsg = "";
        SqlConnection dbConnection = MethodsCommon.ConnectDb();
        dbConnection.Open();
        try {
            String sqlString = "DELETE FROM tbl_shopping_list WHERE sl_id = @id";
            SqlCommand dbCommand = new SqlCommand(sqlString, dbConnection);
            dbCommand.Parameters.Add("@id", SqlDbType.Int).Value = slId;
            dbCommand.ExecuteNonQuery();
        }
        catch (Exception e) {
            errormsg = e.Message;
        } finally {
            dbConnection.Close();
        }
    }

    public static void UpdateShoppingListName(string title, int listId, out string errormsg) {
        errormsg = "";
        SqlConnection dbConnection = MethodsCommon.ConnectDb();
        dbConnection.Open();
        try {
            String sqlString = "UPDATE tbl_shopping_list SET sl_title = @title WHERE sl_id = @id";
            SqlCommand dbCommand = new SqlCommand(sqlString, dbConnection);
            dbCommand.Parameters.Add("@title", SqlDbType.NVarChar).Value = title;
            dbCommand.Parameters.Add("@id", SqlDbType.Int).Value = listId;
            dbCommand.ExecuteNonQuery();
        }
        catch (Exception e) {
            errormsg = e.Message;
            Console.WriteLine(e);
        }
        finally {
            dbConnection.Close();

        }
    }



}