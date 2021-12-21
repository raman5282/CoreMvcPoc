using System.ComponentModel.DataAnnotations;

namespace CoreMvcPoc.Entities
{
    public class BaseModel
    {
        [Key]
        public int Id { get; set; }
    }
}