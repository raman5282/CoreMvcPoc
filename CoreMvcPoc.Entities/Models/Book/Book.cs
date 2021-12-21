using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreMvcPoc.Entities
{
    public class Book : BaseModel
    {
      
        [Required(ErrorMessage = "Name is required")]        
        public string Name { get; set; }   
        [Required(ErrorMessage = "Author is required")]        
        public string Author { get; set; }         
        [Required(ErrorMessage = "Price is required")]   
        [Range(1.0, double.MaxValue, ErrorMessage = "Enter a valid price")]
        public string Price { get; set; }       
        public int UserId { get; set; }
        public string Image { get; set; }
    }
}