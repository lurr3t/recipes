using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace dbwbs_projekt.Models; 

public class Authentication {
    
    static public bool IsLoggedIn(HttpContext httpContext) {
        return httpContext.Session.GetString("username") != null;
        
    }

    static public bool Login(UserDetails udInput) {
        string errormsg = "";
        PasswordHasher<UserDetails> hasher = new PasswordHasher<UserDetails>();
        UserDetails udHash = new UserDetails();
        
        
        // read from db
        udHash = UserMethods.ReadUser(udInput.username, out errormsg);
        udHash.username = udInput.username;
        
        PasswordVerificationResult result = hasher.VerifyHashedPassword(udHash, udHash.passwordEncrypted, udInput.password);
        return result == PasswordVerificationResult.Success;

    }
    
    static public bool Create(UserDetails udInput, out string errormsg, bool update) {
        errormsg = "";
        PasswordHasher<UserDetails> hasher = new PasswordHasher<UserDetails>();
        UserDetails udHash = new UserDetails();
        
        udHash.username = udInput.username;

        if (!update) {
            // first create user and get id
            try {
                udHash.userId = UserMethods.CreateUser(udInput, out errormsg);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                return false;
            }
        }
        else {
            udHash.userId = udInput.userId;
        }
        
        udInput.passwordEncrypted = hasher.HashPassword(udHash, udInput.password);
        //save to db
        UserMethods.UpdateUser(udInput, out errormsg);
        
        return errormsg == "";
    }

    // must also save the id to the cookie in MemStorageHandler
    public async void LoginAuthenticationCookie(HttpContext context, UserDetails ud) {
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, ud.username),
            new Claim(ClaimTypes.NameIdentifier, ud.userId.ToString()),
            new Claim(ClaimTypes.Hash, ud.passwordEncrypted)
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            RedirectUri = "/Recipes/Recipes"
        };

        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity), 
            authProperties);
    }
    
    public async void LogoutAuthenticationCookie(HttpContext context) {
        
        // Clear the existing external cookie
        await context.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
    }
    

    
    
    
    
}