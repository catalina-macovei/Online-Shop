﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Product
    {
        [Key]        
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string PhotoSrc { get; set; }    

        public double Price { get; set; }
        public int Rating { get; set; }

        public int Stock {  get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Categories { get; set; }
        public virtual ICollection<Cart>? Carts { get; set; }
        public virtual ICollection<Ordered_Product>? Ordered_Products { get; set; }
    }
}
