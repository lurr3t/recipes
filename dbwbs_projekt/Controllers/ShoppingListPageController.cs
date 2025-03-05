using System.Data.SqlClient;
using System.Data.SqlTypes;
using dbwbs_labb3.Models.CookieHandler;
using dbwbs_projekt.Models;
using dbwbs_projekt.Models.GptHandler;
using dbwbs_projekt.Models.Recipes;
using dbwbs_projekt.Models.ShoppingLists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dbwbs_projekt.Controllers; 

public class ShoppingListPageController : Controller {
    
    
    // GET
    [Authorize]
    public IActionResult ShoppingListPage(int? listId, string? searchString, int clear) {
        
        /*
        ControllerCommon.SetMetaData(HttpContext, ViewData, "Shopping List Page",
            "Shopping List for the recipe site", new List<string>() {
                "shopping list", "food", "food recipes", "shopping list site"
            });
            */
        
        string errormsg = "";

        ShoppingListPageDetails? slpd = new ShoppingListPageDetails();
        int primaryKey = 0;
        
        if (listId != null) {
            primaryKey = (int)listId;
            slpd = ShoppingListsMethods.ReadShoppingList(primaryKey, out errormsg);
            IngredientsMethods im = new IngredientsMethods();
            SqlConnection db = MethodsCommon.ConnectDb();
            slpd.Ingredients = im.ReadIngredients(db, null,primaryKey, out errormsg);
        } else {
            slpd = MemStorageHandler.GetShoppingListPageDetailsCookie(HttpContext);
            if (slpd != null) {
                // used later when adding ingredients
                primaryKey = (int) slpd.sl_id!;
            }
        }
        slpd.sl_id = primaryKey;
        
        ShoppingListPageViewModel slpdvm = new ShoppingListPageViewModel();
        slpdvm.ShoppingList = slpd;
        
        if (searchString != null || clear == 1) {
            slpdvm.Search = true;
        }
        
        // get the recipes from db
        RecipesMethods rm = new RecipesMethods();
        int userId = (int)MemStorageHandler.GetUserDetailsCookie(HttpContext).userId;
        if (clear == 0) {
            slpdvm.Recipes = rm.ReadRecipes(userId, searchString, null, null, out errormsg);    
        } else {
            slpdvm.Recipes = rm.ReadRecipes(userId, null, null, null, out errormsg);
        }
        
        
        return View(slpdvm);
    }
    
    [HttpPost]
    public RedirectToActionResult AddIngredient(int regularId, string unit, string amount, string name) {

        IngredientDetails id = new IngredientDetails();
        id.IngredientName = name;
        id.IngredientUnit = unit;
        id.IngredientQuantity = amount;
        SqlConnection db = MethodsCommon.ConnectDb();
        db.Open();
        IngredientsMethods.CreateIngredient(db, id, regularId, null, null);
        db.Close();
        
        return RedirectToAction("ShoppingListPage", new { listId = regularId });

    }

    [HttpPost]
    public RedirectToActionResult CheckIngredient(int ingredientId, int listId, int check) {
        string errormsg = "";
        // work in progress
        SqlConnection db = MethodsCommon.ConnectDb();
        IngredientsMethods im = new IngredientsMethods();
        // First needs to read the ingredient
        IngredientDetails id = im.ReadIngredients(db, null, listId, out errormsg).Find(x => x.IngredientId == ingredientId); 
        id.IngredientChecked = check;
        
        db.Open();
        IngredientsMethods.UpdateIngredient(db, id, ingredientId, null);
        db.Close();

        
        return RedirectToAction("ShoppingListPage", new {listId});
    }

    public RedirectToActionResult DeleteIngredient(int ingredientId, int listId) {
        SqlConnection db = MethodsCommon.ConnectDb();
        db.Open();
        IngredientsMethods.DeleteIngredient(db, ingredientId);
        db.Close();
        
        return RedirectToAction("ShoppingListPage", new {listId});
    }
    
    [HttpPost]
    public RedirectToActionResult AddIngredientAi(int listId, List<int> recipeIds) {

        string errormsg = "";
        SqlConnection db = MethodsCommon.ConnectDb();
        // get all ingredients from recipes
        List<IngredientDetails> allIngredients = new List<IngredientDetails>();
        IngredientsMethods im = new IngredientsMethods();
        foreach (int recipeId in recipeIds) {
            if (recipeId != 0) {
                List<IngredientDetails> ingredients = im.ReadIngredients(db, recipeId, null, out errormsg);
                foreach (IngredientDetails ingredient in ingredients) {
                    allIngredients.Add(ingredient);
                }   
            }
        } 
        
        // get all ingredients from shopping list
        List<IngredientDetails> listIngredients = im.ReadIngredients(db, null, listId, out errormsg);
        foreach (IngredientDetails ingredient in listIngredients) {
            allIngredients.Add(ingredient);
        }
        
        // Delete the current ingredients from shopping list
        foreach (IngredientDetails ingredient in listIngredients) {
            db.Open();
            IngredientsMethods.DeleteIngredient(db, ingredient.IngredientId);
            db.Close();
        }
        
        // join with ai
        /*
         * The ai function is turned off since it doesn't work properly. Now it only adds
         * everything together
         */
        try {
            GptHandler gh = new GptHandler();
            //Task<List<IngredientDetails>> joinedIngredients = gh.JoinIngredients(allIngredients);
            
            db.Open();
            foreach (IngredientDetails ingredient in allIngredients) {
                IngredientsMethods.CreateIngredient(db, ingredient, listId, null, null);
            }
            db.Close();
        }
        catch (Exception e) {
            Console.WriteLine(e);
        }
        
        
        
        return RedirectToAction("ShoppingListPage", new { listId = listId });

    }

    [HttpPost]
    public RedirectToActionResult UpdateTitle(string title, int listId) {
        string errormsg = "";
        
        SqlConnection db = MethodsCommon.ConnectDb();
        ShoppingListsMethods.UpdateShoppingListName(title, listId, out errormsg);
        return RedirectToAction("ShoppingListPage", new { listId = listId });
    }
    
}