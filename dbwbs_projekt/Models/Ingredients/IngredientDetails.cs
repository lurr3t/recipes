using System.ComponentModel.DataAnnotations;

namespace dbwbs_projekt.Models; 

public class IngredientDetails {
    
    public int IngredientId { get; set; }
    public string? IngredientQuantity { get; set; }
    public string? IngredientUnit { get; set; }
    public string? IngredientName { get; set; }
    public int? IngredientChecked { get; set; }
    
}