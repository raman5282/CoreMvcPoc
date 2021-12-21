using System.ComponentModel.DataAnnotations;

namespace CoreMvcPoc.Entities
{
    public class CredentialsViewModel
    {
        [Required(ErrorMessage="Email is required")]
        [Display(Name="Email")]
        [EmailAddress(ErrorMessage="Enter valid email")]
        public string UserName { get; set; }
        [Required(ErrorMessage="Password is required")]
        public string Password { get; set; }
    }
}