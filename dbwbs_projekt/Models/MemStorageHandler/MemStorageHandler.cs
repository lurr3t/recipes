using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using dbwbs_projekt.Models;
using dbwbs_projekt.Models.Recipes;
using dbwbs_projekt.Models.ShoppingLists;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace dbwbs_labb3.Models.CookieHandler; 

public class MemStorageHandler {
    
    // Timeout in seconds
    private const int cookieTimeout = 3600;
    

    public static void SaveRecipesViewModelToSession(RecipesViewModel rvm, HttpContext context) {
        try {
            context.Session.SetString("recipesViewModel", JsonConvert.SerializeObject(rvm));
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }

    public static void ClearRecipesViewModelSession(HttpContext context) {
        context.Session.Clear();
    }
    
    public static RecipesViewModel? GetRecipesViewModelSession(HttpContext context) {

        string? rvm = context.Session.GetString("recipesViewModel");
        if (rvm != null) {
            return JsonConvert.DeserializeObject<RecipesViewModel>(rvm);    
        }
        return null;
    }
    
    
    public static void SaveRecipesViewModelToCookie(RecipesViewModel rm, HttpContext context) {
        try {
            string cookieString = JsonConvert.SerializeObject(rm);
        
            // Adding options for the cookies
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires = new DateTimeOffset(DateTime.Now.AddSeconds(cookieTimeout));

            context.Response.Cookies.Append("recipes", cookieString, cookieOptions);
            
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }
    
    
    public static void SaveShoppingListPageDetailsToCookie(ShoppingListPageDetails content, HttpContext context) {
        try {
            string cookieString = JsonConvert.SerializeObject(content);
            // Adding options for the cookies
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires = new DateTimeOffset(DateTime.Now.AddSeconds(cookieTimeout));
            context.Response.Cookies.Append("slPageDetails", cookieString, cookieOptions);
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public static void ClearShoppingListPageDetailsToCookie(HttpContext context) {
        try { 
            string cookieString = JsonConvert.SerializeObject(null); 
            context.Response.Cookies.Append("slPageDetails", cookieString); 
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public static ShoppingListPageDetails? GetShoppingListPageDetailsCookie(HttpContext context) {
        if (context.Request.Cookies.ContainsKey("slPageDetails")) {
            string cookieString = context.Request.Cookies["slPageDetails"];
            return JsonConvert.DeserializeObject<ShoppingListPageDetails>(cookieString);    
        }
        return null;
    }
    
    public static RecipesViewModel? GetRecipesViewModelCookie(HttpContext context) {
        if (context.Request.Cookies.ContainsKey("recipes")) {
            string cookieString = context.Request.Cookies["recipes"];
            return JsonConvert.DeserializeObject<RecipesViewModel>(cookieString);    
        }
        return null;
    }
    
    
    public static void ClearRecipesViewModelCookie(HttpContext context) {
        try { 
            string cookieString = JsonConvert.SerializeObject(null); 
            context.Response.Cookies.Append("recipes", cookieString); 
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }
    
    
    
    public static void ClearUserDetailsCookie(HttpContext context) {
        try { 
            string cookieString = JsonConvert.SerializeObject(null); 
            context.Response.Cookies.Append("userDetails", cookieString); 
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public static UserDetails? GetUserDetailsCookie(HttpContext context) {
        if (context.Request.Cookies.ContainsKey("userDetails")) {
            string cookieString = context.Request.Cookies["userDetails"];
            return JsonConvert.DeserializeObject<UserDetails>(cookieString);    
        }
        return null;
    }
    
    public static void SaveUserDetailsToCookie(UserDetails content, HttpContext context) {
        try {
            string cookieString = JsonConvert.SerializeObject(content);
            // Adding options for the cookies
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires = new DateTimeOffset(DateTime.Now.AddSeconds(cookieTimeout));
            context.Response.Cookies.Append("userDetails", cookieString, cookieOptions);
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }

    
    public static void SaveToCookie<T>(T content, HttpContext context, string key) {
        try {
            string cookieString = JsonConvert.SerializeObject(content);
            // Adding options for the cookies
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires = new DateTimeOffset(DateTime.Now.AddSeconds(cookieTimeout));
            context.Response.Cookies.Append(key, cookieString, cookieOptions);
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public static T GetCookie<T>(HttpContext context, string key) {
        
        if (context.Request.Cookies.ContainsKey(key)) {
            string cookieString = context.Request.Cookies[key];
            return JsonConvert.DeserializeObject<T>(cookieString);    
        }
        throw new Exception();
    }
    
    public static void ClearCookie(HttpContext context, string key) {
        try { 
            string cookieString = JsonConvert.SerializeObject(null); 
            context.Response.Cookies.Append(key, cookieString); 
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }
    
    
    
}