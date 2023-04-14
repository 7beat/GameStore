using Newtonsoft.Json;

namespace GameStore.Models
{
	public class CartItem
	{
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public int Quantity { get; set; }

		[JsonIgnore]
		public Product Product { get; set; }
	}
}
