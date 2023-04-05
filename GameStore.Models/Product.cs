using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Developer { get; set; }
        [Required]
        [Range(1, 1000)]
        public double Price { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }

        // Nav
        [Required]
        [Display(Name = "Platform")]
        public int PlatformId { get; set; }
        [ValidateNever]
        public Platform Platform { get; set; }
        [Required]
        [Display(Name = "Genre")]
        public int GenreId { get; set; }
        [ValidateNever]
        public Genre Genre { get; set; }
    }
}
