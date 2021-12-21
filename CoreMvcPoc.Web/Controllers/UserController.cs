using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreMvcPoc.Entities;
using ServiceManager;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace CoreMvcPoc.Web.Controllers
{
    public class UserController : Controller
    {
        private WebServiceManager webServiceManager;
        public UserController()
        {
             this.webServiceManager = new WebServiceManager();   
        }
        public IActionResult SignUp()
        {
            var model = new User();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(User model)
        {
            if(! ModelState.IsValid) { // re-render the view when validation failed.
                return View("SignUp", model);
            }
            var result = await SignUpRequest(model);
            if(result.Success)
            {
               return RedirectToAction("Login");
            }
            return new BadRequestObjectResult("Error in signup");
        }
        
        public IActionResult Login()
        {
            var model = new CredentialsViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(CredentialsViewModel model)
        {
            if(! ModelState.IsValid) 
            { // re-render the view when validation failed.
                return View("Login", model);
            }

            var result = await webServiceManager.Post<TokenResponse, CredentialsViewModel>(string.Format("{0}{1}", Constant.BaseApiUrl,Constant.LoginUrl), model); 
            if(result.Success)
            {              
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.UserName),
                    new Claim("UserToken", result.ResponseObject.Token ),
                    new Claim(ClaimTypes.Role , Convert.ToString((RoleType) result.ResponseObject.Role))
                };

                // create identity
                ClaimsIdentity identity = new ClaimsIdentity(claims, "cookie");

                // create principal
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                // sign-in
                await HttpContext.SignInAsync(
                        scheme: "CustomSecurityScheme",
                        principal: principal);
                 var reditectToActionResult = (RoleType)result.ResponseObject.Role == RoleType.User ? RedirectToAction("Index", "Books") :   RedirectToAction("Index", "Home", new {area = "Admin"});     
                 return reditectToActionResult;
            }
            return new BadRequestObjectResult("Error in signup");           
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                    scheme: "CustomSecurityScheme");

            return RedirectToAction("Login");
        }


        [NonAction]
        private async Task<ResponseData<SignupResponse>> SignUpRequest(User model)
        {
            return await webServiceManager.Post<SignupResponse, User>(string.Format("{0}{1}", Constant.BaseApiUrl,Constant.SignupUrl), model); 
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
        
    }    
}