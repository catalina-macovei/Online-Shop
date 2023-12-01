using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Product
    {
        [Key]        
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        //poza ??

        public double Price { get; set; }

        public int Stock {  get; set; }

        public int UserId { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
