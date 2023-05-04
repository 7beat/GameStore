using GameStore.Models.Dto;

namespace GameStore.Models.ViewModels
{
	public class ShoppingCartVM
	{
		public IEnumerable<ShoppingCart> ListCart { get; set; }

		public OrderHeaderDto OrderHeader { get; set; }
	}
}