using dbwbs_labb3.Models.CookieHandler;
using dbwbs_projekt.Models.ShoppingLists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dbwbs_projekt.Controllers; 

public class ShoppingListsController : Controller {
    // GET
    
    [Authorize]
    public IActionResult ShoppingLists(string searchString) {
        
        ControllerCommon.SetMetaData(HttpContext, ViewData, "Shopping Lists",
            "Shopping Lists for the recipe site", new List<string>() {
                "shopping lists", "shopping list", "food", "food recipes", "shopping list site"
            });
        
        
        string errormsg = "";
        ShoppingListsDetails shoppingListsDetails = new ShoppingListsDetails();
        
        
        if (searchString != null) {
            MemStorageHandler.SaveToCookie(new ShoppingListsDetails() {
                searchString = searchString
            }, HttpContext, "ShoppingListsDetails");
        } else {
            try {
                ShoppingListsDetails result = MemStorageHandler.GetCookie<ShoppingListsDetails>(HttpContext, "ShoppingListsDetails");
                searchString = result.searchString;
            }
            catch (Exception e) { }
        }
        
        ViewBag.searchString = searchString;
        
        shoppingListsDetails.ShoppingLists = ShoppingListsMethods.ReadShoppingLists((int)MemStorageHandler.GetUserDetailsCookie(HttpContext).userId, searchString, null, null, out errormsg);
        
        return View(shoppingListsDetails);
    }
    
    public RedirectToActionResult AddShoppingList(string shoppingListName) {
        string errormsg = "";
        
        ShoppingListPageDetails shoppingListPageDetails = new ShoppingListPageDetails();
        
        // create in db
        shoppingListPageDetails.sl_id = ShoppingListsMethods.CreateShoppingList(out errormsg, (int)MemStorageHandler.GetUserDetailsCookie(HttpContext).userId, shoppingListName);
        shoppingListPageDetails.sl_title = shoppingListName;
        // add key to cookies
        
        MemStorageHandler.SaveShoppingListPageDetailsToCookie(shoppingListPageDetails, HttpContext);
        
        return new RedirectToActionResult("ShoppingListPage", "ShoppingListPage", null);
    }

    public RedirectToActionResult DeleteShoppingList(int listId) {
        
        string errormsg = "";

        ShoppingListsMethods.DeleteShoppingList(listId, out errormsg);
        return new RedirectToActionResult("ShoppingLists", "ShoppingLists", null);
    }

    public RedirectResult ClearSearch() {
        MemStorageHandler.ClearCookie(HttpContext, "ShoppingListsDetails");
        return new RedirectResult("ShoppingLists");
    }
    
    
}