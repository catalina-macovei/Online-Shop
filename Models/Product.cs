using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Product
    {
        [Key]        
        public int Id { get; set; }

        [Required(ErrorMessage = "Name of the product is required!")]
        [StringLength(100, ErrorMessage = "Name can't have more than 100 characters!")]
        [MinLength(3, ErrorMessage = "Name can't have less than 3 characters!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description required!")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Photo required!")]
        public string PhotoSrc { get; set; } = "/images/products/default.png";

        [Required(ErrorMessage = "Price is required!")]
        [Range(5, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 5.")]
        public double Price { get; set; }
        public int? Rating { get; set; } // not required

        [Required(ErrorMessage = "Number of items required!")]
        [Range(1, double.MaxValue, ErrorMessage = "Number of items required!")]
        public int Stock {  get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [Required(ErrorMessage = "Category required!")]
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Categories { get; set; }
        public virtual ICollection<Cart>? Carts { get; set; }
        public virtual ICollection<Ordered_Product>? Ordered_Products { get; set; }
    }
}
