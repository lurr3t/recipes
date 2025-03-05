using System.Diagnostics;
using CsvHelper;
using dbwbs_labb3.Models.CookieHandler;
using Microsoft.AspNetCore.Mvc;
using dbwbs_projekt.Models;
using dbwbs_projekt.Models.GptHandler;
using dbwbs_projekt.Models.Recipes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;


namespace dbwbs_projekt.Controllers;

public class RecipesController : Controller {
    private readonly ILogger<RecipesController> _logger;

    public RecipesController(ILogger<RecipesController> logger) {
        _logger = logger;
    }

    [Authorize]
    public IActionResult Recipes(string? searchString, string? sortOrder) {
        ControllerCommon.SetMetaData(HttpContext, ViewData, "Recipes",
            "Recipes for the recipe site", new List<string>() {
                "recipes", "recipe", "food", "food recipes", "recipe site"
            });
        
        
        string errormsg = "";

        RecipesViewModel vm = new RecipesViewModel();
        
        if (MemStorageHandler.GetRecipesViewModelCookie(HttpContext) != null && searchString == null) {
            searchString = MemStorageHandler.GetRecipesViewModelCookie(HttpContext)?.searchString;
        } else if (searchString != null) {
            vm.searchString = searchString;
            MemStorageHandler.SaveRecipesViewModelToCookie(vm, HttpContext);
        }
        ViewBag.searchString = searchString;
        
        // Read from db
        RecipesMethods rm = new RecipesMethods();
        vm.Recipes = rm.ReadRecipes((int)MemStorageHandler.GetUserDetailsCookie(HttpContext).userId, searchString, sortOrder, null, out errormsg);
        
        // Read from session
        var recipe = MemStorageHandler.GetRecipesViewModelSession(HttpContext); 
        if (recipe != null) {
            vm.Edited = recipe.Edited;
            vm.AddedRecipe = recipe.AddedRecipe;
            ViewBag.validate = recipe.msg!;
            

        }
        
        return View(vm);
    }
    
    [HttpPost]
    public RedirectToActionResult GetRecipeAi(string recipeUrl) {
        
        RecipeDetails addedRecipe;
        GptHandler gpt = new GptHandler();
        try {
            addedRecipe = gpt.Run(recipeUrl);
            // add it to sessions
            RecipesViewModel recipesViewModel = new RecipesViewModel();
            recipesViewModel.AddedRecipe = addedRecipe;
            MemStorageHandler.SaveRecipesViewModelToSession(recipesViewModel, HttpContext);
        }
        catch (Exception e) {
            ViewBag.errormsg = e.Message;
        }
        
        
        return RedirectToAction("Recipes");
    }


    [HttpPost]
    public RedirectToActionResult AddedRecipe(RecipeDetails rd, int? deleteIngredientIndex, bool addIngredient, int Edited, int imageIndex) {


        bool isValid = IsValid(rd);
        
        MemStorageHandler.ClearRecipesViewModelSession(HttpContext);

        if (deleteIngredientIndex != null) {
            rd.Ingredients.RemoveAt((int) deleteIngredientIndex);
        }
        else if (addIngredient) {
            rd.Ingredients?.Add(new IngredientDetails());
        }
        
        // save to db
        if (deleteIngredientIndex == null && !addIngredient && Edited == 0 && isValid) {
            // saves the selected image to the the first position in the image list
            if (rd.ImgUrls != null) rd.ImgUrls[0] = rd.ImgUrls[imageIndex];
            RecipesMethods rm = new RecipesMethods();
            string errormsg = "";
            rm.CreateRecipe((int)MemStorageHandler.GetUserDetailsCookie(HttpContext).userId, rd, out errormsg);    
        }
        // update
        else if (deleteIngredientIndex == null && !addIngredient && Edited == 1 && isValid) {
            RecipesMethods rm = new RecipesMethods();
            string errormsg = "";
            rm.UpdateRecipe(rd, out errormsg);
        }
        // save to session
        else {
            RecipesViewModel vm = new RecipesViewModel();
            vm.AddedRecipe = rd;
            vm.Edited = Edited;
            if (!isValid && !addIngredient && deleteIngredientIndex == null) {
                vm.msg = "The format for the quantity is not valid!";    
            }
            MemStorageHandler.SaveRecipesViewModelToSession(vm, HttpContext);
        }
        
        return RedirectToAction("Recipes");
    }

    
    public RedirectToActionResult AddedRecipeDelete() {
        MemStorageHandler.ClearRecipesViewModelSession(HttpContext);
        return RedirectToAction("recipes");
    }
    
    
    
    public RedirectToActionResult ClearSearch() {
        MemStorageHandler.ClearRecipesViewModelCookie(HttpContext);
        return RedirectToAction("Recipes");
    }
    
    
    [HttpPost]
    public RedirectToActionResult Delete(int deleteRecipeId) {
        string errormsg;
        RecipesMethods rm = new RecipesMethods();
        rm.DeleteRecipe(deleteRecipeId, out errormsg);
        ViewBag.errormsg = errormsg;
        return RedirectToAction("Recipes");
    }
    
    // Also saves if it's new or edited
    [HttpPost]
    public RedirectToActionResult Edit(int editRecipeId) {
        string errormsg;
        RecipesViewModel rvm = new RecipesViewModel();
        RecipesMethods rm = new RecipesMethods();

        rvm.Edited = 1;
        rvm.AddedRecipe = rm.ReadRecipe(editRecipeId, out errormsg);
        rvm.errormsg = errormsg;
        
        MemStorageHandler.ClearRecipesViewModelSession(HttpContext);
        MemStorageHandler.SaveRecipesViewModelToSession(rvm, HttpContext);
        return RedirectToAction("Recipes");
    }

    // Validates if the quantity is a float. And changes , to .
    private bool IsValid(RecipeDetails rd) {
        bool isValid = true;
        
        foreach (IngredientDetails ingredient in rd.Ingredients) {
            isValid = float.TryParse(ingredient.IngredientQuantity, out _);
            if (isValid) {
                ingredient.IngredientQuantity = ingredient.IngredientQuantity.Replace(',', '.');
            } else if (ingredient.IngredientQuantity == null) {
                isValid = true;
            }
            else if (!isValid && ingredient.IngredientQuantity.Contains(".")) {
                isValid = true;
            }
            else {
                break;
            }
        }
        return isValid;
    }
    
    
    
    
}