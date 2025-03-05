namespace dbwbs_projekt.Models.Recipes; 

public class RecipesViewModel {

    public string? msg { get; set; }
    public string? errormsg { get; set;}
    public string? searchString { get; set; }
    // remembers if the user has edited the recipe or newly added it
    public int Edited;
    public RecipeDetails? AddedRecipe { get; set; }
    public List<RecipeDetails> Recipes { get; set; }
    
    
    
}