using System.Runtime.InteropServices.JavaScript;

namespace dbwbs_projekt.Models.Recipes; 

public class RecipeDetails {
    
    public int Id { get; set; }
    public string? Url { get; set; }
    public DateTime? Created { get; set; }
    public string? RecipeName { get; set; }
    public string? Portions { get; set; }
    public string? Description { get; set; }
    public List<string>? ImgUrls { get; set; }
    public List<IngredientDetails>? Ingredients { get; set; }
    
}