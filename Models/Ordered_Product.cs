using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Ordered_Product
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }

    }
}
