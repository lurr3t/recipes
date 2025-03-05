using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace dbwbs_projekt.Controllers; 

public class ControllerCommon : Controller {

    public static void SetMetaData(HttpContext context, ViewDataDictionary vdd, string title, string description,
        List<string> keys) {
        vdd["Title"] = title;
        vdd["Url"] = context.Request.GetDisplayUrl();
        vdd["Description"] = description;
        vdd["Keys"] = keys;
    }
    
    
}