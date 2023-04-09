using GameStore.Models.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Models
{
	[JsonObject(MemberSerialization.OptIn)]
	public class ShoppingCart
	{
		public int Id { get; set; }
		[JsonProperty]
		public int ProductId { get; set; }
		[ForeignKey("ProductId")]
		[ValidateNever]
		public Product Product { get; set; }
		[JsonProperty]
		[Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
		public int Count { get; set; }

		public string ApplicationUserId { get; set; }
		[ForeignKey("ApplicationUserId")]
		[ValidateNever]
		public ApplicationUser ApplicationUser { get; set; }
	}
}
