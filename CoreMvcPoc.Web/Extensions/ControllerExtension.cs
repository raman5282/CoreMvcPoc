using Microsoft.AspNetCore.Mvc;
using ServiceManager;

public static class ControllerExtension
{
    public static RedirectToActionResult CheckAuthorization<T>(this Controller controller, ResponseData<T> result)
    {
        if(result.Message.ToLower().Contains("unauthorize"))
        { 
            return controller.RedirectToAction("Logout" , "User");
        }
        return null;
    }

}