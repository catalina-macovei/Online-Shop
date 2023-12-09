using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="The content is mandatory")]
        public string? Content { get; set; }
        public DateTime Date { get; set; }
        public double? Rating { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }
    }
}
