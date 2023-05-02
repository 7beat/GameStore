using System.ComponentModel.DataAnnotations;

namespace GameStore.Models.Dto
{
    public class OrderHeaderDto
    {
        public double OrderTotal { get; set; }

        [Required]
        [EmailAddress]
        public string? GuestEmailAddress { get; set; }
        [Required]
        [StringLength(13, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 10)]
        public string? PhoneNumber { get; set; }
        [Required]
        public string? StreetAddress { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? State { get; set; }
        [Required]
        public string? PostalCode { get; set; }
        [Required]
        public string? Name { get; set; }

        public bool IsDigital { get; set; } = true;
    }
}

