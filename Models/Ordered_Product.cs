using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Ordered_Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }

        public int? OrderId { get; set; }
        public virtual Order? Order { get; set; }
    }
}
