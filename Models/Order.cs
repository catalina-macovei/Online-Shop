using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Order
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Your first name is mandatory")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Your last name is mandatory")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "The city is mandatory")]
        public string? City { get; set; }

        [Required(ErrorMessage = "The address is mandatory")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Your number of telephone is mandatory")]
        public string? Telephone { get; set; }

        [Required(ErrorMessage = "Your email is mandatory")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "The choice of the delivery method is mandatory")]
        public string? Delivery { get; set; }

        [Required(ErrorMessage = "The choice of the payment method is mandatory")]
        public string? Payment {  get; set; }
        public double TotalPrice { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<Ordered_Product>? Ordered_Products { get; set; }

    }
}
