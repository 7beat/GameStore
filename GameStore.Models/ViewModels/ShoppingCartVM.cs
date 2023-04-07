namespace GameStore.Models.ViewModels
{
	public class ShoppingCartVM
	{
		public ICollection<CartItem> CartItems { get; set; }

		public double CartTotal { get; set; }
	}
}