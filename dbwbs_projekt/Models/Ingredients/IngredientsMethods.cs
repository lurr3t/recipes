using System.Data;
using System.Data.SqlClient;

namespace dbwbs_projekt.Models; 

public class IngredientsMethods {
    
    
    // needs to have an open connection. Needs to be closed ater use
    public static void CreateIngredient(SqlConnection dbConnection, IngredientDetails ingredient, int? slKey,
        int? recipeKey, SqlTransaction transaction) {
        string sqlString;
        SqlCommand dbCommand;
        int id = 0;

        if (recipeKey != null) {
            sqlString =
                "INSERT INTO [tbl_ingredient]\n" +
                "    ([i_recipe_id], [i_amount], [i_name], [i_unit])\n" +
                "VALUES (@id, @Amount, @Name, @Unit);";
            id = (int)recipeKey;
        } else if (slKey != null) {
            sqlString =
                "INSERT INTO [tbl_ingredient]\n" +
                "    ([i_sl_id], [i_amount], [i_name], [i_unit], [i_checked])\n" +
                "VALUES (@id, @Amount, @Name, @Unit, @Checked);";
            id = (int)slKey;
        } else {
            return;
        }
        
        

        dbCommand = new SqlCommand(sqlString, dbConnection, transaction);

        // Converts the amount
        double? amount = null;
        if (ingredient.IngredientQuantity != null) {
            if (double.TryParse((ingredient.IngredientQuantity), out _)) {
                amount = Convert.ToDouble(ingredient.IngredientQuantity);    
            }
        }

        dbCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;
        dbCommand.Parameters.AddWithValue("@Amount", amount ?? (object)DBNull.Value);
        dbCommand.Parameters.AddWithValue("@Name", ingredient.IngredientName ?? (object)DBNull.Value);
        dbCommand.Parameters.AddWithValue("@Unit", ingredient.IngredientUnit ?? (object)DBNull.Value);
        if (slKey != null) {
            dbCommand.Parameters.AddWithValue("@Checked", ingredient.IngredientChecked ?? (object)DBNull.Value);
        }

        dbCommand.ExecuteNonQuery();
    }


    public static void UpdateIngredient(SqlConnection dbConnection, IngredientDetails ingredient, int primary, SqlTransaction? transaction) {
        
        SqlCommand dbCommand;
        string sqlString = 
            "UPDATE [tbl_ingredient] SET\n" +
            "[i_amount] = @amount,\n[i_name] = @name,\n" +
            "[i_unit] = @unit, [i_checked] = @checked\nWHERE i_id = @id;";

        dbCommand = new SqlCommand(sqlString, dbConnection, transaction);
        
        dbCommand.Parameters.AddWithValue("@amount", ingredient.IngredientQuantity ?? (object)DBNull.Value);
        dbCommand.Parameters.AddWithValue("@name", ingredient.IngredientName ?? (object)DBNull.Value);
        dbCommand.Parameters.AddWithValue("@unit", ingredient.IngredientUnit ?? (object)DBNull.Value);
        dbCommand.Parameters.AddWithValue("@checked", ingredient.IngredientChecked ?? (object)DBNull.Value);
        dbCommand.Parameters.AddWithValue("@id", primary);

        dbCommand.ExecuteNonQuery();
    }
    
    
    
    public static void DeleteIngredient(SqlConnection dbConnection, int primary) {
        SqlCommand dbCommand;
        const string sqlString = "DELETE FROM tbl_ingredient\nWHERE i_id=@id;";
        dbCommand = new SqlCommand(sqlString, dbConnection);
        dbCommand.Parameters.AddWithValue("@id", primary);
        dbCommand.ExecuteNonQuery();
    }
    
    
    
    public List<IngredientDetails> ReadIngredients(SqlConnection dbConnection, int? recipeId, int? slId,
        out string errormsg) {
          
        errormsg = "";

        List<IngredientDetails> ingredients = new List<IngredientDetails>();
        dbConnection.Open();
        try {
            String sqlString = "";
            int id = 0;
            
            if (recipeId != null) {
                sqlString = "SELECT * FROM tbl_ingredient WHERE i_recipe_id = @id";
                id = (int)recipeId;
            } else if (slId != null) {
                sqlString = "SELECT * FROM tbl_ingredient WHERE i_sl_id = @id";
                id = (int)slId;
            } else {
                errormsg = "No id provided";
                return ingredients;
            }
            SqlCommand dbCommand = new SqlCommand(sqlString, dbConnection);
            
            dbCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;
            
            
            SqlDataReader reader = dbCommand.ExecuteReader();
            while (reader.Read()) {
                IngredientDetails ingredient = new IngredientDetails();
                if (!Convert.IsDBNull(reader[("i_name")])) {
                    ingredient.IngredientName = reader["i_name"].ToString();
                }
                if (!Convert.IsDBNull(reader[("i_amount")])) {
                    ingredient.IngredientQuantity = reader["i_amount"].ToString();
                }
                if (!Convert.IsDBNull(reader[("i_unit")])) {
                    ingredient.IngredientUnit = reader["i_unit"].ToString();
                }
                if (!Convert.IsDBNull(reader[("i_id")])) {
                    ingredient.IngredientId = Convert.ToInt16(reader["i_id"]);
                }
                if (!Convert.IsDBNull(reader[("i_checked")])) {
                    ingredient.IngredientChecked = Convert.ToInt16(reader["i_checked"]);
                }
                ingredients.Add(ingredient);
            }
        }
        catch (Exception e) {
            errormsg = e.Message;
        } finally {
            dbConnection.Close();
        }

        return ingredients;
    } 
    
}