using CsvHelper;
using dbwbs_labb3.Models.CookieHandler;
using dbwbs_projekt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace dbwbs_projekt.Controllers; 

public class AccountController : Controller {
    
    
    // GET
    [HttpGet]
    public IActionResult Login() {
        ViewBag.verify = false;
        ControllerCommon.SetMetaData(HttpContext, ViewData, "Login",
            "Login to the recipe site", new List<string>() {
                "login", "recipe", "food", "food recipes", "login page"
            });
        
        return View();
    }

    [HttpPost]
    public IActionResult Login(UserDetails? udInput) {
        
        
        if (udInput == null || udInput.username == null || udInput.password == null) {
            ViewBag.unsuccess = "You must fill all the fields!";
            return View();
        }
        
        
        if (!ModelState.IsValid) {
            ViewBag.unsuccess =
                "Password must contain at least one letter,\n one digit, be at least 8 characters long, and have at least one special character";
            return View();
        }
        
        
        if (udInput.login == 1) {
            if (Authentication.Login(udInput)) {
                Console.WriteLine("Credentials verified");
                UserDetails ud = UserMethods.ReadUser(udInput.username, out _);
                ud.username = udInput.username;
                Authentication createAuth = new Authentication();
                createAuth.LoginAuthenticationCookie(HttpContext, ud);
                MemStorageHandler.SaveUserDetailsToCookie(new UserDetails() {
                    userId = ud.userId,
                    username = udInput.username
                }, HttpContext);
                RedirectToAction("Recipes", "Recipes");
            } else {
                ViewBag.unsuccess = "Wrong username or password!";
            }
        }
        else if (udInput.create == 1) {
            string errormsg = "";
            if (Authentication.Create(udInput, out errormsg, false)) {
                ViewBag.success = "User " + udInput.username + " created!";
            }
            else {
                ViewBag.unsuccess = errormsg;
            }
        }
        
        
        return View();

    }

}