using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;


namespace dbwbs_projekt.Models.Recipes; 

public class RecipesMethods {
    
    private SqlConnection ConnectDb() {
        Console.WriteLine("Connecting to the database recipes...");
        SqlConnection dbConnection = new SqlConnection();
        var password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD");
        dbConnection.ConnectionString = $"Server=db; Database=projekt; User Id=sa; Password={password}; TrustServerCertificate=True";
        Console.WriteLine("Connected to the database recipes");
        return dbConnection;
    } 
    
    public void CreateRecipe(int user, RecipeDetails recipe, out string errormsg) {
        errormsg = "";
        
        SqlConnection dbConnection = ConnectDb();
        dbConnection.Open();
        try {
            String sqlString = "INSERT INTO [tbl_recipe]\n" +
                               "    ([recipe_user_id], [recipe_title], [recipe_description], [recipe_url]," +
                               " [recipe_created], [recipe_img_url], [recipe_portions])\n" +
                               "VALUES (@userId, @recipeName, @recipeDesc," +
                               " @recipeUrl, CAST( GETDATE() AS Date ), @imgUrl, @portions);\nSELECT CAST(scope_identity() AS int);";
            SqlCommand dbCommand = new SqlCommand(sqlString, dbConnection);
            
            dbCommand.Parameters.Add("@userId", SqlDbType.Int).Value = user;
            dbCommand.Parameters.AddWithValue("@recipeName", recipe.RecipeName ?? (object)DBNull.Value);
            dbCommand.Parameters.AddWithValue("@recipeDesc", recipe.Description ?? (object)DBNull.Value);
            dbCommand.Parameters.AddWithValue("@recipeUrl", recipe.Url ?? (object)DBNull.Value);
            dbCommand.Parameters.AddWithValue("@recipeCreated", recipe.Created ?? (object)DBNull.Value);
            dbCommand.Parameters.AddWithValue("@imgUrl", recipe.ImgUrls.First() ?? (object)DBNull.Value);
            dbCommand.Parameters.AddWithValue("@portions", Convert.ToInt16(recipe.Portions));
            
            
            // Get the primary key from scope_identity
            int primary = Convert.ToInt16(dbCommand.ExecuteScalar());
            
            // Each ingredient
            if (recipe.Ingredients != null)
                foreach (IngredientDetails ingredient in recipe.Ingredients) {
                    IngredientsMethods.CreateIngredient(dbConnection, ingredient, null, primary, null);
                    
                }
        }
        catch (Exception e) {
            errormsg = e.Message;
        } finally {
            dbConnection.Close();
        }
    }

      
      public List<RecipeDetails> ReadRecipes(int user, string? search, string? sort, string? order, out string errormsg) {
          errormsg = "";
          
          // Checks the input
          if (search == null) {
              search = "";
          }
          if (sort == null) {
              sort = "recipe_created ";
          }
          if (order == null) { 
              order = "DESC";
          }
          
          List<RecipeDetails> recipes = new List<RecipeDetails>();
          List<int> keys = new List<int>();
          SqlConnection dbConnection = ConnectDb();
          dbConnection.Open();
          try {
              String sqlString = "SELECT recipe_id FROM tbl_recipe\n" +
                                 "INNER JOIN tbl_ingredient ON tbl_ingredient.i_recipe_id = tbl_recipe.recipe_id\n" +
                                 "INNER JOIN tbl_user ON tbl_user.user_id = tbl_recipe.recipe_user_id\n" +
                                 "WHERE (user_id=@user) AND\n      ((recipe_title LIKE '%' + @input + '%') OR\n" +
                                 "      (recipe_description LIKE '%' + @input + '%') OR\n" +
                                 "      (recipe_created LIKE '%' + @input + '%') OR\n" +
                                 "      (i_amount LIKE '%' + @input + '%') OR\n" +
                                 "      (i_name LIKE '%' + @input + '%') OR\n" +
                                 "      (i_unit LIKE '%' + @input + '%'))\n" +
                                 "GROUP BY recipe_id, recipe_title, recipe_description, recipe_created\n" +
                                 "ORDER BY recipe_created DESC;";
              SqlCommand dbCommand = new SqlCommand(sqlString, dbConnection);
              
              dbCommand.Parameters.Add("@user", SqlDbType.Int).Value = user;
              dbCommand.Parameters.Add("@input", SqlDbType.NVarChar).Value = search;
             //dbCommand.Parameters.Add("@sort", SqlDbType.NVarChar).Value = sort;
              //dbCommand.Parameters.Add("@order", SqlDbType.NVarChar).Value = order;
              
              SqlDataReader reader = dbCommand.ExecuteReader();
              
              while (reader.Read()) {
                  keys.Add((Convert.ToInt16(reader["recipe_id"])));
              }
          }
          catch (Exception e) {
              errormsg = e.Message;
          } finally {
              dbConnection.Close();
          }

          foreach (int key in keys) {
              recipes.Add(ReadRecipe(key, out errormsg));
          }

          return recipes;
      }
      
      public RecipeDetails ReadRecipe(int recipeId, out string errormsg) {
          
          errormsg = "";
          
          
          RecipeDetails recipe = new RecipeDetails();
          // Adds the recipeId to the recipe
          recipe.Id = recipeId;
          SqlConnection dbConnection = ConnectDb();
          dbConnection.Open();
          try {
              String sqlString = "SELECT * FROM tbl_recipe WHERE recipe_id = @recipeId";
              SqlCommand dbCommand = new SqlCommand(sqlString, dbConnection);
              dbCommand.Parameters.Add("@recipeId", SqlDbType.Int).Value = recipeId;
              SqlDataReader reader = dbCommand.ExecuteReader();
              while (reader.Read()) {
                  recipe.Created = Convert.ToDateTime(reader["recipe_created"]);
                  recipe.RecipeName = reader["recipe_title"].ToString();
                  // adding url
                  List<string> urls = new List<string>();
                  urls.Add(reader["recipe_img_url"].ToString());
                  recipe.ImgUrls = urls;
                  recipe.Portions = reader["recipe_portions"].ToString();
                  recipe.Description = reader["recipe_description"].ToString();
                  recipe.Url = reader["recipe_url"].ToString();
              }
          }
          catch (Exception e) {
              errormsg = e.Message;
          } finally {
              dbConnection.Close();
          }

          IngredientsMethods im = new IngredientsMethods();
          recipe.Ingredients = im.ReadIngredients(dbConnection, recipeId, null, out errormsg);

          return recipe;
      } 
      
      
      public void DeleteRecipe(int recipeId, out string errormsg) {
          errormsg = "";
          SqlConnection dbConnection = ConnectDb();
          dbConnection.Open();

          try {

              const string sqlString = "DELETE FROM tbl_recipe\nWHERE recipe_id=@id;";
              SqlCommand dbCommand = new SqlCommand(sqlString, dbConnection);
              dbCommand.Parameters.Add("@id", SqlDbType.Int).Value = recipeId;
              dbCommand.ExecuteNonQuery();
          }
          catch (Exception e) {
              errormsg = e.Message;
          } finally { dbConnection.Close(); }
        
      }

      public void UpdateRecipe(RecipeDetails newRecipe, out string errormsg) { 
            errormsg = "";

          // Connect to the database
          SqlConnection dbConnection = ConnectDb();
          dbConnection.Open();

        
          SqlTransaction transaction = dbConnection.BeginTransaction();

          try
          {
              // Read the old recipe from the database
              RecipeDetails oldRecipe = ReadRecipe(newRecipe.Id, out errormsg);
              
              string sqlString = "UPDATE [tbl_recipe] SET\n[recipe_title] = @title,\n" +
                                 "[recipe_description] = @description,\n" +
                                 "\n[recipe_portions] = @portions\nWHERE recipe_id=@id;"; 
              
              SqlCommand updateCommand = new SqlCommand(sqlString, dbConnection, transaction);

              updateCommand.Parameters.AddWithValue("@id", newRecipe.Id);
              updateCommand.Parameters.AddWithValue("@title", newRecipe.RecipeName ?? (object)DBNull.Value);
              updateCommand.Parameters.AddWithValue("@description", newRecipe.Description ?? (object)DBNull.Value);
              //updateCommand.Parameters.AddWithValue("@imgUrl", oldRecipe.ImgUrls!.First() ?? (object)DBNull.Value);
              updateCommand.Parameters.AddWithValue("@portions", newRecipe.Portions ?? (object)DBNull.Value);

              updateCommand.ExecuteNonQuery();

                  // Compare and update ingredients
                  if (newRecipe.Ingredients != null)
                  {
                      foreach (IngredientDetails newIngredient in newRecipe.Ingredients)
                      {
                          // Find the corresponding old ingredient
                          IngredientDetails oldIngredient = oldRecipe.Ingredients.Find(c => c.IngredientId == newIngredient.IngredientId);

                          // If the old ingredient doesn't exist, insert it
                          if (oldIngredient == null) {
                              IngredientsMethods.CreateIngredient(dbConnection, newIngredient, null, newRecipe.Id, transaction);
                          }
                          else {
                              IngredientsMethods.UpdateIngredient(dbConnection, newIngredient, oldIngredient.IngredientId, transaction);
                          }
                      }
    
                      // Delete ingredients that were not included in the new recipe
                      foreach (IngredientDetails oldIngredient in oldRecipe.Ingredients)
                      {
                          if (newRecipe.Ingredients.Find(c => c.IngredientId == oldIngredient.IngredientId) == null)
                          {
                              SqlCommand deleteComponentCommand = new SqlCommand(
                                  "DELETE FROM [tbl_ingredient] " +
                                  "WHERE [i_id] = @id",
                                  dbConnection,
                                  transaction);

                              deleteComponentCommand.Parameters.AddWithValue("@id", oldIngredient.IngredientId);
                              deleteComponentCommand.ExecuteNonQuery();
                          }
                      } 
                  }
                  // Commit the transaction
                  transaction.Commit();
          }
          catch (Exception e)
          {
              errormsg = e.Message;
              transaction.Rollback();
          }
          finally
          {
              dbConnection.Close();
          }
      }
      
      
      
      
      
}