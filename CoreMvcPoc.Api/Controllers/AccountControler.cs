using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CoreMvcPoc.BLL; 
using CoreMvcPoc.DAL; 
using CoreMvcPoc.Entities;
namespace CoreMvcPoc.Api
{
    [Route("api/[controller]")] 
    public class AccountController : Controller
    {   
        private readonly IUserService userService;     
        public AccountController(IUserService userService)
        {
            this.userService = userService;
        }
        
        [HttpPost]
        public IActionResult Post([FromBody]User model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = userService.SignUp(model);
            if(result.Status)
            {
                return new JsonResult("Account created");
            }
            else {
                return new BadRequestObjectResult(result.Message); 
            }           
        }
    }

}