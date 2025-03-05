using System.ComponentModel.DataAnnotations;

namespace dbwbs_projekt.Models; 

public class UserDetails {
    
    public string? passwordEncrypted { get; set; }
    public int? userId { get; set; }
    [Required]
    public string? username { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Invalid password format")]
    public string? password { get; set; }
    public int? login { get; set; }
    public int? create { get; set; }
    
}