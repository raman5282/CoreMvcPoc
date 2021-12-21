using System;
using System.ComponentModel.DataAnnotations;
namespace CoreMvcPoc.Entities
{
    public class User : BaseModel
    {    
        public User()
        {
            this.Role=2;
        }    
        [Required(ErrorMessage = "Email is required")]  
        [EmailAddress(ErrorMessage="Enter a valid email address")]      
        public string Email { get; set; }   
        [Required(ErrorMessage = "Password is required")]        
        public string Password { get; set; }         
        [Required(ErrorMessage = "First name is required")]   
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]   
        public string LastName { get; set; }

        public string Token { get; set; }   
        public int Role { get; set; }      
    }
}