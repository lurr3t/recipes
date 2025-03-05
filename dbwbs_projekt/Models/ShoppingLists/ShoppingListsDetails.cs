namespace dbwbs_projekt.Models.ShoppingLists; 

public class ShoppingListsDetails {
    
    public string? searchString { get; set; }
    public string? Title { get; set; }
    public DateTime Created { get; set; }
    public List<ShoppingListPageDetails>? ShoppingLists { get; set; }
    
}