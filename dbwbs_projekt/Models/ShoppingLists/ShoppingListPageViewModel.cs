using dbwbs_projekt.Models.Recipes;

namespace dbwbs_projekt.Models.ShoppingLists; 

public class ShoppingListPageViewModel {

    public bool Search {get; set;}
    public ShoppingListPageDetails? ShoppingList {get; set;}
    public List<RecipeDetails>? Recipes {get; set;}

}