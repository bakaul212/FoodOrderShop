using System.ComponentModel.DataAnnotations;

namespace FoodOrderShop.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
    }
}