using dbwbs_labb3.Models.CookieHandler;
using dbwbs_projekt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dbwbs_projekt.Controllers; 

public class AccountPageController : Controller {
    
    [Authorize]
    public IActionResult AccountPage() {
        ControllerCommon.SetMetaData(HttpContext, ViewData, "Account Page",
            "Account Page for the recipe site", new List<string>() {
                "account", "account page", "food", "food recipes", "account site", "change password"
            });
        
        ViewBag.username = MemStorageHandler.GetUserDetailsCookie(HttpContext).username;
        return View();
    }   
    
    
    public RedirectResult ResetPassword(string newPassword) {
        Authentication auth = new Authentication();
        
        UserDetails ud = MemStorageHandler.GetUserDetailsCookie(HttpContext);
        ud.password = newPassword;

        string errormsg = "";
        Authentication.Create(ud, out errormsg, true);
        if (errormsg != "") {
            ViewBag.unsuccess = errormsg;
        } else {
            ViewBag.success = "Password changed!";
        }

        return new RedirectResult("AccountPage");
    }
    
    
    public RedirectResult Logout() {
        Authentication auth = new Authentication();
        auth.LogoutAuthenticationCookie(HttpContext);
        MemStorageHandler.ClearUserDetailsCookie(HttpContext);
        return Redirect("/Account/Login");
    }
    
}