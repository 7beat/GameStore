namespace GameStore.Models.ViewModels
{
	public class ShoppingCartVM
	{
		public ICollection<CartItem> CartItems { get; set; }

		public double CartTotal { get; set; }

		// Db
		public IEnumerable<ShoppingCart> ListCart { get; set; }

		public OrderHeader OrderHeader { get; set; }
	}
}