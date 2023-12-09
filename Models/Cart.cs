using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Cart
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]   // ne asiguram ca id are autoincrement si niciodata nu se va repeta
        public int Id { get; set; }
        public int Quantity { get; set; }  
        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
