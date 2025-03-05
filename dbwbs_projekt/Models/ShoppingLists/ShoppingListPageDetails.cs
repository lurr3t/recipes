namespace dbwbs_projekt.Models.ShoppingLists; 

public class ShoppingListPageDetails {
    
    public int? sl_id { get; set; }
    public DateTime? sl_created { get; set; }
    public string? sl_title { get; set; }
    public List<IngredientDetails>? Ingredients { get; set; }
    
    
}